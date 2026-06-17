using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour, IDamageable, IMovable
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    private float baseSpeed;

    [Header("Rotacion de sprites")]
    [SerializeField] private Sprite[] rotationCycle;
    [SerializeField] private float spriteChangeInterval = 0.15f;

    [Header("Integridad")]
    [SerializeField] private float maxIntegrity = 100f;
    private float currentIntegrity;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private int currentSpriteIndex = 0;
    private float spriteTimer = 0f;
    private bool isMoving = false;

    [SerializeField] private IntegrityBar integrityBar;

    [Header("Efectos de daño")]
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private float blinkDuration = 0.4f;
    [SerializeField] private float blinkOpacity = 0.75f;

    [Header("Regeneración")]
    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private float regenPerSecond = 5f;
    [SerializeField] private float regenMaxIntegrity = 75f;

    private float lastDamageTime = -999f;
    private bool isBlinking = false;

    [Header("Power-ups")]
    private bool isInvisible = false;
    public bool IsInvisible => isInvisible;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentIntegrity = maxIntegrity;
        baseSpeed = speed;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;
        isMoving = moveInput != Vector2.zero;

        HandleSpriteRotation();

        HandleRegeneration();
    }

    private void FixedUpdate()
    {
        Move(moveInput);
    }

    private void HandleSpriteRotation()
    {
        if (!isMoving)
        {
            spriteTimer = 0f;
            return;
        }

        spriteTimer += Time.deltaTime;

        if (spriteTimer >= spriteChangeInterval)
        {
            spriteTimer = 0f;
            currentSpriteIndex = (currentSpriteIndex + 1) % rotationCycle.Length;
            spriteRenderer.sprite = rotationCycle[currentSpriteIndex];
        }
    }

    // IMovable
    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    // IDamageable
    public void TakeDamage(float amount)
    {
        if (isInvisible) return;

        currentIntegrity -= amount;
        currentIntegrity = Mathf.Clamp(currentIntegrity, 0f, maxIntegrity);
        lastDamageTime = Time.time;

        integrityBar.UpdateBar(currentIntegrity, maxIntegrity);

        // Efectos visuales
        CameraShake.Instance?.ShakeOnDamage();
        StartCoroutine(FlashRed());
        if (!isBlinking) StartCoroutine(BlinkRoutine());

        if (currentIntegrity <= 0f) Die();
    }

    public void Die()
    {
        GameManager.Instance.OnPlayerDied();
    }

    public void Respawn()
    {
        currentIntegrity = maxIntegrity;
        speed = baseSpeed; // ← agregar esto
        spriteRenderer.color = Color.white;
        isInvisible = false;
        integrityBar.UpdateBar(currentIntegrity, maxIntegrity);
    }

    public float GetIntegrity() => currentIntegrity;
    public float GetMaxIntegrity() => maxIntegrity;

    public IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        // Guardar velocidad original ANTES de modificar
        float originalSpeed = speed;
        speed = originalSpeed * multiplier;

        spriteRenderer.color = new Color(0.4f, 0.7f, 1f, 1f);
        HUDManager.Instance?.ShowPowerUp("@Override", duration);

        yield return new WaitForSeconds(duration);

        // Restaurar solo si la velocidad actual sigue siendo la modificada
        // (evita restaurar si ya hubo otro powerup encima)
        if (Mathf.Approximately(speed, originalSpeed * multiplier))
            speed = originalSpeed;

        spriteRenderer.color = Color.white;
        HUDManager.Instance?.HidePowerUp();
    }
    public IEnumerator InvisibilityRoutine(float duration)
    {
        isInvisible = true;

        // Color verdoso semitransparente
        spriteRenderer.color = new Color(0.4f, 1f, 0.4f, 0.5f);

        HUDManager.Instance?.ShowPowerUp("// comentario", duration);

        yield return new WaitForSeconds(duration);

        isInvisible = false;
        spriteRenderer.color = Color.white;
        HUDManager.Instance?.HidePowerUp();
    }

    private void HandleRegeneration()
    {
        if (currentIntegrity <= 0f) return;
        if (currentIntegrity >= regenMaxIntegrity) return;
        if (currentIntegrity > maxIntegrity * 0.3f) return; // solo bajo el 30%

        if (Time.time - lastDamageTime >= regenDelay)
        {
            currentIntegrity += regenPerSecond * Time.deltaTime;
            currentIntegrity = Mathf.Min(currentIntegrity, regenMaxIntegrity);
            integrityBar.UpdateBar(currentIntegrity, maxIntegrity);
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(flashDuration);
        if (!isBlinking)
            spriteRenderer.color = Color.white;
    }

    private IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            // Alternar entre opacidad completa y reducida
            float alpha = Mathf.PingPong(elapsed / blinkInterval, 1f) > 0.5f
                ? 1f : blinkOpacity;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        spriteRenderer.color = Color.white;
        isBlinking = false;
    }
}
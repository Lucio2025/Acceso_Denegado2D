using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour, IDamageable, IMovable
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;

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

    [Header("Power-ups")]
    private bool isInvisible = false;
    public bool IsInvisible => isInvisible;

    private float baseSpeed;

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
        integrityBar.UpdateBar(currentIntegrity, maxIntegrity);
        if (currentIntegrity <= 0f) Die();
    }

    public void Die()
    {
        GameManager.Instance.OnPlayerDied();
    }

    public void Respawn()
    {
        currentIntegrity = maxIntegrity;
        integrityBar.UpdateBar(currentIntegrity, maxIntegrity);
    }

    public float GetIntegrity() => currentIntegrity;
    public float GetMaxIntegrity() => maxIntegrity;

    public IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        speed = baseSpeed * multiplier;
        GameManager.Instance.ShowPowerUpUI("@Override", duration);
        yield return new WaitForSeconds(duration);
        speed = baseSpeed;
        GameManager.Instance.HidePowerUpUI();
    }
    public IEnumerator InvisibilityRoutine(float duration)
    {
        isInvisible = true;
        Color c = spriteRenderer.color;
        c.a = 0.4f;
        spriteRenderer.color = c;

        GameManager.Instance.ShowPowerUpUI("// comentario", duration);
        yield return new WaitForSeconds(duration);

        isInvisible = false;
        c.a = 1f;
        spriteRenderer.color = c;
        GameManager.Instance.HidePowerUpUI();
    }
}
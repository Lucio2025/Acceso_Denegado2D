using UnityEngine;
using System.Collections;

public class TheCompiler : MonoBehaviour, IDamageable
{
    [Header("Vida — dividida en 3 fases")]
    [SerializeField] private float maxHealth = 300f;
    private float currentHealth;
    private int currentPhase = 0; // 0, 1, 2

    [Header("Proyectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject warningPrefab; // línea punteada
    [SerializeField] private Transform[] firePoints;   // puntos de disparo
    [SerializeField] private float warningDuration = 1.5f;

    [Header("Cadencia por fase")]
    [SerializeField] private float[] fireCooldowns = { 2.5f, 1.8f, 1.2f };

    [Header("Botones de la sala")]
    [SerializeField] private BossButton[] bossButtons; // 3 botones

    [Header("Cámaras de la sala")]
    [SerializeField] private SecurityCamera[] roomCameras;

    [Header("Barrera de entrada")]
    [SerializeField] private GameObject entranceBarrier;

    [Header("Salida")]
    [SerializeField] private GameObject exitBarrier;

    private SpriteRenderer sr;
    private bool isDead = false;
    private int buttonsPressed = 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void StartBattle()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Desactivar cámaras al inicio
        SetCamerasActive(false);

        // Suscribir botones
        for (int i = 0; i < bossButtons.Length; i++)
        {
            int index = i; // capturar para el closure
            bossButtons[i].OnButtonPressed += () => OnBossButtonPressed(index);
        }

        StartCoroutine(BattleRoutine());
    }

    // ── Rutina principal ───────────────────────────────────

    private IEnumerator BattleRoutine()
    {
        // Pausa dramática al entrar
        yield return new WaitForSeconds(2f);
        CameraShake.Instance?.Shake(0.3f, 0.1f);

        StartCoroutine(ButtonActivationRoutine()); // ← agregar esto

        // Ciclo de ataque
        while (!isDead)
        {
            yield return StartCoroutine(AttackCycle());
        }
    }

    private IEnumerator AttackCycle()
    {
        // Advertencia visual antes de disparar
        yield return StartCoroutine(ShowWarning());

        // Disparar
        Shoot();

        float cooldown = fireCooldowns[Mathf.Min(currentPhase, fireCooldowns.Length - 1)];
        yield return new WaitForSeconds(cooldown);
    }

    // ── Advertencia ────────────────────────────────────────

    private IEnumerator ShowWarning()
    {
        if (warningPrefab == null || firePoints.Length == 0)
        {
            yield return new WaitForSeconds(warningDuration);
            yield break;
        }

        // Elegir punto de fuego según fase
        Transform firePoint = firePoints[currentPhase % firePoints.Length];

        GameObject warning = Instantiate(warningPrefab, firePoint.position,
            firePoint.rotation);
        Destroy(warning, warningDuration);

        yield return new WaitForSeconds(warningDuration);
    }

    // ── Disparo ────────────────────────────────────────────

    private void Shoot()
    {
        if (projectilePrefab == null || firePoints.Length == 0) return;

        Transform firePoint = firePoints[currentPhase % firePoints.Length];
        GameObject proj = Instantiate(projectilePrefab, firePoint.position,
            firePoint.rotation);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Disparar en la dirección del firePoint
            rb.linearVelocity = firePoint.up * GetProjectileSpeed();
        }
    }

    private float GetProjectileSpeed()
    {
        return currentPhase switch
        {
            0 => 3f,
            1 => 4.5f,
            2 => 6f,
            _ => 3f
        };
    }

    // ── Botones ────────────────────────────────────────────

    private void OnBossButtonPressed(int buttonIndex)
    {
        buttonsPressed++;
        AdvancePhase();
    }

    private void AdvancePhase()
    {
        currentPhase++;
        currentHealth = maxHealth * (1f - currentPhase / 3f);
        UpdateHealthBar();

        CameraShake.Instance?.Shake(0.4f, 0.15f);
        StartCoroutine(FlashRed());

        if (currentPhase == 1)
        {
            // Fase 2: encender una cámara
            if (roomCameras.Length > 0)
                roomCameras[0].gameObject.SetActive(true);
        }
        else if (currentPhase == 2)
        {
            // Fase 3: apagar esa cámara, encender todas las demás
            SetCamerasActive(true);
        }
        else if (currentPhase >= 3)
        {
            Die();
        }
    }

    // ── Activar botón de fase ──────────────────────────────

    // Llamado desde el BattleRoutine cada X segundos
    private IEnumerator ButtonActivationRoutine()
    {
        float[] buttonTimes = { 6f, 7f, 7f }; // tiempo hasta activar botón por fase

        for (int i = 0; i < bossButtons.Length; i++)
        {
            yield return new WaitForSeconds(buttonTimes[i]);
            if (!isDead && i < bossButtons.Length)
                bossButtons[i].Activate();
        }
    }

    // ── IDamageable ────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        // El jefe no recibe daño de proyectiles, solo de botones
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        SetCamerasActive(false);

        if (exitBarrier != null)
            exitBarrier.SetActive(false);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Parpadeo de muerte
        for (int i = 0; i < 6; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        // Fade out
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            sr.color = c;
            yield return null;
        }

        GameManager.Instance?.OnPlayerWon();
        Destroy(gameObject);
    }

    // ── Utilidades ─────────────────────────────────────────

    private void UpdateHealthBar()
    {
        // Usar el HUD si tienen barra de jefe, o dejarlo sin barra por ahora
        float ratio = currentHealth / maxHealth;
        Debug.Log($"Jefe salud: {Mathf.RoundToInt(ratio * 100)}%");
    }

    private void SetCamerasActive(bool active)
    {
        foreach (var cam in roomCameras)
            if (cam != null) cam.gameObject.SetActive(active);
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
    }
}
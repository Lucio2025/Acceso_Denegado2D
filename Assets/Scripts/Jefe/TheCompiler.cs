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
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float warningDuration = 1.5f;

    [Header("Cadencia por fase")]
    [SerializeField] private float[] fireCooldowns = { 2.5f, 1.8f, 1.2f };

    [Header("Botones de la sala")]
    [SerializeField] private BossButton[] bossButtons;

    [Header("Cámaras de la sala")]
    [SerializeField] private SecurityCamera[] roomCameras;

    [Header("Barrera de entrada")]
    [SerializeField] private GameObject entranceBarrier;

    [Header("Salida")]
    [SerializeField] private GameObject exitBarrier;

    [Header("Trigger de sala")]
    [SerializeField] private BossRoomTrigger bossRoomTrigger;

    private SpriteRenderer sr;
    private bool isDead = false;
    private int buttonsPressed = 0;

    private Transform lastFirePoint;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void StartBattle()
    {
        currentHealth = maxHealth;
        BossHealthBar.Instance?.Show();

        UpdateHealthBar();

        SetCamerasActive(false);

        for (int i = 0; i < bossButtons.Length; i++)
        {
            int index = i;
            bossButtons[i].OnButtonPressed += () => OnBossButtonPressed(index);
        }

        StartCoroutine(BattleRoutine());
    }

    // ── Rutina principal ───────────────────────────────────

    private IEnumerator BattleRoutine()
    {
        yield return new WaitForSeconds(2f);
        CameraShake.Instance?.Shake(0.3f, 0.1f);

        StartCoroutine(ActivateCurrentPhaseButton());

        while (!isDead)
        {
            yield return StartCoroutine(AttackCycle());
        }
    }

    private IEnumerator AttackCycle()
    {
        yield return StartCoroutine(ShowWarning());
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

        Transform firePoint = firePoints[Random.Range(0, firePoints.Length)];
        lastFirePoint = firePoint;

        GameObject warning = Instantiate(warningPrefab, firePoint.position,
            firePoint.rotation);
        Destroy(warning, warningDuration);

        yield return new WaitForSeconds(warningDuration);
    }

    // ── Disparo ────────────────────────────────────────────

    private void Shoot()
    {
        if (projectilePrefab == null || lastFirePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, lastFirePoint.position,
            lastFirePoint.rotation);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = lastFirePoint.up * GetProjectileSpeed();
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
        if (buttonIndex != currentPhase) return;

        buttonsPressed++;
        AdvancePhase();
    }

    private void AdvancePhase()
    {
        currentPhase++;
        currentHealth = maxHealth * (1f - currentPhase / 3f);

        CameraShake.Instance?.Shake(0.4f, 0.15f);
        StartCoroutine(FlashRed());
        UpdateHealthBar();

        if (currentPhase == 1)
        {
            if (roomCameras.Length > 0)
                roomCameras[0].gameObject.SetActive(true);

            StartCoroutine(ActivateCurrentPhaseButton());
        }
        else if (currentPhase == 2)
        {
            SetCamerasActive(true);

            StartCoroutine(ActivateCurrentPhaseButton());
        }
        else if (currentPhase >= 3)
        {
            Die();
        }
    }

    // ── Activar botón de fase ──────────────────────────────

    private IEnumerator ActivateCurrentPhaseButton()
    {
        float[] buttonTimes = { 6f, 7f, 7f };
        int phaseIndex = currentPhase;

        if (phaseIndex >= bossButtons.Length) yield break;

        yield return new WaitForSeconds(buttonTimes[phaseIndex]);

        while (!isDead && !bossButtons[phaseIndex].IsPressed())
        {
            bossButtons[phaseIndex].Activate();

            yield return new WaitForSeconds(bossButtons[phaseIndex].GetActiveWindow() + 3f);
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

        BossHealthBar.Instance?.Hide();

        SetCamerasActive(false);

        if (exitBarrier != null)
            exitBarrier.SetActive(false);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        for (int i = 0; i < 6; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

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
        float ratio = currentHealth / maxHealth;
        BossHealthBar.Instance?.UpdateHealth(ratio);
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

    public void ResetBoss()
    {
        if (!isDead) StopAllCoroutines();

        isDead = false;
        currentPhase = 0;
        currentHealth = maxHealth;
        buttonsPressed = 0;

        foreach (var btn in bossButtons)
            if (btn != null) btn.ResetButton();

        SetCamerasActive(false);
        BossHealthBar.Instance?.Hide();

        if (entranceBarrier != null)
            entranceBarrier.SetActive(false);

        if (bossRoomTrigger != null)
            bossRoomTrigger.Reset();
    }

}
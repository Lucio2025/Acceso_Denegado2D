using UnityEngine;
using System.Collections;

public class BossButton : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Tiempo activo")]
    [SerializeField] private float activeWindow = 8f;

    public event System.Action OnButtonPressed;

    private SpriteRenderer sr;
    private bool isActive = false;
    private bool isPressed = false;
    private Coroutine activeCoroutine;

    public float GetActiveWindow() => activeWindow;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
    }

    public void Activate()
    {
        if (isPressed) return;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ActiveWindowRoutine());
    }

    private IEnumerator ActiveWindowRoutine()
    {
        isActive = true;
        sr.sprite = activeSprite;

        yield return new WaitForSeconds(activeWindow);

        isActive = false;
        sr.sprite = idleSprite;
        activeCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isActive || isPressed) return;

        isPressed = true;
        isActive = false;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);

        sr.sprite = pressedSprite;
        OnButtonPressed?.Invoke();
    }

    public bool IsPressed() => isPressed;

    public void ResetButton()
    {
        isPressed = false;
        isActive = false;
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = null;
        if (sr != null && idleSprite != null)
            sr.sprite = idleSprite;
    }
}
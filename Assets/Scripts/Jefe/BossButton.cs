using UnityEngine;
using System.Collections;

public class BossButton : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;    // apagado
    [SerializeField] private Sprite activeSprite;  // encendido, pisable
    [SerializeField] private Sprite pressedSprite; // pisado

    [Header("Tiempo activo")]
    [SerializeField] private float activeWindow = 8f;

    public event System.Action OnButtonPressed;

    private SpriteRenderer sr;
    private bool isActive = false;
    private bool isPressed = false;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
    }

    // El jefe llama esto para encender el botón
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

        // Se apagó sin que lo pisaran
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
}
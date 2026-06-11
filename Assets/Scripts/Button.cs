using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Estado")]
    private bool isPressed = false;
    private SpriteRenderer spriteRenderer;

    // Evento: otros objetos pueden suscribirse para saber cuando se activa
    public event System.Action OnPressed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }

    // Se llama cuando el jugador pisa el botón (via trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            Interact();
        }
    }

    // IInteractable
    public void Interact()
    {
        isPressed = true;
        spriteRenderer.sprite = pressedSprite;
        OnPressed?.Invoke(); // Avisa a quien esté escuchando
        Debug.Log($"{gameObject.name} activado!");
    }

    public bool IsPressed() => isPressed;
}
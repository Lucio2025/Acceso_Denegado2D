using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Estado")]
    private bool isPressed = false;
    private SpriteRenderer spriteRenderer;

    public event System.Action OnPressed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            Interact();
        }
    }

    public void Interact()
    {
        isPressed = true;
        spriteRenderer.sprite = pressedSprite;
        CameraShake.Instance?.ShakeOnButton();
        OnPressed?.Invoke();
        Debug.Log($"{gameObject.name} activado!");
    }

    public bool IsPressed() => isPressed;
}
using UnityEngine;

public class YSortable : MonoBehaviour
{
    private SpriteRenderer sr;

    [Tooltip("Offset adicional para objetos que van ENCIMA de otros (ej: computadora sobre mueble = 1)")]
    [SerializeField] private int baseOrder = 0;

    [Tooltip("Ajusta el punto de referencia vertical. Útil para sprites altos.")]
    [SerializeField] private float yOffset = 0f;

    [SerializeField] private float yMin = -20f;
    [SerializeField] private float yMax = 20f;

    [SerializeField] private int orderMin = 2;
    [SerializeField] private int orderMax = 50;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        float y = transform.position.y + yOffset;

        float t = Mathf.InverseLerp(yMin, yMax, y);
        int order = Mathf.RoundToInt(Mathf.Lerp(orderMax, orderMin, t));

        sr.sortingOrder = baseOrder + order;
    }
}
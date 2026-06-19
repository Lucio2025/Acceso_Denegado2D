using UnityEngine;

public class YSortable : MonoBehaviour
{
    private SpriteRenderer sr;

    [Tooltip("Offset adicional para objetos que van ENCIMA de otros (ej: computadora sobre mueble = 1)")]
    [SerializeField] private int baseOrder = 0;

    [Tooltip("Ajusta el punto de referencia vertical. Útil para sprites altos.")]
    [SerializeField] private float yOffset = 0f;

    // Estos valores definen el rango del mapa en unidades de Unity
    // Si tu mapa mide 50 unidades de alto, ponés yMin=-5 yMax=45
    [SerializeField] private float yMin = -20f;
    [SerializeField] private float yMax = 20f;

    // El Order va a variar entre estos valores (nunca fuera de este rango)
    [SerializeField] private int orderMin = 2;
    [SerializeField] private int orderMax = 50;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        float y = transform.position.y + yOffset;

        // Mapear posición Y al rango de Order (invertido: más abajo = Order mayor)
        float t = Mathf.InverseLerp(yMin, yMax, y); // 0 a 1
        int order = Mathf.RoundToInt(Mathf.Lerp(orderMax, orderMin, t));

        sr.sortingOrder = baseOrder + order;
    }
}
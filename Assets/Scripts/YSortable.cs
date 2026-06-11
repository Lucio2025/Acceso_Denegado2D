using UnityEngine;

public class YSortable : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float yOffset = 0f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-(transform.position.y + yOffset) * 2) + 1000;
    }
}
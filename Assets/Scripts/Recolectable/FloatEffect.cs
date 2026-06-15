using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.12f;
    [SerializeField] private float speed = 1.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float y = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, y, startPosition.z);
    }
}
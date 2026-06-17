using UnityEngine;

public class CompilerProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float lifetime = 4f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<Player>()?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
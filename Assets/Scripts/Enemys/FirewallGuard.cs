using UnityEngine;

public class FirewallGuard : MonoBehaviour, IMovable
{
    [Header("Patrulla")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    [Header("Daño al jugador")]
    [SerializeField] private float damageOnContact = 30f;
    [SerializeField] private float damageCooldown = 1f;

    private Transform currentTarget;
    private float damageTimer = 0f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentTarget = pointB;
    }

    private void Update()
    {
        Patrol();
        damageTimer -= Time.deltaTime;
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        float dirX = currentTarget.position.x - transform.position.x;
        if (Mathf.Abs(dirX) > 0.01f)
            spriteRenderer.flipX = dirX < 0;

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
            currentTarget = currentTarget == pointB ? pointA : pointB;
    }

    // IMovable
    public void Move(Vector2 direction)
    {
        // La patrulla la maneja Patrol()
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ApplyDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (damageTimer <= 0f)
            ApplyDamage(other);
    }

    private void ApplyDamage(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damageOnContact);
            damageTimer = damageCooldown;
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.15f);
        Gizmos.DrawSphere(pointB.position, 0.15f);
    }
}
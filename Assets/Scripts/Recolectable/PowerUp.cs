using UnityEngine;

public abstract class PowerUp : MonoBehaviour, ICollectible
{
    [SerializeField] protected float duration = 5f;

    [SerializeField] protected GameObject collectFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnCollect();
    }

    public void OnCollect()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null) ApplyEffect(player);

        if (collectFX != null)
            Instantiate(collectFX, transform.position, Quaternion.identity);
        //Player player = FindFirstObjectByType<Player>();
        if (player != null) ApplyEffect(player);

        Destroy(gameObject);
    }

    protected abstract void ApplyEffect(Player player);
}
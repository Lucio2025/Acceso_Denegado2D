using UnityEngine;

public class Token : MonoBehaviour, ICollectible
{
    [SerializeField] private int value = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnCollect();
    }

    public void OnCollect()
    {
        GameManager.Instance.AddScore(value);
        Destroy(gameObject);
    }
}
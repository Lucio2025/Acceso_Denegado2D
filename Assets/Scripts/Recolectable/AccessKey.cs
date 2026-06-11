using UnityEngine;

public class AccessKey : MonoBehaviour, ICollectible
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnCollect();
    }

    public void OnCollect()
    {
        GameManager.Instance.AddKey();
        Destroy(gameObject);
    }
}
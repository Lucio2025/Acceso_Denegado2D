using UnityEngine;

public class Token : MonoBehaviour, ICollectible
{
    [SerializeField] private int value = 10;

    [SerializeField] private GameObject collectFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnCollect();
    }

    public void OnCollect()
    {
        GameManager.Instance.AddScore(value);

        if (collectFX != null)
            Instantiate(collectFX, transform.position, Quaternion.identity);
        GameManager.Instance.AddScore(value);

        Destroy(gameObject);
    }
}
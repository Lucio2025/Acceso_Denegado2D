using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private TheCompiler boss;
    [SerializeField] private GameObject entranceBarrier;

    private bool triggered = false;

    private void Start()
    {
        if (entranceBarrier != null)
            entranceBarrier.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (entranceBarrier != null)
            entranceBarrier.SetActive(true);

        if (boss != null)
            boss.StartBattle();

        gameObject.SetActive(false);
    }
}
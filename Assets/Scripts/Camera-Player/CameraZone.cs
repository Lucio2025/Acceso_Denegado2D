using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] private Vector3 cameraTargetPosition;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float followThreshold = 2f;
    [SerializeField] private float followSpeed = 5f;

    private BoxCollider2D zone;
    private bool playerInside = false;

    public enum FollowMode { Fixed, FollowX, FollowY }
    private FollowMode followMode;

    private Transform playerTransform;

    private void Awake()
    {
        zone = GetComponent<BoxCollider2D>();
        DetermineFollowMode();
    }

    private void DetermineFollowMode()
    {
        if (zone == null) return;

        float w = zone.size.x;
        float h = zone.size.y;

        if (w > h + followThreshold)
            followMode = FollowMode.FollowX;
        else if (h > w + followThreshold)
            followMode = FollowMode.FollowY;
        else
            followMode = FollowMode.Fixed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerTransform = other.transform;
        playerInside = true;

        CameraController.Instance.MoveToZone(cameraTargetPosition, followMode,
            followSpeed, playerTransform, zone);

        if (spawnPoint != null)
            GameManager.Instance.SetLastCheckpoint(
                spawnPoint.position, cameraTargetPosition);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void OnDrawGizmos()
    {
        if (zone == null) zone = GetComponent<BoxCollider2D>();
        DetermineFollowMode();

        Gizmos.color = followMode switch
        {
            FollowMode.FollowX => Color.cyan,
            FollowMode.FollowY => Color.green,
            _ => Color.yellow
        };

        if (zone != null)
            Gizmos.DrawWireCube(transform.position, zone.size);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawSphere(cameraTargetPosition, 0.2f);

        if (spawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(spawnPoint.position, 0.15f);
        }
    }
}
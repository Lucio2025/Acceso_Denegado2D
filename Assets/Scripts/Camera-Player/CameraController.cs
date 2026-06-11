using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private FadeController fadeController;

    private CameraZone.FollowMode currentMode = CameraZone.FollowMode.Fixed;
    private float followSpeed = 5f;
    private Transform playerTransform;
    private BoxCollider2D currentZone;
    private Vector3 fixedTarget;
    private bool isTransitioning = false;

    private void Awake()
    {
        Instance = this;
        fixedTarget = transform.position;
    }

    private void LateUpdate()
    {
        if (isTransitioning || playerTransform == null) return;

        if (currentMode == CameraZone.FollowMode.Fixed) return;

        Vector3 target = transform.position;

        if (currentMode == CameraZone.FollowMode.FollowX)
        {
            // Seguir al jugador en X, clampear dentro del collider
            float minX = currentZone.bounds.min.x;
            float maxX = currentZone.bounds.max.x;
            float clampedX = Mathf.Clamp(playerTransform.position.x, minX, maxX);
            target = new Vector3(clampedX, fixedTarget.y, transform.position.z);
        }
        else if (currentMode == CameraZone.FollowMode.FollowY)
        {
            float minY = currentZone.bounds.min.y;
            float maxY = currentZone.bounds.max.y;
            float clampedY = Mathf.Clamp(playerTransform.position.y, minY, maxY);
            target = new Vector3(fixedTarget.x, clampedY, transform.position.z);
        }

        transform.position = Vector3.Lerp(transform.position, target,
            followSpeed * Time.deltaTime);
    }

    public void MoveToZone(Vector3 zoneCenter, CameraZone.FollowMode mode,
        float speed, Transform player, BoxCollider2D zone)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(zoneCenter, mode, speed, player, zone));
    }

    private IEnumerator TransitionRoutine(Vector3 destination,
        CameraZone.FollowMode mode, float speed, Transform player, BoxCollider2D zone)
    {
        isTransitioning = true;

        yield return StartCoroutine(fadeController.FadeOut());

        fixedTarget = new Vector3(destination.x, destination.y, transform.position.z);
        transform.position = fixedTarget;

        currentMode = mode;
        followSpeed = speed;
        playerTransform = player;
        currentZone = zone;

        yield return StartCoroutine(fadeController.FadeIn());

        isTransitioning = false;
    }

    public FadeController GetFadeController() => fadeController;
}
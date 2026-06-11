using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int currentKeys = 0;
    private int currentScore = 0;

    private Vector3 lastCheckpointPosition;
    private Vector3 lastCameraPosition;

    private Player player;

    [SerializeField] private IntegrityBar integrityBar;
    [SerializeField] private HUDManager hudManager;

    private void Awake()
    {
        Instance = this;
        player = FindFirstObjectByType<Player>();
    }

    // ── Checkpoint ─────────────────────────────────────────

    public void SetLastCheckpoint(Vector3 playerPos, Vector3 cameraPos)
    {
        lastCheckpointPosition = playerPos;
        lastCameraPosition = cameraPos;
    }

    public void OnPlayerDied()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return StartCoroutine(
            CameraController.Instance.GetFadeController().FadeOut());

        player.transform.position = lastCheckpointPosition;
        CameraController.Instance.transform.position = new Vector3(
            lastCameraPosition.x, lastCameraPosition.y,
            CameraController.Instance.transform.position.z);

        player.Respawn();

        yield return StartCoroutine(
            CameraController.Instance.GetFadeController().FadeIn());
    }

    // ── Keys ───────────────────────────────────────────────

    public void AddKey()
    {
        currentKeys++;
        hudManager?.UpdateKeys(currentKeys);
        Debug.Log($"Keys: {currentKeys}");
    }

    public int GetKeyCount() => currentKeys;

    public void UseKeys(int amount)
    {
        currentKeys = Mathf.Max(0, currentKeys - amount);
        hudManager?.UpdateKeys(currentKeys);
    }

    // ── Score ──────────────────────────────────────────────

    public void AddScore(int amount)
    {
        currentScore += amount;
        hudManager?.UpdateScore(currentScore);
    }

    // ── PowerUp UI ─────────────────────────────────────────

    public void ShowPowerUpUI(string name, float duration)
    {
        hudManager?.ShowPowerUp(name, duration);
    }

    public void HidePowerUpUI()
    {
        hudManager?.HidePowerUp();
    }
}
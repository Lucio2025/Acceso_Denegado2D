using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Keys")]
    [SerializeField] private TextMeshProUGUI keysText;
    [SerializeField] private Image keyIcon;

    [Header("PowerUp")]
    [SerializeField] private GameObject powerUpPanel;
    [SerializeField] private TextMeshProUGUI powerUpName;
    [SerializeField] private Image powerUpBar;

    private Coroutine powerUpCoroutine;

    private void Awake()
    {
        Instance = this;
        powerUpPanel?.SetActive(false);
        UpdateScore(0);
        UpdateKeys(0);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateKeys(int keys)
    {
        if (keysText != null)
            keysText.text = $"x{keys}";
    }

    public void ShowPowerUp(string name, float duration)
    {
        if (powerUpPanel == null) return;
        powerUpPanel.SetActive(true);
        powerUpName.text = name;

        if (powerUpCoroutine != null)
            StopCoroutine(powerUpCoroutine);
        powerUpCoroutine = StartCoroutine(PowerUpBarRoutine(duration));
    }

    public void HidePowerUp()
    {
        powerUpPanel?.SetActive(false);
        if (powerUpCoroutine != null)
            StopCoroutine(powerUpCoroutine);
    }

    private IEnumerator PowerUpBarRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (powerUpBar != null)
                powerUpBar.fillAmount = 1f - (elapsed / duration);
            yield return null;
        }
        HidePowerUp();
    }
}
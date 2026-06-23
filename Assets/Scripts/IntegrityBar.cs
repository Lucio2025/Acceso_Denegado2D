using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntegrityBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Marco de corrupción")]
    [SerializeField] private Image corruptionFrame;
    [SerializeField] private float corruptionThreshold = 0.5f;
    [SerializeField] private float waveSpeed = 1.2f;
    [SerializeField] private float waveMinAlpha = 0.3f;

    private float currentRatio = 1f;

    public void UpdateBar(float current, float max)
    {
        currentRatio = current / max;
        float ratio = currentRatio;

        fillImage.fillAmount = ratio;
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);

        int percent = Mathf.CeilToInt(ratio * 100);
        label.text = $"Integridad del objeto: {percent}%";
    }

    private void Update()
    {
        if (corruptionFrame == null) return;
        if (currentRatio >= corruptionThreshold)
        {
            SetFrameAlpha(0f);
            return;
        }

        float baseAlpha = Mathf.InverseLerp(corruptionThreshold, 0f, currentRatio);

        float wave = (Mathf.Sin(Time.time * waveSpeed) + 1f) * 0.5f; // 0 a 1
        float finalAlpha = Mathf.Lerp(baseAlpha * waveMinAlpha, baseAlpha, wave);

        SetFrameAlpha(finalAlpha);
    }

    private void SetFrameAlpha(float alpha)
    {
        Color c = corruptionFrame.color;
        c.a = alpha;
        corruptionFrame.color = c;
    }

}
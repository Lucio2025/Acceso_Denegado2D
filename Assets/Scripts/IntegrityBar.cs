using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntegrityBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI label;

    public void UpdateBar(float current, float max)
    {
        float ratio = current / max;
        fillImage.fillAmount = ratio;

        int percent = Mathf.CeilToInt(ratio * 100);
        label.text = $"Integridad del objeto: {percent}%";

        // Color: verde → amarillo → rojo
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }
}
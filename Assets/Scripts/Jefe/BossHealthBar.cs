using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance;

    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image barraVida;
    [SerializeField] private Image barraGhost;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Ghost bar")]
    [SerializeField] private float ghostDelay = 0.6f;
    [SerializeField] private float ghostSpeed = 0.4f;

    [Header("Fade de entrada")]
    [SerializeField] private float fadeDuration = 1.5f;

    private float targetGhost = 1f;
    private Coroutine ghostCoroutine;

    private void Awake()
    {
        Instance = this;
        if (canvasGroup == null)
            canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (barraGhost != null)
            barraGhost.fillAmount = Mathf.MoveTowards(
                barraGhost.fillAmount, targetGhost, ghostSpeed * Time.deltaTime);
    }

    public void Show()
    {
        panel.SetActive(true);
        barraVida.fillAmount = 1f;
        barraGhost.fillAmount = 1f;
        targetGhost = 1f;
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StartCoroutine(FadeOut());
    }

    public void UpdateHealth(float ratio)
    {
        barraVida.fillAmount = ratio;

        if (ghostCoroutine != null) StopCoroutine(ghostCoroutine);
        ghostCoroutine = StartCoroutine(GhostDelay(ratio));
    }

    private IEnumerator GhostDelay(float target)
    {
        yield return new WaitForSeconds(ghostDelay);
        targetGhost = target;
        ghostCoroutine = null;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.4f);
            yield return null;
        }
        panel.SetActive(false);
    }
}
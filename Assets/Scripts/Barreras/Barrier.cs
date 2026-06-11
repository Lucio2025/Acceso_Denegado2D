using UnityEngine;
using System.Collections;

public abstract class Barrier : MonoBehaviour
{
    [Header("Efecto de desaparición")]
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private int flashCount = 4;
    [SerializeField] private Color flashColor = new Color(1f, 0.8f, 0f, 1f);

    protected SpriteRenderer[] spriteRenderers;
    protected bool isOpen = false;

    protected virtual void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    protected void Open()
    {
        if (isOpen) return;
        isOpen = true;

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            SetColor(flashColor);
            yield return new WaitForSeconds(flashDuration);
            SetColor(Color.white);
            yield return new WaitForSeconds(flashDuration);
        }

        float elapsed = 0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void SetColor(Color color)
    {
        foreach (var sr in spriteRenderers)
            sr.color = color;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

}
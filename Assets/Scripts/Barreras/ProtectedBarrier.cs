using UnityEngine;
using System.Collections;

public class ProtectedBarrier : Barrier, IInteractable
{
    [Header("Acceso")]
    [SerializeField] private int keysRequired = 1;

    [Header("Indicador flotante")]
    [SerializeField] private KeyIndicator keyIndicator;
    [SerializeField] private Vector3 indicatorOffset = new Vector3(0f, 0.7f, 0f);

    private int keysInserted = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetupIndicator();
    }

    private void SetupIndicator()
    {
        if (keyIndicator == null) return;

        keyIndicator.transform.position =
            transform.position + indicatorOffset;

        keyIndicator.Initialize(keysRequired);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Interact();
    }

    public void Interact()
    {
        if (isOpen) return;

        int playerKeys = GameManager.Instance.GetKeyCount();

        if (playerKeys >= 1)
        {
            GameManager.Instance.UseKeys(1);
            keysInserted++;
            keyIndicator?.FillNext();

            if (keysInserted >= keysRequired)
            {
                StartCoroutine(OpenWithIndicator());
            }
        }
        else
        {
            StartCoroutine(DeniedFlash());
        }
    }

    private IEnumerator OpenWithIndicator()
    {
        yield return new WaitForSeconds(0.3f);

        if (keyIndicator != null)
            yield return StartCoroutine(keyIndicator.FadeOutAndDestroy());

        Open();
    }

    private IEnumerator DeniedFlash()
    {
        foreach (var sr in spriteRenderers)
            sr.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(0.15f);
        foreach (var sr in spriteRenderers)
            sr.color = Color.white;
    }
}
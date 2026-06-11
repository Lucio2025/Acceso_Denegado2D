using UnityEngine;
using System.Collections;

public class KeyIndicator : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite emptySlot;
    [SerializeField] private Sprite filledSlot;

    [Header("Animación flotante")]
    [SerializeField] private float floatAmplitude = 0.12f;
    [SerializeField] private float floatSpeed = 1.8f;

    [Header("Separación entre slots")]
    [SerializeField] private float slotSpacing = 0.35f;

    private SpriteRenderer[] slots;
    private int filledCount = 0;
    private int totalSlots = 0;
    private Vector3 basePosition;

    public void Initialize(int keyCount)
    {
        totalSlots = keyCount;
        slots = new SpriteRenderer[keyCount];
        basePosition = transform.position;

        float totalWidth = slotSpacing * (keyCount - 1);
        float startX = -totalWidth / 2f;

        for (int i = 0; i < keyCount; i++)
        {
            GameObject slot = new GameObject($"Slot_{i}");
            slot.transform.SetParent(transform);

            float xPos = startX + i * slotSpacing;
            slot.transform.localPosition = new Vector3(xPos, 0f, 0f);

            SpriteRenderer sr = slot.AddComponent<SpriteRenderer>();
            sr.sprite = emptySlot;
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 5;

            slots[i] = sr;
        }

        StartCoroutine(FloatRoutine());
    }

    public void FillNext()
    {
        if (filledCount >= totalSlots) return;
        slots[filledCount].sprite = filledSlot;
        filledCount++;
    }

    public IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            foreach (var sr in slots)
            {
                if (sr == null) continue;
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator FloatRoutine()
    {
        while (true)
        {
            float y = basePosition.y +
                Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(
                basePosition.x, y, basePosition.z);
            yield return null;
        }
    }
}
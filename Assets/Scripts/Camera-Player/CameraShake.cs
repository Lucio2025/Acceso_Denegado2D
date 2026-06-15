using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Temblor por daño")]
    [SerializeField] private float damageDuration = 0.15f;
    [SerializeField] private float damageMagnitude = 0.08f;

    [Header("Temblor por botón")]
    [SerializeField] private float buttonDuration = 0.08f;
    [SerializeField] private float buttonMagnitude = 0.05f;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShakeOnDamage() => Shake(damageDuration, damageMagnitude);
    public void ShakeOnButton() => Shake(buttonDuration, buttonMagnitude);

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        // Guardar posición ACTUAL justo antes de temblar
        Vector3 posicionAntesDeTemblar = transform.position;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = posicionAntesDeTemblar + new Vector3(x, y, 0f);
            yield return null;
        }

        // Restaurar la posición que tenía antes del temblor
        transform.position = posicionAntesDeTemblar;
        shakeCoroutine = null;
    }
}
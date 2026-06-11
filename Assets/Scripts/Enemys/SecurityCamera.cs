using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour
{
    [Header("Sprites por dirección")]
    [SerializeField] private Sprite spriteUp;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteRight;

    public enum CameraDirection { Up, Down, Left, Right }
    [SerializeField] private CameraDirection facingDirection = CameraDirection.Down;

    [Header("Cono de visión")]
    [SerializeField] private float rayLength = 5f;
    [Range(5f, 120f)]
    [SerializeField] private float coneAngle = 30f;
    [SerializeField] private int rayCount = 8;
    [SerializeField] private Vector2 originOffset = Vector2.zero;
    [SerializeField] private Color coneColor = new Color(1f, 0f, 0f, 0.35f);

    [Header("Daño")]
    [SerializeField] private float damagePerSecond = 80f;
    [SerializeField] private LayerMask blockingLayers;

    [Header("Parpadeo")]
    [SerializeField] private bool doesFlicker = false;
    [SerializeField] private float timeOn = 3f;
    [SerializeField] private float timeOff = 1f;
    [SerializeField] private int flickerCount = 3;
    [SerializeField] private float flickerSpeed = 0.08f;

    [Header("Botón vinculado")]
    [SerializeField] private Button linkedButton;

    // ── Referencias ────────────────────────────────────────
    private SpriteRenderer spriteRenderer;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private bool isConeActive = true;
    private bool isDisabled = false;
    private Vector2 facingDir;

    // ── Init ───────────────────────────────────────────────

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Buscar MeshFilter y MeshRenderer en el hijo ConeVisual
        Transform coneVisual = transform.Find("ConeVisual");
        if (coneVisual == null)
        {
            Debug.LogError("Falta el hijo 'ConeVisual' en " + gameObject.name);
            return;
        }

        meshFilter = coneVisual.GetComponent<MeshFilter>();
        meshRenderer = coneVisual.GetComponent<MeshRenderer>();

        // Material simple de color
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        meshRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

        ApplyDirection();
    }

    private void Start()
    {
        if (linkedButton != null)
            linkedButton.OnPressed += DisablePermanently;

        if (doesFlicker)
            StartCoroutine(FlickerRoutine());
    }

    // ── Update ─────────────────────────────────────────────

    private void Update()
    {
        if (isDisabled)
        {
            ClearMesh();
            return;
        }

        if (isConeActive)
            DrawCone();
        else
            ClearMesh();
    }

    // ── Dirección ──────────────────────────────────────────

    private void ApplyDirection()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        facingDir = facingDirection switch
        {
            CameraDirection.Up => Vector2.up,
            CameraDirection.Down => Vector2.down,
            CameraDirection.Left => Vector2.left,
            CameraDirection.Right => Vector2.right,
            _ => Vector2.down
        };

        spriteRenderer.sprite = facingDirection switch
        {
            CameraDirection.Up => spriteUp,
            CameraDirection.Down => spriteDown,
            CameraDirection.Left => spriteLeft,
            CameraDirection.Right => spriteRight,
            _ => spriteDown
        };
    }

    // ── Cono ───────────────────────────────────────────────

    private void DrawCone()
    {
        if (meshFilter == null) return;

        Vector2 origin2D = (Vector2)transform.position + originOffset;
        float halfAngle = coneAngle * 0.5f;
        float baseAngle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = (float)i / rayCount;
            float angle = baseAngle - halfAngle + t * coneAngle;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            RaycastHit2D hit = Physics2D.Raycast(origin2D, dir, rayLength, blockingLayers);
            float dist = hit.collider != null ? hit.distance : rayLength;

            Vector2 worldPoint = origin2D + dir * dist;

            vertices[i + 1] = transform.InverseTransformPoint(worldPoint);
        }

        for (int i = 0; i < rayCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = coneColor;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        CheckDamage(origin2D);
    }

    private void ClearMesh()
    {
        if (meshFilter != null && meshFilter.mesh != null)
            meshFilter.mesh.Clear();
    }

    // ── Daño ───────────────────────────────────────────────

    private void CheckDamage(Vector2 origin)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, rayLength);

        foreach (var col in hits)
        {
            if (!col.CompareTag("Player")) continue;

            Vector2 toPlayer = (Vector2)col.transform.position - origin;
            float angle = Vector2.Angle(facingDir, toPlayer);

            if (angle > coneAngle * 0.5f) continue;

            RaycastHit2D los = Physics2D.Raycast(
                origin, toPlayer.normalized, toPlayer.magnitude, blockingLayers);

            if (los.collider != null) continue;

            col.GetComponent<Player>()?.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // ── Parpadeo ───────────────────────────────────────────

    private IEnumerator FlickerRoutine()
    {
        while (!isDisabled)
        {
            yield return new WaitForSeconds(timeOn);
            if (isDisabled) yield break;

            SetConeVisible(false);
            yield return new WaitForSeconds(timeOff);
            if (isDisabled) yield break;

            for (int i = 0; i < flickerCount; i++)
            {
                SetConeVisible(true);
                yield return new WaitForSeconds(flickerSpeed);
                SetConeVisible(false);
                yield return new WaitForSeconds(flickerSpeed);
            }

            SetConeVisible(true);
        }
    }

    private void SetConeVisible(bool visible)
    {
        isConeActive = visible;
        spriteRenderer.color = visible
            ? Color.white
            : new Color(1f, 1f, 1f, 0.3f);
    }

    // ── Desactivación por botón ────────────────────────────

    private void DisablePermanently()
    {
        isDisabled = true;
        StopAllCoroutines();
        ClearMesh();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);
    }

    // ── Gizmos ─────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        ApplyDirection();
        Vector2 origin = (Vector2)transform.position + originOffset;
        float baseAngle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
        float half = coneAngle * 0.5f;

        Gizmos.color = Color.red;
        float radL = (baseAngle - half) * Mathf.Deg2Rad;
        Gizmos.DrawRay(origin, new Vector2(Mathf.Cos(radL), Mathf.Sin(radL)) * rayLength);
        float radR = (baseAngle + half) * Mathf.Deg2Rad;
        Gizmos.DrawRay(origin, new Vector2(Mathf.Cos(radR), Mathf.Sin(radR)) * rayLength);

        Gizmos.color = new Color(1f, 0.3f, 0.3f);
        Gizmos.DrawRay(origin, facingDir * rayLength);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(origin, 0.05f);
    }
}
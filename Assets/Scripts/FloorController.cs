using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public sealed class FloorController : MonoBehaviour
{
    [Header("Random event timing (seconds)")]
    [SerializeField, Min(0.1f)] private float minimumWait = 8f;
    [SerializeField, Min(0.1f)] private float maximumWait = 15f;
    [SerializeField, Min(0.1f)] private float warningDuration = 3f;
    [SerializeField, Min(0.05f)] private float flashInterval = 0.25f;
    [SerializeField, Min(0.1f)] private float dangerDuration = 5f;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private Collider2D playerCollider;

    private enum Phase { Safe, Warning, Dangerous }
    private Phase phase;
    private float remaining;
    private float warningElapsed;
    private SpriteRenderer floorRenderer;
    private BoxCollider2D floorCollider;
    private Color normalColor;

    public bool IsDangerous { get { return phase == Phase.Dangerous; } }

    private void Awake()
    {
        floorRenderer = GetComponent<SpriteRenderer>();
        floorCollider = GetComponent<BoxCollider2D>();
        floorCollider.isTrigger = true;
        normalColor = floorRenderer.color;
        if (playerCollider == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerCollider = player.GetComponent<Collider2D>();
        }
    }

    private void OnEnable() { BeginSafePhase(); }

    private void BeginSafePhase()
    {
        phase = Phase.Safe;
        remaining = Random.Range(minimumWait, maximumWait);
        floorRenderer.color = normalColor;
    }

    private void Update()
    {
 
        if (Time.timeScale <= 0f) return;
        remaining -= Time.deltaTime;
        if (phase == Phase.Safe)
        {
            if (remaining <= 0f)
            {
                phase = Phase.Warning;
                remaining = warningDuration;
                warningElapsed = 0f;
                floorRenderer.color = dangerColor;
            }
        }
        else if (phase == Phase.Warning)
        {
            warningElapsed += Time.deltaTime;
            floorRenderer.color = Mathf.FloorToInt(warningElapsed / flashInterval) % 2 == 0
                ? dangerColor : normalColor;
            if (remaining <= 0f)
            {
                phase = Phase.Dangerous;
                remaining = dangerDuration;
                floorRenderer.color = dangerColor;
            }
        }
        else if (remaining <= 0f) BeginSafePhase();
    }

    private void LateUpdate()
    {
        if (!IsDangerous || Time.timeScale <= 0f || playerCollider == null ||
            !playerCollider.enabled || !playerCollider.gameObject.activeInHierarchy) return;

        Physics2D.SyncTransforms();
        ColliderDistance2D contact = floorCollider.Distance(playerCollider);
        if (contact.isValid && contact.distance <= 0.001f && GameManager.Instance != null)
            GameManager.Instance.EndGame();
    }

    private void OnDisable()
    {
        phase = Phase.Safe;
        if (floorRenderer != null) floorRenderer.color = normalColor;
    }

    private void OnValidate()
    {
        minimumWait = Mathf.Max(0.1f, minimumWait);
        maximumWait = Mathf.Max(minimumWait, maximumWait);
        warningDuration = Mathf.Max(0.1f, warningDuration);
        flashInterval = Mathf.Max(0.05f, flashInterval);
        dangerDuration = Mathf.Max(0.1f, dangerDuration);
    }
}
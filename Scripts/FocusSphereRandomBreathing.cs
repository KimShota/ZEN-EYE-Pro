using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class FocusSphereRandomBreathing : MonoBehaviour
{
    public Transform focusSphere;
    public XRGazeInteractor gazeInteractor;

    [Header("XR Camera")]
    public Transform xrCamera;

    public enum BreathingPhase
    {
        Phase1,  // 4th best
        Phase2,  // 5th best (lowest)
        Phase3,  // 3rd best
        Phase4,  // 2nd best
        Phase5   // Best (highest)
    }

    [Header("Phase Control (set manually before play)")]
    public BreathingPhase breathingPhase = BreathingPhase.Phase1;

    [Header("Breathing Settings")]
    public float minScale = 0.5f;
    public float maxScale = 1.6f;
    public float breathingNoiseSpeed = 1f;
    public float breathingJitterAmplitude = 0.3f;

    [Header("Scale Speed Settings")]
    public float normalScaleSpeed = 4f;
    public float gazeScaleSpeed = 1.5f;

    [Header("Gaze Influence Settings")]
    public float gazeShrinkBoost = 0.25f;
    public float gazeRaycastDistance = 10f;

    [Header("Motion Settings")]
    public float xRange = 0.8f;
    public float yRange = 0.3f;
    public float forwardOffset = 2f;

    public float minMoveSpeed = 0.5f;          // New: lower bound of move speed
    public float maxMoveSpeed = 2.0f;          // New: upper bound of move speed
    public float speedChangeInterval = 5f;     // New: interval to update move speed
    public float directionChangeInterval = 1f;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    private Vector3 currentTargetPosition;
    private float speedTimer = 0f;             // New: timer for speed changes
    private float directionTimer = 0f;
    private float moveSpeed;                   // New: dynamic move speed

    private float noiseSeed;
    private float jitterSeed;

    private bool wasGazedAtLastFrame = false;
    private float currentShrinkChance;

    void Start()
    {
        if (focusSphere == null || xrCamera == null)
        {
            Debug.LogError("Focus Sphere or XR Camera not assigned!");
            enabled = false;
            return;
        }

#if !UNITY_EDITOR
        if (gazeInteractor == null)
        {
            Debug.LogError("Gaze Interactor not assigned!");
            enabled = false;
            return;
        }
#endif

        noiseSeed = Random.Range(0f, 100f);
        jitterSeed = Random.Range(0f, 100f);

        SetNewTargetPosition();
        focusSphere.position = currentTargetPosition;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);  // Initialize random speed
    }

    void Update()
    {
        float t = Time.time;
        float baseShrinkChance = GetShrinkProbability();

        bool isBeingGazedAt = IsSphereBeingLookedAt();

        if (isBeingGazedAt && !wasGazedAtLastFrame)
        {
            currentShrinkChance = Mathf.Clamp01(baseShrinkChance + gazeShrinkBoost);
            Debug.Log($"[Gaze STARTED] Shrink Chance increased to: {currentShrinkChance:F2}");
        }
        else if (!isBeingGazedAt && wasGazedAtLastFrame)
        {
            currentShrinkChance = baseShrinkChance;
            Debug.Log($"[Gaze STOPPED] Shrink Chance returned to: {currentShrinkChance:F2}");
        }
        else
        {
            currentShrinkChance = isBeingGazedAt
                ? Mathf.Clamp01(baseShrinkChance + gazeShrinkBoost)
                : baseShrinkChance;
        }

        wasGazedAtLastFrame = isBeingGazedAt;

        // Breathing animation
        float baseNoise = Mathf.PerlinNoise(t * breathingNoiseSpeed, noiseSeed);
        float jitter = Mathf.PerlinNoise(t * breathingNoiseSpeed * 2f, jitterSeed) * breathingJitterAmplitude;
        float rawBreath = baseNoise + jitter;

        float adjustedBreath = Mathf.Lerp(1f, 0f, currentShrinkChance);
        float blend = Mathf.Clamp01(Mathf.Lerp(rawBreath, adjustedBreath, currentShrinkChance));
        float targetScale = Mathf.Lerp(minScale, maxScale, blend);

        float scaleSpeed = isBeingGazedAt ? gazeScaleSpeed : normalScaleSpeed;
        focusSphere.localScale = Vector3.Lerp(focusSphere.localScale, Vector3.one * targetScale, Time.deltaTime * scaleSpeed);

        // Direction logic
        directionTimer += Time.deltaTime;
        if (directionTimer >= directionChangeInterval)
        {
            SetNewTargetPosition();
            directionTimer = 0f;
        }

        // Speed update logic
        speedTimer += Time.deltaTime;
        if (speedTimer >= speedChangeInterval)
        {
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
            speedTimer = 0f;
        }

        // Move sphere
        focusSphere.position = Vector3.Lerp(focusSphere.position, currentTargetPosition, Time.deltaTime * moveSpeed);

        // Clamp vertical range
        float minY = xrCamera.position.y - 0.2f;
        float maxY = xrCamera.position.y + 0.6f;
        Vector3 clamped = focusSphere.position;
        clamped.y = Mathf.Clamp(clamped.y, minY, maxY);
        focusSphere.position = clamped;

        // Debug info
        if (debugText != null)
            debugText.text = $"Shrink Chance: {currentShrinkChance:F2}\nSpeed: {moveSpeed:F2}";
    }

    void SetNewTargetPosition()
    {
        float xOffset = Random.Range(-xRange, xRange);
        float yOffset = Random.Range(-yRange, yRange);

        Vector3 offset = (xrCamera.right * xOffset) + (xrCamera.up * yOffset) + (xrCamera.forward * forwardOffset);
        currentTargetPosition = xrCamera.position + offset;
    }

    float GetShrinkProbability()
    {
        switch (breathingPhase)
        {
            case BreathingPhase.Phase1: return 0.2f;
            case BreathingPhase.Phase2: return 0.1f;
            case BreathingPhase.Phase3: return 0.3f;
            case BreathingPhase.Phase4: return 0.4f;
            case BreathingPhase.Phase5: return 0.5f;
            default: return 0.4f;
        }
    }

    bool IsSphereBeingLookedAt()
    {
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        Ray ray = new Ray(gazeInteractor.rayOriginTransform.position, gazeInteractor.rayOriginTransform.forward);
#endif
        if (Physics.Raycast(ray, out RaycastHit hit, gazeRaycastDistance))
        {
            return hit.transform == focusSphere;
        }
        return false;
    }

    public void SetBreathingPhase(BreathingPhase phase)
    {
        breathingPhase = phase;
        Debug.Log("Breathing set to" + phase);
    }
}

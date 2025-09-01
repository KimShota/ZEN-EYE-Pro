using UnityEngine;
using Unity.XR.PXR;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EyeTrackingAccuracyTest : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetPrefab;
    public float targetDistance = 2.0f;
    public float targetOffset = 0.3f;
    public float dwellTime = 1.5f;
    public float warmupTime = 3.5f;   // ⏳ longer warm-up

    [Header("UI Settings")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;

    [Header("Auto Return Settings")]
    public string returnSceneName = "IntroScene"; // 🎯 set in Inspector
    public float returnDelay = 5f;                // ⏱ wait seconds

    private Transform hmd;
    private List<Vector3> targetLocalOffsets;
    private int currentTargetIndex = 0;
    private float timer = 0f;
    private float currentTargetTime;

    private List<float> errorSamples = new List<float>();
    private List<float> allTargetErrors = new List<float>();

    private GameObject currentTarget;

    void Start()
    {
        hmd = Camera.main.transform;
        resultText.gameObject.SetActive(false);

        targetLocalOffsets = new List<Vector3>
        {
            new Vector3(0, 0, targetDistance),             // warm-up (center)
            new Vector3(-targetOffset, 0, targetDistance), // left
            new Vector3(targetOffset, 0, targetDistance),  // right
            new Vector3(0, targetOffset, targetDistance),  // up
            new Vector3(0, -targetOffset, targetDistance)  // down
        };

        ShowNextTarget();
    }

    void Update()
    {
        if (currentTarget == null) return;

        timer += Time.deltaTime;

        if (PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 gazeDir))
        {
            Vector3 worldOrigin = hmd.position;
            Vector3 worldDir = hmd.TransformDirection(gazeDir);
            Vector3 targetWorldPos = hmd.TransformPoint(targetLocalOffsets[currentTargetIndex]);

            float angleError = Vector3.Angle(worldDir, (targetWorldPos - worldOrigin).normalized);

            if (timer <= currentTargetTime)
            {
                errorSamples.Add(angleError);
            }
            else
            {
                float avgError = 0;
                foreach (float e in errorSamples) avgError += e;
                avgError /= errorSamples.Count;

                if (currentTargetIndex > 0) // skip warm-up in results
                {
                    allTargetErrors.Add(avgError);
                    Debug.Log($"Target {currentTargetIndex} Avg Error: {avgError:F2}°");
                }
                else
                {
                    Debug.Log("Warm-up target skipped.");
                }

                errorSamples.Clear();
                currentTargetIndex++;

                if (currentTargetIndex < targetLocalOffsets.Count)
                {
                    ShowNextTarget();
                }
                else
                {
                    FinishTest();
                }
            }
        }
    }

    void ShowNextTarget()
    {
        if (currentTarget != null) Destroy(currentTarget);

        Vector3 pos = hmd.TransformPoint(targetLocalOffsets[currentTargetIndex]);
        currentTarget = Instantiate(targetPrefab, pos, Quaternion.identity);
        currentTarget.transform.LookAt(hmd);

        timer = 0f;
        errorSamples.Clear();

        if (currentTargetIndex == 0)
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = "👀 Look at the warm-up target!";
            currentTargetTime = warmupTime; // ⏳ longer for warm-up
        }
        else
        {
            instructionText.gameObject.SetActive(false);
            currentTargetTime = dwellTime;
        }
    }

    void FinishTest()
    {
        if (currentTarget != null) Destroy(currentTarget);

        float total = 0f;
        foreach (float e in allTargetErrors) total += e;
        float overallAvg = total / allTargetErrors.Count;

        instructionText.gameObject.SetActive(true);
        instructionText.text = "✅ Test Finished!";

        resultText.gameObject.SetActive(true);
        resultText.text = "";

        for (int i = 0; i < allTargetErrors.Count; i++)
        {
            resultText.text += $"Target {i + 1}: {allTargetErrors[i]:F2}°\n";
        }

        resultText.text += $"\nOverall Avg Error: {overallAvg:F2}°";

        // 🔹 Adjusted thresholds for real-world use
        if (overallAvg > 5f)
        {
            resultText.text += "\n⚠️ Strongly recommended: Recalibrate the eye tracker.";
        }
        else if (overallAvg > 3.5f)
        {
            resultText.text += "\nℹ️ Accuracy is fair, recalibration may help.";
        }
        else
        {
            resultText.text += "\n✅ Accuracy is good. No recalibration needed.";
        }

        Debug.Log($"Overall Average Error: {overallAvg:F2}°");

        // ⏳ Start return coroutine
        StartCoroutine(ReturnToScene());
    }

    IEnumerator ReturnToScene()
    {
        yield return new WaitForSeconds(returnDelay);
        if (!string.IsNullOrEmpty(returnSceneName))
        {
            SceneManager.LoadScene(returnSceneName);
        }
    }
}

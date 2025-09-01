using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FocusExperimentManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI countdownText;
    public Button startButton;
    public GameObject welcomePanel;

    [Header("Welcome Image Panel")]
    public GameObject welcomeImagePanel;

    [Header("Background Overlay")]
    public GameObject blackBackground;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] distractionClips;

    [Header("Round Settings")]
    public float roundDuration = 30f;

    [Header("Focus Visual (3D Sphere)")]
    public GameObject focusSphere;

    [Header("Visual Distractions")]
    public VisualDistractionManager visualDistractionManager;

    private int currentRound = 0;

    void Start()
    {
        startButton.onClick.AddListener(StartFirstSession);
        focusSphere.SetActive(false);

        // Show welcome image at start
        welcomePanel.SetActive(true);
        welcomeImagePanel.SetActive(true);
        blackBackground.SetActive(false);
        countdownText.text = "";
    }

    public void StartFirstSession()
    {
        welcomePanel.SetActive(false);
        StartCoroutine(BeginSessionRoutine());
    }

    IEnumerator BeginSessionRoutine()
    {
        yield return ShowCountdown();

        focusSphere.SetActive(true);
        visualDistractionManager.StartVisuals();
        yield return RunRound();
        visualDistractionManager.StopVisuals();
        focusSphere.SetActive(false);

        currentRound++;

        // Show interim message BEFORE Round 2
        countdownText.text = "準備ができたら、「スタート」ボタンを押してください。";
        welcomePanel.SetActive(true);
        blackBackground.SetActive(true);
        startButton.gameObject.SetActive(true);

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartSecondSession);
    }

    void StartSecondSession()
    {
        countdownText.text = ""; // Clear interim message
        welcomePanel.SetActive(false);
        StartCoroutine(SecondSessionRoutine());
    }

    IEnumerator SecondSessionRoutine()
    {
        yield return ShowCountdown();

        focusSphere.SetActive(true);
        visualDistractionManager.StartVisuals();
        yield return RunRound();
        visualDistractionManager.StopVisuals();
        focusSphere.SetActive(false);

        // Final message
        countdownText.text = "実験は終了しました。\nご参加いただきありがとうございました！";
        welcomePanel.SetActive(true);
        blackBackground.SetActive(true);
        startButton.gameObject.SetActive(false);
    }

    IEnumerator ShowCountdown()
    {
        blackBackground.SetActive(true);
        welcomePanel.SetActive(true);
        startButton.gameObject.SetActive(false);

        // Hide welcome image before countdown
        welcomeImagePanel.SetActive(false);

        countdownText.text = "セッションを開始します...";
        yield return new WaitForSeconds(1f);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "";
        welcomePanel.SetActive(false);
        blackBackground.SetActive(false);
    }

    IEnumerator RunRound()
    {
        float timer = 0f;
        while (timer < roundDuration)
        {
            PlayRandomDistraction();
            timer += 5f;
            yield return new WaitForSeconds(5f);
        }
    }

    void PlayRandomDistraction()
    {
        if (distractionClips.Length > 0)
        {
            audioSource.PlayOneShot(distractionClips[Random.Range(0, distractionClips.Length)]);
        }
    }
}

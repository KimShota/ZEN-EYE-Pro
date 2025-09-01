using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public enum RoundType
{
    Normal,
    Audio,
    Visual,
    Combined
}

public class TrialResult
{
    public string Timestamp;
    public RoundType Round;
    public int TrialNumber;
    public int UserAnswer;
    public int CorrectAnswer;
    public bool IsCorrect;
    public float ReactionTime;
}

public class AnswerManager : MonoBehaviour
{
    [Header("Managers & UI")]
    public FlashManager flashManager;
    public TMP_InputField answerInput;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI welcomeMessage;

    [Header("Session Panel UI")]
    public GameObject readyPanel;
    public TextMeshProUGUI sessionLabel;
    public Button startSessionButton;

    [Header("Language Selection")]
    public Button japaneseButton;
    public Button englishButton;
    private string currentLanguage = "English";

    public GameObject numPad;
    public float delayBeforeReload = 10f;

    private int currentTrial = 0;
    private int currentRoundIndex = 0;
    private RoundType currentRound = RoundType.Normal;

    private float reactionStartTime;
    private float lastReactionTime;

    private int audioFirstHalfCorrect = 0, audioSecondHalfCorrect = 0;
    private int visualFirstHalfCorrect = 0, visualSecondHalfCorrect = 0;
    private int normalCorrect = 0, combinedCorrect = 0;

    private List<TrialResult> allTrialResults = new List<TrialResult>();

    private Dictionary<RoundType, int> trialsPerRound = new Dictionary<RoundType, int>
    {
        {RoundType.Normal, 5},
        {RoundType.Audio, 6},
        {RoundType.Visual, 6},
        {RoundType.Combined, 6}
    };

    void Start()
    {
        startSessionButton.onClick.AddListener(() => StartRound(currentRoundIndex));
        japaneseButton.onClick.AddListener(SetLanguageToJapanese);
        englishButton.onClick.AddListener(SetLanguageToEnglish);

        welcomeMessage.gameObject.SetActive(true);
        japaneseButton.gameObject.SetActive(true);
        englishButton.gameObject.SetActive(true);

        instructionText.gameObject.SetActive(false);
        readyPanel.SetActive(false);
        answerInput.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        numPad.SetActive(false);

        SetLanguage("English"); 
    }

    public void SetLanguageToJapanese()
    {
        SetLanguage("Japanese");
        ProceedToReadyPanel();
    }

    public void SetLanguageToEnglish()
    {
        SetLanguage("English");
        ProceedToReadyPanel();
    }

    void SetLanguage(string language)
    {
        currentLanguage = language;
        flashManager.SetLanguage(language);

        welcomeMessage.text = language == "Japanese"
            ? "フラッシュ暗算へようこそ！\nこの実験では、5つの1桁の数字を加算します。\n数字はすぐに消えるので、1つ1つに注意してください！\n\n　　言語を選択してください。"
            : "Welcome to Flash Mental Arithmetic!\nIn this experiment, you will perform 1-digit addition for 5 digits.\nThey will disappear pretty quickly, so pay attention to each digit!\n\n    Please select your language.";
    }

    void ProceedToReadyPanel()
    {
        welcomeMessage.gameObject.SetActive(false);
        japaneseButton.gameObject.SetActive(false);
        englishButton.gameObject.SetActive(false);

        instructionText.gameObject.SetActive(false);
        answerInput.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        numPad.SetActive(false);

        currentRound = (RoundType)currentRoundIndex;
        sessionLabel.text = $"Round {currentRoundIndex + 1}: {currentRound}\nAre you ready?";
        readyPanel.SetActive(true);
        
        
    }

    public void StartRound(int roundIndex)
    {
        currentTrial = 0;
        currentRound = (RoundType)roundIndex;
        readyPanel.SetActive(true);
        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        for (int i = 3; i >= 1; i--)
        {
            sessionLabel.text = currentLanguage == "Japanese"
                ? $"開始まであと {i}..."
                : $"It will start in {i}...";
            yield return new WaitForSeconds(1f);
        }

        readyPanel.SetActive(false);
        StartNextTrial();
    }

    void StartNextTrial()
    {

        resultText.text = "";

        instructionText.text = currentLanguage == "Japanese"
            ? "次の5つの数字の合計を計算してください。"
            : "Please do addition for the following 5 digits.";

        instructionText.gameObject.SetActive(true);
        answerInput.gameObject.SetActive(true);
        resultText.gameObject.SetActive(true);
        numPad.SetActive(true);

        flashManager.StartFlashing(currentRound, currentTrial);
    }

    public void MarkReactionStart()
    {
        reactionStartTime = Time.time;
    }

    public void SubmitAnswer()
    {
        string userInput = answerInput.text;

        int userAnswer;
        if (!int.TryParse(userInput, out userAnswer))
        {
            resultText.text = currentLanguage == "Japanese" ? "有効な数字を入力してください。" : "Please enter a valid answer.";
            return;
        }

        List<int> digits = flashManager.GetGeneratedDigits();
        int correctAnswer = EvaluateResult(digits);

        bool isCorrect = userAnswer == correctAnswer;
        if (isCorrect)
        {
            if (currentRound == RoundType.Audio)
            {
                if (currentTrial < 3) audioFirstHalfCorrect++;
                else audioSecondHalfCorrect++;
            }
            else if (currentRound == RoundType.Visual)
            {
                if (currentTrial < 3) visualFirstHalfCorrect++;
                else visualSecondHalfCorrect++;
            }
            else if (currentRound == RoundType.Normal)
            {
                normalCorrect++;
            }
            else if (currentRound == RoundType.Combined)
            {
                combinedCorrect++;
            }

            resultText.text = currentLanguage == "Japanese"
                ? $"正解です！結果は {correctAnswer} です。"
                : $"Correct! The result is {correctAnswer}";
        }
        else
        {
            resultText.text = currentLanguage == "Japanese"
                ? $"不正解です。正解は {correctAnswer} でした。"
                : $"Incorrect. The correct result was {correctAnswer}";
        }

        lastReactionTime = Time.time - reactionStartTime;
        resultText.text += $"\nReaction Time: {lastReactionTime:F2} seconds";
        answerInput.text = "";

        TrialResult trial = new TrialResult
        {
            Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Round = currentRound,
            TrialNumber = currentTrial + 1,
            UserAnswer = userAnswer,
            CorrectAnswer = correctAnswer,
            IsCorrect = isCorrect,
            ReactionTime = lastReactionTime
        };

        allTrialResults.Add(trial);
        StartCoroutine(HandleNextTrial());
    }

    IEnumerator HandleNextTrial()
    {
        yield return new WaitForSeconds(3f);
        currentTrial++;

        if (currentTrial < trialsPerRound[currentRound])
        {
            StartNextTrial();
        }
        else
        {
            // Reset visuals after round 3
            if (currentRound == RoundType.Visual)
            {
                foreach (var vis in flashManager.abnormalVisuals)
                {
                    vis.SetActive(false);
                }
                flashManager.ClearShownAbnormalVisuals();
            }

            currentRoundIndex++;
            if (currentRoundIndex < System.Enum.GetNames(typeof(RoundType)).Length)
            {
                ProceedToReadyPanel();
            }
            else
            {
                DisplayFinalResults();
                answerInput.gameObject.SetActive(false);
                instructionText.gameObject.SetActive(false);
                flashManager.blackBackground.SetActive(true);
                flashManager.numPad.SetActive(false);
                               
            }
        }
    }

    private int EvaluateResult(List<int> numbers)
    {
        int result = numbers[0];
        for (int i = 1; i < numbers.Count; i++) result += numbers[i];
        return result;
    }

    private void DisplayFinalResults()
    {   
        string summary = currentLanguage == "Japanese"
            ? "\u2728 実験が終了しました！\n\n"
            : "\u2728 Experiment is done!\n\n";

        summary += $"Round 1 (Normal): {normalCorrect}/5\n";
        summary += $"Round 2 (Audio) - Env: {audioFirstHalfCorrect}/3 | Number: {audioSecondHalfCorrect}/3\n";
        summary += $"Round 3 (Visual) - Ambient: {visualFirstHalfCorrect}/3 | Abnormal: {visualSecondHalfCorrect}/3\n";
        summary += $"Round 4 (Combined): {combinedCorrect}/6\n";

        resultText.text = summary;
        saveFileToCSV();
        StartReload();
    }

    public void OnNumberButtonPressed(string number)
    {
        answerInput.text += number;
    }

    public void OnClearPressed()
    {
        answerInput.text = "";
    }

    public void OnSubmitPressed()
    {
        SubmitAnswer();
    }

    public void StartReload()
    {
        StartCoroutine(ReloadSceneWithDelay());
    }

    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeReload);
        //Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("IntroWM");
    }

    private void saveFileToCSV()
    {
        string filename = $"Experiment_Results_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string path = Path.Combine(Application.persistentDataPath, filename);

        List<string> lines = new List<string>
        {
            "Timestamp,Round,TrialNumber,UserAnswer,CorrectAnswer,IsCorrect,ReactionTime(s)"
        };

        foreach (var trial in allTrialResults)
        {
            lines.Add($"{trial.Timestamp},{trial.Round},{trial.TrialNumber},{trial.UserAnswer},{trial.CorrectAnswer},{trial.IsCorrect},{trial.ReactionTime:F2}");
        }

        File.WriteAllLines(path, lines);
        Debug.Log($"Detailed results saved to: {path}");
    }

 

}

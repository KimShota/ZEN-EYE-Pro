using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashManager : MonoBehaviour
{
    public TextMeshProUGUI flashText;
    public float flashDuration = 0.5f;
    public int digitCount = 5;

    public AudioSource audioSource;

    [Header("Language-based Digit Clips")]
    public AudioClip[] englishDigitClips;  
    public AudioClip[] japaneseDigitClips; 

    [Header("Distraction Clips")]
    public AudioClip[] environmentalClips;
    public AudioClip[] abnormalClips;
    public AudioClip[] waiterVoiceClips;

    public GameObject numPad;
    public GameObject blackBackground;
    public GameObject normalVisuals;
    public GameObject abnormalresto;
    public GameObject[] abnormalVisuals;
    public GameObject waiterObject;
    public GameObject ambientSound;
    public GameObject abnormalSounds;
    public GameObject catSound;

    private List<int> generatedDigits = new List<int>();
    private string currentLanguage = "English";
    private List<int> shownAbnormalVisualIndices = new List<int>();

    public void SetLanguage(string language)
    {
        currentLanguage = language;
    }

    public void StartFlashing(RoundType roundType, int trialNumber)
    {
        StartCoroutine(FlashDigits(roundType, trialNumber));
        numPad.SetActive(false);
        
    }

    IEnumerator FlashDigits(RoundType roundType, int trialNumber)
    {
        flashText.text = "";
        generatedDigits.Clear();

        if (roundType == RoundType.Audio && trialNumber == 3 && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        SetupDistractions(roundType, trialNumber);

        for (int i = 0; i < digitCount; i++)
        {
            int number = Random.Range(1, 10);
            generatedDigits.Add(number);
            flashText.text = number.ToString();

            if ((roundType == RoundType.Audio || roundType == RoundType.Combined) && audioSource)
            {
                if (roundType == RoundType.Audio && trialNumber < 3 && environmentalClips.Length > 0)
                {
                    audioSource.PlayOneShot(environmentalClips[Random.Range(0, environmentalClips.Length)]);
                }
                else if (roundType == RoundType.Audio && trialNumber >= 3)
                {
                    AudioClip[] digitSet = currentLanguage == "Japanese" ? japaneseDigitClips : englishDigitClips;
                    if (digitSet.Length > number && digitSet[number] != null)
                        audioSource.PlayOneShot(digitSet[number]);
                }
                else if (roundType == RoundType.Combined && waiterVoiceClips.Length > 0)
                {
                    audioSource.PlayOneShot(waiterVoiceClips[Random.Range(0, waiterVoiceClips.Length)]);
                }
            }

            yield return new WaitForSeconds(flashDuration);
            flashText.text = "";
            yield return new WaitForSeconds(0.2f);
        }

        numPad.SetActive(true);
        //flashText.text = "Enter Result";

        FindObjectOfType<AnswerManager>().MarkReactionStart();
       
    }

    void SetupDistractions(RoundType round, int trial)
    {
        if (round == RoundType.Normal)
        {
            foreach (var vis in abnormalVisuals)
                vis.SetActive(false);
            shownAbnormalVisualIndices.Clear();
        }

        normalVisuals.SetActive(false);
        abnormalresto.SetActive(false);
        waiterObject.SetActive(false);
        ambientSound.SetActive(false);
        abnormalSounds.SetActive(false);
        blackBackground.SetActive(true);

        if (round == RoundType.Visual)
        {
            blackBackground.SetActive(false);
            if (trial < 3)
            {
                normalVisuals.SetActive(true);
               
            }
            else
            {
                normalVisuals.SetActive(false);
                abnormalresto.SetActive(true);
                
                int visualIndex = trial - 3;
                if (!shownAbnormalVisualIndices.Contains(visualIndex) && visualIndex < abnormalVisuals.Length)
                {
                    shownAbnormalVisualIndices.Add(visualIndex);
                    abnormalVisuals[visualIndex].SetActive(true);
                }
                foreach (int index in shownAbnormalVisualIndices)
                {
                    if (index < abnormalVisuals.Length)
                        abnormalVisuals[index].SetActive(true);
                }
            }
            waiterObject.SetActive(true);
            
        }
        else if (round == RoundType.Combined)
        {
            abnormalresto.SetActive(true);
            ambientSound.SetActive(true);
            abnormalSounds.SetActive(true);
            waiterObject.SetActive(true);
            blackBackground.SetActive(false);
            catSound.SetActive(true);

            int visualIndex = trial;
            if (!shownAbnormalVisualIndices.Contains(visualIndex) && visualIndex < abnormalVisuals.Length)
            {
                shownAbnormalVisualIndices.Add(visualIndex);
                abnormalVisuals[visualIndex].SetActive(true);
            }
            foreach (int index in shownAbnormalVisualIndices)
            {
                if (index < abnormalVisuals.Length)
                    abnormalVisuals[index].SetActive(true);
            }
        }
    }

    public List<int> GetGeneratedDigits() => generatedDigits;

    public void ClearShownAbnormalVisuals()
    {
        shownAbnormalVisualIndices.Clear();
    }
}

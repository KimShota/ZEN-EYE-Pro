using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GazeTrackingController : MonoBehaviour
{
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject leftPanel;
    [SerializeField] GameObject rightPanel;
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject portraitLeftGrid;
    [SerializeField] GameObject portraitRightGrid;
    [SerializeField] GameObject landscapeLeftGrid;
    [SerializeField] GameObject landscapeRightGrid;

    private bool useMouseInEditor;
    bool isLeftBlinking = false;
    bool isRightBlinking = false;

    Vector3 origin, vector;
    float maxDistance = 50f;

    int maxFrame = 5;
    int frameId = 0;
    float timePerFrame = 15f;
    float timeRemaining;
    float descriptionTime = 8f;

    PointTime previousPoint = new();
    double previousChangeTime = 0;
    List<string> pointList = new();

    double positiveFixation = 0;
    double negativeFixation = 0;

    readonly string[] imagePrefix = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
    List<int> imageList;

    float scoreWaitTime = 5f;

    string currentImagePair = "";

    void Start()
    {
        
#if UNITY_EDITOR
        useMouseInEditor = true;
#else
        useMouseInEditor = false;
#endif
        
        previousPoint.point = Vector3.zero;
        previousPoint.epochTime = ToUnixDateTime(DateTime.Now);
        previousPoint.cellID = "None";
        previousChangeTime = previousPoint.epochTime;

        descriptionPanel.SetActive(true);
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        scorePanel.SetActive(false);

        timeRemaining = timePerFrame + descriptionTime;
        StartCoroutine(BeforeEnding());

        imageList = new(imagePrefix.Length);
        for (int i = 0; i < imagePrefix.Length; ++i) imageList.Add(i);
        Randomize();

        SwitchImages(frameId);
    }

    void Update()
    {   
        timeRemaining -= Time.deltaTime;

        if (frameId < maxFrame && timeRemaining <= timePerFrame)
        {
            descriptionPanel.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);
            
            GetGazeInfo();
            GetEyeBlink();

            if (timeRemaining <= 0)
            {
                frameId++;
                timeRemaining = timePerFrame;
                SwitchImages(frameId);
                Debug.Log(" Frame switched to: " + frameId);
            }
        }
    }
   

    void GetEyeBlink()
    {
        if (PXR_EyeTracking.GetLeftEyeGazeOpenness(out float leftOpenness) &&
            PXR_EyeTracking.GetRightEyeGazeOpenness(out float rightOpenness))
        {
            isLeftBlinking = leftOpenness < 0.2f;
            isRightBlinking = rightOpenness < 0.2f;

            if (isLeftBlinking && isRightBlinking)
            {
                Debug.Log("Blink detected");
            }
            else if (isLeftBlinking)
            {
                Debug.Log("Left eye blinking");
            }
            else if (isRightBlinking)
            {
                Debug.Log("Right eye blinking");
            }
        }
    }


    void GetGazeInfo()
    {
        Ray ray;
        Vector3 point = Vector3.zero;

        if (useMouseInEditor)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            try
            {
                var gazeInfo = EyeTrackingController.GetGazeData();
                origin = gazeInfo.origin;
                vector = gazeInfo.vector;

                Debug.Log($" Gaze Origin: {origin}, Gaze Vector: {vector}");

                if (vector == Vector3.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
                {
                    Debug.LogWarning(" Eye tracking data is invalid. Ray will not be cast");
                    return;
                }
                else
                {
                    ray = new Ray(origin, vector);
                }
            }
            catch (UnityException e)
            {
                Debug.LogWarning(" Failed to get gaze data: " + e);
                return;
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        double currentEpochTime = ToUnixDateTime(DateTime.Now);

        if (hits.Length > 0)
        {
            point = hits[0].point;

            if (previousPoint.point.x <= 0 && point.x > 0)
            {
                double delta = currentEpochTime - previousChangeTime;
                positiveFixation += delta;
                previousChangeTime = currentEpochTime;
                Debug.Log($" Positive fixation updated: +{delta:F2} ms (Total: {positiveFixation:F2} ms)");
            }
            else if (previousPoint.point.x >= 0 && point.x < 0)
            {
                double delta = currentEpochTime - previousChangeTime;
                negativeFixation += delta;
                previousChangeTime = currentEpochTime;
                Debug.Log($" Negative fixation updated: +{delta:F2} ms (Total: {negativeFixation:F2} ms)");
            }

            foreach (RaycastHit hitInfo in hits)
            {
                GameObject hitObj = hitInfo.collider.gameObject;
                if (hitObj.name.StartsWith("Grid_"))
                {
                    Debug.Log(" Gaze HIT Grid: " + hitObj.name);
                    AddPoint(point, frameId, currentEpochTime, hitObj.name);
                    return;
                }
            }

            AddPoint(point, frameId, currentEpochTime, "None");
        }
        else
        {
            Debug.Log(" Raycast hit nothing at all.");
            AddPoint(Vector3.zero, frameId, currentEpochTime, "None");
        }
    }

    void AddPoint(Vector3 point, int frameId, double currentEpochTime, string cellID)
    {

        {
            Dictionary<string, string> pointDict = new()
            {
                 { "\"frameId\"", frameId.ToString() },
                 { "\"x\"", point.x.ToString("F4") },
                 { "\"y\"", point.y.ToString("F4") },
                 { "\"z\"", point.z.ToString("F4") },
                 { "\"time\"", currentEpochTime.ToString() },
                 { "\"cellID\"", cellID },
                 { "\"imagePair\"", currentImagePair },
                 { "\"leftBlink\"", isLeftBlinking ? "1" : "0" },
                 { "\"rightBlink\"", isRightBlinking ? "1" : "0" }
            };

            // ✅ This is the line you were missing
            pointList.Add(MyDictionaryToJson(pointDict));

            // ✅ Update previous point tracking (this was already correct)
            previousPoint.point = point;
            previousPoint.epochTime = currentEpochTime;
            previousPoint.cellID = cellID;
        }
    }

    IEnumerator BeforeEnding()
    {
        yield return new WaitUntil(() => frameId == maxFrame);

        double score = Math.Round(negativeFixation / (positiveFixation + negativeFixation) * 100, 0);
        float finalScore = (float)score;

        Debug.Log($"📊 Final Fixation — Positive: {positiveFixation:F2} ms, Negative: {negativeFixation:F2} ms, Score: {score}%");

        StoreData(finalScore);

        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        scoreText.GetComponent<TextMeshPro>().text = score.ToString() + "%";
        scorePanel.SetActive(true);

        yield return new WaitForSeconds(scoreWaitTime);
        SceneManager.LoadScene("IntroWM");
    }

    void StoreData(float score)
    {
        string data = MyArrayToJson(pointList);
        WebAPIController.Handler("POST", "stress", score, data);
        WriteCSV(score);
    }

    void WriteCSV(float finalScore)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Application.persistentDataPath + $"/gaze_log_{timestamp}.csv";

        using StreamWriter writer = new(filePath);
        writer.WriteLine("frameId,x,y,z,time,cellID,imagePair,leftBlink,rightBlink,score");

        foreach (string line in pointList)
        {
            string cleaned = line.Replace("\"", "").Replace("{", "").Replace("}", "").Replace(":", "");
            writer.WriteLine(cleaned + "," + finalScore);
        }

        Debug.Log("CSV saved to: " + filePath);
    }

    void SwitchImages(int frame)
    {
        int randomFrame = imageList[frame];
        string name = imagePrefix[randomFrame];
        currentImagePair = name;

        SpriteRenderer leftSpriteRenderer = leftPanel.GetComponent<SpriteRenderer>();
        SpriteRenderer rightSpriteRenderer = rightPanel.GetComponent<SpriteRenderer>();

        Texture2D leftImage = Resources.Load<Texture2D>("StressChecker/ART_2023/" + name + "1");
        Texture2D rightImage = Resources.Load<Texture2D>("StressChecker/ART_2023/" + name + "2");

        leftSpriteRenderer.sprite = Sprite.Create(leftImage, new Rect(0, 0, leftImage.width, leftImage.height), new Vector2(0.5f, 0.5f), 50f);
        rightSpriteRenderer.sprite = Sprite.Create(rightImage, new Rect(0, 0, rightImage.width, rightImage.height), new Vector2(0.5f, 0.5f), 50f);

        portraitLeftGrid.SetActive(false);
        portraitRightGrid.SetActive(false);
        landscapeLeftGrid.SetActive(false);
        landscapeRightGrid.SetActive(false);

        switch (name)
        {
            case "A":
            case "F":
            case "H":
            case "I":
            case "L":
            case "N":
            case "O":
                landscapeLeftGrid.SetActive(true);
                landscapeRightGrid.SetActive(true);
                break;
            case "B":
            case "C":
            case "D":
            case "E":
            case "G":
            case "J":
            case "K":
            case "M":
                portraitLeftGrid.SetActive(true);
                portraitRightGrid.SetActive(true);
                break;
        }
    }

    void Randomize()
    {
        for (int i = 0; i < imageList.Count; ++i)
        {
            int j = UnityEngine.Random.Range(0, imageList.Count);
            (imageList[i], imageList[j]) = (imageList[j], imageList[i]);
        }
    }

    static string MyDictionaryToJson(IDictionary<string, string> dict)
    {
        return "{" + string.Join(",", dict.Select(d => $"{d.Key}:{d.Value}")) + "}";
    }

    static string MyArrayToJson(IList<string> list)
    {
        return "[" + string.Join(",", list) + "]";
    }

    public double ToUnixDateTime(DateTime dateTime)
    {
        return (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}

public class PointTime
{
    public Vector3 point;
    public double epochTime;
    public string cellID;
}

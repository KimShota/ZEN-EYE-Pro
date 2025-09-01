/*
using System;
using System.Collections;
using System.Collections.Generic;
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

    #region GazeData
    //GameObject sphere;
    //Vector3 vector;
    //Matrix4x4 matrix;
    Vector3 origin, vector;

    // Maximun distance to measure
    float maxDistance = 50;

    #endregion

    #region Session
    int maxFrame = 5;
    int frameId = 0;
    float timePerFrame = 15;
    float timeRemaining;
    float descriptionTime = 8.0f;
    bool done = false;
    #endregion

    #region PointData
    PointTime previousPoint = new();
    double previousChangeTime = 0;
    List<string> pointList = new();
    #endregion

    private double positiveFixation = 0;
    private double negativeFixation = 0;

    readonly string[] imagePrefix = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
    List<int> imageList;

    float scoreWaitTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        previousPoint.point = new(0, 0, 0);
        previousPoint.epochTime = ToUnixDateTime(DateTime.Now);
        previousChangeTime = previousPoint.epochTime;

        // Show description panel at the start
        descriptionPanel.SetActive(true);

        // Hide images panels at the start
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);

        // Hide score panel at the start
        scorePanel.SetActive(false);
        timeRemaining = timePerFrame + descriptionTime;
        StartCoroutine(BeforeEnding());

        //Randomize images to show
        imageList = new(imagePrefix.Length);
        for (int i = 0; i < imagePrefix.Length; ++i) imageList.Add(i);
        Randomize();

        // Load first image
        SwitchImages(frameId);

    }

    void Update()
    {
        // Decrease the remaining time of the current frame
        timeRemaining -= Time.deltaTime;

        // Do until last frame is reached
        if (frameId < maxFrame && timeRemaining <= timePerFrame)
        {

            // Show image panels
            descriptionPanel.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);

            // Measure gaze date
            GetGazeInfo();

            // Reset timer and switch to next fram when remaining time is equal or less than 0
            if (timeRemaining <= 0)
            {
                frameId += 1;
                timeRemaining = timePerFrame;
                SwitchImages(frameId);
                Debug.Log(frameId);
            }

        }
    }

    //void GenerateIndex()
    //{
    //    List<int> possible = Enumerable.Range(1, 48).ToList();
    //    List<int> listNumbers = new List<int>();
    //    for (int i = 0; i < 6; i++)

    //        int index = Random.Range(0, possible.Count);
    //        listNumbers.Add(possible[index]);
    //        possible.RemoveAt(index);
    //    }
    //}

    void Randomize()
    {
        for (int i = 0; i < imageList.Count; ++i)
        {
            int j = UnityEngine.Random.Range(0, imageList.Count);
            int element = imageList[j];
            imageList[j] = imageList[i];
            imageList[i] = element;
        }
    }

    /// Save data, show score and go back to intro scene
    IEnumerator BeforeEnding()
    {
        yield return new WaitUntil(() => frameId == maxFrame);

        double score = Math.Round(negativeFixation / (positiveFixation + negativeFixation) * 100, 0); // 

        StoreData((float)score);

        leftPanel.SetActive(false);
        rightPanel.SetActive(false);

        scoreText.GetComponent<TextMeshPro>().text = "" + score.ToString() + "%";
        scorePanel.SetActive(true);

        yield return new WaitForSeconds(scoreWaitTime);

        SceneManager.LoadScene("IntroWM");
    }

    /// This method loads images and displays them in the panels
    /// <param name="frame"> The current frame of the loop
    void SwitchImages(int frame)
    {
        int randomFrame = imageList[frame];
        string name = imagePrefix[randomFrame];

        SpriteRenderer leftSpriteRenderer = leftPanel.GetComponent<SpriteRenderer>();
        SpriteRenderer rightSpriteRenderer = rightPanel.GetComponent<SpriteRenderer>();

        Texture2D leftImage = Resources.Load<Texture2D>("StressChecker/ART_2023/" + name + "1");
        Texture2D rightImage = Resources.Load<Texture2D>("StressChecker/ART_2023/" + name + "2");

        Sprite leftSprite = Sprite.Create(leftImage, new Rect(0.0f, 0.0f, leftImage.width, leftImage.height), new Vector2(0.5f, 0.5f), 50.0f);
        Sprite rightSprite = Sprite.Create(rightImage, new Rect(0.0f, 0.0f, rightImage.width, rightImage.height), new Vector2(0.5f, 0.5f), 50.0f);

        leftSpriteRenderer.sprite = leftSprite;
        rightSpriteRenderer.sprite = rightSprite;
    }

    /// This method reads the gaze vectors from EyeTracking, calculates the point observed by the user and store the point in a point list.
    /// If the user gaze didn't point to the panels, a blank point is stored in the list.
    void GetGazeInfo()
    {

        try
        {
            var gazeInfo = EyeTrackingController.GetGazeData();
            origin = gazeInfo.origin;
            vector = gazeInfo.vector;
        }
        catch (UnityException e)
        {
            Debug.Log(e.ToString());
        }

        //// ?????v?Z
        RaycastHit hit;
        Vector3 point = new(0, 0, 0);

        double currentEpochTime = ToUnixDateTime(DateTime.Now);

        //if (true)
        if (Physics.Raycast(origin, vector, out hit, maxDistance))
        {
            //distance = hit.distance;
            point = hit.point;
            //point = new(1, 1, 1);

            // We use equal to zero because 0 is the default x value when looking at areas outside of target area
            if (previousPoint.point.x <= 0 && point.x > 0)
            {
                positiveFixation += (currentEpochTime - previousChangeTime);
                previousChangeTime = currentEpochTime;
            }

            if (previousPoint.point.x >= 0 && point.x < 0)
            {
                negativeFixation += (currentEpochTime - previousChangeTime);
                previousChangeTime = currentEpochTime;
            }

            if (previousPoint.point.x != point.x || previousPoint.point.y != point.y || previousPoint.point.z != point.z)
            {
                AddPoint(point, frameId, currentEpochTime);
            }
        }
        else
        {
            AddPoint(new Vector3(0, 0, 0), frameId, currentEpochTime);
        }

    }

    /// This method converts the input point and information to JSON format and add them to the point list.
    /// <param name="point"> The point to add
    /// <param name="frameId"> The current loop frame
    void AddPoint(Vector3 point, int frameId, double currentEpochTime)
    {
        Dictionary<string, string> pointDict = new()
        {
            {
                "\"frameId\"",
                frameId.ToString()
            },
            {
                "\"point\"",
                "{" + string.Format("\"x\": {0}, \"y\": {1}, \"z\": {2}", point.x, point.y, point.z) + "}"
            },
            {
                "\"time\"",
                currentEpochTime.ToString()
            }
        };
        pointList.Add(MyDictionaryToJson(pointDict));
        previousPoint.point = point;
        previousPoint.epochTime = currentEpochTime;
    }

    /// This method converts point list to JSON format and send it to the backend server through the API.
    void StoreData(float score)
    {

        string data = MyArrayToJson(pointList);
        WebAPIController.Handler("POST", "stress", score, data);
    }

    /// This method converts dictionary to JSON
    static string MyDictionaryToJson(IDictionary<string, string> dict)
    {
        var x = dict.Select(d =>
            string.Format("{0}: {1}", d.Key, string.Join(",", d.Value)));
        return "{" + string.Join(",", x) + "}";
    }

    /// This method converts array to JSON
    static string MyArrayToJson(IList<string> list)
    {
        var x = list.Select(d =>
            string.Format("{0}", d));
        return "[" + string.Join(",", x) + "]";
    }

    /// This method converts dateTime to epoch time
    public double ToUnixDateTime(DateTime dateTime)
    {

        DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (double)(dateTime - UnixTime).TotalMilliseconds;
    }
}

public class PointTime
{
    public Vector3 point;
    public double epochTime;
}
*/

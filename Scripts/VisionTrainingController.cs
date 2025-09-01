using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisionTrainingController : MonoBehaviour
{
    [SerializeField] AudioClip audioClip1;
    [SerializeField] GameObject sphere;
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject scoreFlashItem;
    [SerializeField] GameObject scoreFlashDetails;
    [SerializeField] GameObject scoreDynamicDetails;
    [SerializeField] GameObject countDownPanel;
    [SerializeField] GameObject countDownText;

    // Gaze data
    Vector3 origin, vector;

    // Maximun distance to measure
    float maxDistance = 100;

    float timePerLocation = 0.26f;
    float timeBetweenSphere = 1f;

    float timePerMovement = 2f;
    float timeBetweenMovement = 1f;

    float timeRemaining;

    float countDownTime = 4.0f;
    int locationId = -1;

    int maxLocations = 20;
    int dynamicHorizontalLocations = 5;
    int dynamicVerticalLocations = 5;
    int dynamicDepthLocations = 5;

    List<int> matchingList = new();

    float[] watchTime = { 0.0f, 0.0f, 0.0f, 0.0f };
    float newWatchTime = 0;
    float previousWatchTime = 0;

    float scoreWaitTime = 15f;

    int sessionId = 0;

    float directionX = 1;
    float directionY = 1;
    float directionZ = 1;

    void Start()
    {
        // Hide score panel at the start
        scorePanel.SetActive(false);

        // Hide the ball at the start
        sphere.GetComponent<SphereController>().SetSphereLocation(false);

        timeRemaining = timePerLocation + countDownTime;
        StartCoroutine(BeforeEnding());
    }

    // Update is called once per frame
    void Update()
    {
        // Do until maxLocations is reached
        if (sessionId == 0 && locationId < maxLocations)
        {
            FlashSession();
            DetectGazeCollision();
        }
        else if (sessionId == 0 && locationId == maxLocations)
        {
            sessionId = 1;
            locationId = -1;
            timeRemaining = timePerMovement + countDownTime;
            sphere.GetComponent<SphereController>().SetSphereLocation(false);
        }

        // Do until dynamicHorizontalLocations is reached
        if (sessionId == 1 && locationId < dynamicHorizontalLocations)
        {
            DynamicHorizontalSession();
            DetectGazeCollision();
        }
        else if (sessionId == 1 && locationId == dynamicHorizontalLocations)
        {
            sessionId = 2;
            locationId = -1;
            timeRemaining = timePerMovement + countDownTime;
            sphere.GetComponent<SphereController>().SetSphereLocation(false);

        }

        // Do until dynamicVerticalLocations is reached
        if (sessionId == 2 && locationId < dynamicVerticalLocations)
        {
            DynamicVerticalSession();
            DetectGazeCollision();
        }
        else if (sessionId == 2 && locationId == dynamicHorizontalLocations)
        {
            sessionId = 3;
            locationId = -1;
            timeRemaining = timePerMovement + countDownTime;
            sphere.GetComponent<SphereController>().SetSphereLocation(false);

        }

        // Do until dynamicCurveLocations is reached
        if (sessionId == 3 && locationId < dynamicDepthLocations)
        {
            DynamicDepthSession();
            DetectGazeCollision();
        }
        else if (sessionId == 3 && locationId == dynamicHorizontalLocations)
        {
            sessionId = 4;
        }

    }

    void FlashSession()
    {
        // Decrease the remaining time of the current frame
        timeRemaining -= Time.deltaTime;

        // Show countdown at the start
        CountDownController(timePerLocation);

        // Show the ball for the first time
        if (!countDownPanel.activeSelf && locationId == -1 && timeRemaining <= timePerLocation)
        {
            locationId += 1;
            sphere.GetComponent<SphereController>().SetSphereLocation(true);
        }

        // Hide the sphere when display time is over
        if (timeRemaining <= 0)
        {
            sphere.GetComponent<SphereController>().SetSphereLocation(false);
        }

        // Reset the display time and show the sphere again
        if (timeRemaining <= -timeBetweenSphere)
        {
            locationId += 1;
            // Reduce speed each time
            timeRemaining = timePerLocation - (locationId * 0.005f);
            // Set the sphere color to grey
            // sphere.GetComponent<Renderer>().material.color = new Color(0.21f, 0.21f, 0.21f, 1);

            if (locationId < maxLocations)
            {
                sphere.GetComponent<SphereController>().SetSphereLocation(true);
            }
        }
    }

    void DynamicHorizontalSession()
    {
        // Decrease the remaining time of the current frame
        timeRemaining -= Time.deltaTime;

        // Show countdown at the start
        CountDownController(timePerMovement);

        // Show the ball for the first time
        if (!countDownPanel.activeSelf && locationId == -1 && timeRemaining <= timePerMovement)
        {
            locationId += 1;
            sphere.GetComponent<SphereController>().SetSphereLocation(true);
            directionX = GetDirection(sphere.GetComponent<SphereController>().transform.position.x, SphereController.initialX);
        }

        // Hide the sphere when display time is over. If not, move the sphere
        if (timeRemaining <= 0)
        {
            sphere.GetComponent<SphereController>().SetSphereLocation(false);
        }
        else if (!countDownPanel.activeSelf)
        {
            sphere.GetComponent<SphereController>().MoveHorizontal(directionX, 1.0f);
        }

        // Reset the display time and show the sphere again
        if (timeRemaining <= -timeBetweenMovement)
        {
            locationId += 1;
            timeRemaining = timePerMovement;
            if (locationId < dynamicHorizontalLocations)
            {
                sphere.GetComponent<SphereController>().SetSphereLocation(true);
                directionX = GetDirection(sphere.GetComponent<SphereController>().transform.position.x, SphereController.initialX);
            }
        }
    }

    void DynamicVerticalSession()
    {
        // Decrease the remaining time of the current frame
        timeRemaining -= Time.deltaTime;

        // Show countdown at the start
        CountDownController(timePerMovement);

        // Show the ball for the first time
        if (!countDownPanel.activeSelf && locationId == -1 && timeRemaining <= timePerMovement)
        {
            locationId += 1;
            sphere.GetComponent<SphereController>().SetSphereLocation(true);
            directionY = GetDirection(sphere.GetComponent<SphereController>().transform.position.y, SphereController.initialY);
        }

        // Hide the sphere when display time is over. If not, move the sphere
        if (timeRemaining <= 0)
        {
            sphere.GetComponent<SphereController>().SetSphereLocation(false);
        }
        else if (!countDownPanel.activeSelf)
        {
            sphere.GetComponent<SphereController>().MoveVertical(directionY, 1.0f);
        }

        // Reset the display time and show the sphere again
        if (timeRemaining <= -timeBetweenMovement)
        {
            locationId += 1;
            timeRemaining = timePerMovement;
            if (locationId < dynamicVerticalLocations)
            {
                sphere.GetComponent<SphereController>().SetSphereLocation(true);
                directionY = GetDirection(sphere.GetComponent<SphereController>().transform.position.y, SphereController.initialY);
            }
        }
    }

    void DynamicDepthSession()
    {
        // Decrease the remaining time of the current frame
        timeRemaining -= Time.deltaTime;

        // Show countdown at the start
        CountDownController(timePerMovement);

        // Show the ball for the first time
        if (!countDownPanel.activeSelf && locationId == -1 && timeRemaining <= timePerMovement)
        {
            locationId += 1;
            sphere.GetComponent<SphereController>().SetSphereLocation(true);
            directionZ = GetDirection(sphere.GetComponent<SphereController>().transform.position.z, SphereController.initialZ);
            directionY = GetDirection(sphere.GetComponent<SphereController>().transform.position.y, SphereController.initialY);
            directionX = GetDirection(sphere.GetComponent<SphereController>().transform.position.x, SphereController.initialX);
        }

        // Hide the sphere when display time is over. If not, move the sphere
        if (timeRemaining <= 0)
        {
            sphere.GetComponent<SphereController>().SetSphereLocation(false);
        }
        else if (!countDownPanel.activeSelf)
        {
            sphere.GetComponent<SphereController>().MoveDepth(directionZ, 1.0f);
            sphere.GetComponent<SphereController>().MoveHorizontal(directionX, UnityEngine.Random.Range(0.125f, 0.25f));
            sphere.GetComponent<SphereController>().MoveVertical(directionY, UnityEngine.Random.Range(0.125f, 0.25f));
        }

        // Reset the display time and show the sphere again
        if (timeRemaining <= -timeBetweenMovement)
        {
            locationId += 1;
            timeRemaining = timePerMovement;
            if (locationId < dynamicDepthLocations)
            {
                sphere.GetComponent<SphereController>().SetSphereLocation(true);
                directionZ = GetDirection(sphere.GetComponent<SphereController>().transform.position.z, SphereController.initialZ);
                directionY = GetDirection(sphere.GetComponent<SphereController>().transform.position.y, SphereController.initialY);
                directionX = GetDirection(sphere.GetComponent<SphereController>().transform.position.x, SphereController.initialX);
            }
        }
    }

    float GetDirection(float currentPosition, float initialPosition)
    {
        return (currentPosition - initialPosition) / Math.Abs(currentPosition - initialPosition);
    }

    void CountDownController(float offset)
    {
        if (locationId == -1 && (timeRemaining - offset) > 1.0f && (timeRemaining - offset) < 4.0f)
        {
            countDownPanel.SetActive(true);
            countDownText.GetComponent<TextMeshPro>().text = ((int)(timeRemaining - offset)).ToString();
        }
        else
        {
            countDownPanel.SetActive(false);
        }
    }

    /// Save data, show score and go back to intro scene
    IEnumerator BeforeEnding()
    {
        yield return new WaitUntil(() => sessionId == 4);

        sphere.SetActive(false);

        float staticScoreValue = Convert.ToSingle(Math.Round(matchingList.Count / (float)maxLocations * 100, 0));
        float horizontalScoreValue = Convert.ToSingle(Math.Round(watchTime[1] / (float)(dynamicHorizontalLocations * timePerMovement) * 100, 0));
        float verticalScoreValue = Convert.ToSingle(Math.Round(watchTime[2] / (float)(dynamicVerticalLocations * timePerMovement) * 100, 0));
        float depthScoreValue = Convert.ToSingle(Math.Round(watchTime[3] / (float)(dynamicDepthLocations * timePerMovement) * 100, 0));


        Dictionary<string, string> scoreValues = new()
        {
            {
                "\"static\"",
                staticScoreValue.ToString()
            },
            {
                "\"horizontal\"",
                horizontalScoreValue.ToString()
            },
            {
                "\"vertical\"",
                verticalScoreValue.ToString()
            },
            {
                "\"depth\"",
                depthScoreValue.ToString()
            }
        };

        float averageScore = (staticScoreValue + horizontalScoreValue + verticalScoreValue + depthScoreValue) / 4;

        StoreData(averageScore, MyDictionaryToJson(scoreValues));

        string horizontalScore = "Horizontal: " + horizontalScoreValue.ToString() + "%";
        string verticalScore = "Vertical: " + verticalScoreValue.ToString() + "%";
        string depthScore = "Depth: " + depthScoreValue.ToString() + "%";
        string flashScore = "Flash: " + staticScoreValue.ToString() + "%";
        string level0Score = "0.26sec Level: " + getLevelScore(0, 1).ToString() + "%";
        string level1Score = "0.25sec Level: " + getLevelScore(2, 3).ToString() + "%";
        string level2Score = "0.24sec Level: " + getLevelScore(4, 5).ToString() + "%";
        string level3Score = "0.23sec Level: " + getLevelScore(6, 7).ToString() + "%";
        string level4Score = "0.22sec Level: " + getLevelScore(8, 9).ToString() + "%";
        string level5Score = "0.21sec Level: " + getLevelScore(10, 11).ToString() + "%";
        string level6Score = "0.20sec Level: " + getLevelScore(12, 13).ToString() + "%";
        string level7Score = "0.19sec Level: " + getLevelScore(14, 15).ToString() + "%";
        string level8Score = "0.18sec Level: " + getLevelScore(16, 17).ToString() + "%";
        string level9Score = "0.17sec Level: " + getLevelScore(18, 19).ToString() + "%";

        // scoreText.GetComponent<TextMeshPro>().text = "Score: " + score.ToString() + "%";

        scoreFlashItem.GetComponent<TextMeshPro>().text = flashScore;

        scoreFlashDetails.GetComponent<TextMeshPro>().text =
            level0Score + "\n" + 
            level1Score + "\n" + 
            level2Score + "\n" + 
            level3Score + "\n" + 
            level4Score + "\n" + 
            level5Score + "\n" + 
            level6Score + "\n" + 
            level7Score + "\n" + 
            level8Score + "\n" + 
            level9Score + "\n";

        scoreDynamicDetails.GetComponent<TextMeshPro>().text =
            horizontalScore + "\n\n" +
            verticalScore + "\n\n" +
            depthScore;

        scorePanel.SetActive(true);

        yield return new WaitForSeconds(scoreWaitTime);

        SceneManager.LoadScene("Intro1");
    }

    float getLevelScore(int a, int b)
    {
        int count = (matchingList.Contains(a) ? 1 : 0) + (matchingList.Contains(b) ? 1 : 0);
        return Convert.ToSingle(Math.Round(count / (float)2.0 * 100, 0));
    }

    void DetectGazeCollision()
    {
        newWatchTime += Time.deltaTime;

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

        // ‹——£ŒvŽZ
        RaycastHit hit;
        //Vector3 collisionPoint = new(0, 0, 0);

        if (Physics.SphereCast(origin, 1 , vector, out hit, maxDistance))
        {
            if (!matchingList.Contains(locationId))
            {
                matchingList.Add(locationId);
            }

            // Accumulated time the eye gaze matches the sphere
            watchTime[sessionId] = watchTime[sessionId] + Time.deltaTime;

            // Reproduce the sound with a time span to a higher rate of 0.2 sec
            if (newWatchTime - previousWatchTime > 0.2f)
            {
                ReproduceSound();
                previousWatchTime = newWatchTime;
            }

            //tracker.transform.position = hit.point;
            //distance = hit.distance;
            // collisionPoint = hit.point;

            // Set the sphere color to pink
            //sphere.GetComponent<Renderer>().material.color = new Color(02156f, 0.635f, 0.862f, 1);
        }
    }

    /// This method converts dictionary to JSON
    static string MyDictionaryToJson(IDictionary<string, string> dict)
    {
        var x = dict.Select(d =>
            string.Format("{0}: {1}", d.Key, string.Join(",", d.Value)));
        return "{" + string.Join(",", x) + "}";
    }


    void StoreData(float score, string rawData)
    {
        // TODO: Add logic to save data in DB
        WebAPIController.Handler("POST", "training", score, rawData);
    }

    void ReproduceSound()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play();
    }
}

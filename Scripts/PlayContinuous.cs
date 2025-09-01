using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayContinuous : MonoBehaviour
{
    [SerializeField]
    public VideoClip[] videoArray; // Video list of array
    private VideoPlayer videoPlayer;
    private int videoPlayed = 0;
    private bool triggerValue;  // Store the trigger button press state
   
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Attach the event handler for when the video finishes playing
        videoPlayer.loopPointReached += OnVideoEnd;

        PlayNextVideo();
    }

    // Update is called once per frame
    void Update()
    {
        //Waiting for controller input.
        GetControllerInfo();
    }
   
    // Called when the video finishes
    void OnVideoEnd(VideoPlayer vp)
    {
        if (videoPlayed % 2 == 0)
        {
            SceneManager.LoadScene("StressChecker");
            Debug.Log("Scene Switched");
        }
        else
        {
            // Call PlayNextVideo when the current video ends
            PlayNextVideo();
            Debug.Log("Playing Next Video");
        }
    }

    // Method to play the next video in the array
    void PlayNextVideo()
    {
        // Ensure we're within bounds of the video array
        if (videoArray.Length > 0)
        {
            // Pick a random video clip from the array and play it
            videoPlayer.clip = videoArray[Random.Range(0, videoArray.Length)];
            videoPlayer.Play();
            videoPlayed++;
            Debug.Log("Video player count" + videoPlayed);
        }
        else
        {
            Debug.LogWarning("Video array is empty!");
        }
    }

    void GetControllerInfo()
    {
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

        // Right hand device button presses
        if (rightHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = rightHandDevices[0];

            // Check if the primary or secondary button on the right controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                ResetControllerAndChangeScene();
            }
        }

        // Left hand device button presses
        if (leftHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = leftHandDevices[0];

            // Check if the primary or secondary button on the left controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                ResetControllerAndChangeScene();
            }
        }
    }

    // Method to reset the VideoController and change the scene to "Intro"
    private void ResetControllerAndChangeScene()
    {
       
        // Load the "Intro" scene
        SceneManager.LoadScene("Intro");
        Debug.Log("Trigger button pressed. VideoController reset and scene changed to Intro.");
    }
}
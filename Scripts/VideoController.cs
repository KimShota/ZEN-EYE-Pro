using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.XR.PXR;

public class VideoController : MonoBehaviour
{
    public VideoClip[] VideoClipArray;  // Array to hold your video clips
    private VideoPlayer videoPlayer;     // The VideoPlayer component
    private float timeUntilNextVideo;    // Time before switching to the next video
    private int videosPlayed = 0;        // Counter for how many videos have played
    private string originalSceneName;    // Store the name of the original scene for later return
    private bool isSceneChanging = false;  // Flag to ensure scene is only changed once per cycle
    private bool triggerValue;  // Store the trigger button press state

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();  // Get the VideoPlayer component
        videoPlayer.Pause();  // Ensure the video is paused initially

        // Save the original scene name
        originalSceneName = SceneManager.GetActiveScene().name;

        // Start the video playing the first time
        PlayNextVideo();
    }

    // Update is called once per frame
    void Update()
    {
        // If enough time has passed (after a video finishes playing), and we're not in the middle of changing scenes
        if (Time.time > timeUntilNextVideo && !isSceneChanging)
        {
            // If two videos have been played, start the scene change process
            if (videosPlayed % 2 == 0 && videosPlayed > 0)
            {
                StartCoroutine(SwitchScene());
            }
            else
            {
                // Otherwise, play the next video in sequence
                PlayNextVideo();
            }
        }

        // Check for controller button press and handle scene change/reset
        GetControllerInfo();
    }

    // Method to play the next video
    private void PlayNextVideo()
    {
        // Randomly select a video clip from the array
        videoPlayer.clip = VideoClipArray[Random.Range(0, VideoClipArray.Length)];

        // Update the time until the next video (current time + video length)
        timeUntilNextVideo = Time.time + (float)videoPlayer.clip.length;

        // Play the selected video
        videoPlayer.Play();

        // Increment the counter for videos played
        videosPlayed++;
    }

    // Coroutine to handle scene change
    private IEnumerator SwitchScene()
    {
        isSceneChanging = true;  // Flag to indicate we're changing scenes

        // Wait until the current video has finished before switching scenes
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Load the next scene (you can replace "NextScene" with your actual target scene name)
        SceneManager.LoadScene("StressChecker");

        // Wait a short moment to allow the scene transition to complete (optional)
        yield return new WaitForSeconds(1f);

        // Reset scene-changing flag
        isSceneChanging = false;

        // Reset the video counter so we can play the next two videos in the next cycle
        videosPlayed = 0;

        // Start the next two videos in the loop
        PlayNextVideo();
    }

    // Method to reset everything in the VideoController
    public void ResetController()
    {
        // Reset the video player state
        videoPlayer.Pause();  // Pause the video
        videoPlayer.clip = null;  // Clear the current video clip
        timeUntilNextVideo = 0;  // Reset the time counter
        videosPlayed = 0;  // Reset the video play counter
    }

    // Method to get the controller info and check for trigger button press
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
        // Reset the video controller
        ResetController();

        // Load the "Intro" scene
        SceneManager.LoadScene("Intro");
        Debug.Log("Trigger button pressed. VideoController reset and scene changed to Intro.");
    }
}
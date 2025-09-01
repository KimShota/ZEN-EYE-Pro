using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using TMPro;
using UnityEngine.SceneManagement;

public class HandTrackingController : MonoBehaviour
{
    //[SerializeField] TextMeshPro leftDisplay, rightDisplay;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] Vector3 initialRightPosition;
    [SerializeField] Vector3 initialLeftPosition;
    [SerializeField] Vector3 handSpeed;

    Vector3 initialPost;

    bool triggerValue;

    // Start is called before the first frame update
    void Start()
    {
        HandAimState aimState = new HandAimState();
        HandType handType = HandType.HandLeft;
        bool result = PXR_HandTracking.GetAimState(handType, ref aimState);

        Posef rayPose = aimState.aimRayPose;
        initialPost = new(rayPose.Position.x, rayPose.Position.y, rayPose.Position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Use controller buttons or Hand tracking
        GetControllerInfo();
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

            // Check if the primary button on the right controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out triggerValue) && triggerValue)
            {
                if (SceneManager.GetActiveScene().name != "3DVideo")
                {
                    SceneManager.LoadScene("BeeScene");
                    rightHand.transform.position = new Vector3(-8, 2, (float)-12.6);
                    Debug.Log("Right portal is pressed.");
                }
            }

            // Check if the secondary button on the right controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out triggerValue) && triggerValue)
            {
                if (SceneManager.GetActiveScene().name != "3DVideo")
                {
                    SceneManager.LoadScene("BeeScene");
                    leftHand.transform.position = new Vector3((float)-9.9, 2, (float)-12.6);
                    Debug.Log("Left portal is pressed.");
                }
            }
        }

        // Left hand device button presses
        if (leftHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = leftHandDevices[0];

            // Check if the primary button on the left controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out triggerValue) && triggerValue)
            {
                if (SceneManager.GetActiveScene().name != "3DVideo")
                {
                    SceneManager.LoadScene("BeeScene");
                    rightHand.transform.position = new Vector3(-8, 2, (float)-12.6);
                    Debug.Log("Right portal is pressed.");
                }
            }

            // Check if the secondary button on the left controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out triggerValue) && triggerValue)
            {
                if (SceneManager.GetActiveScene().name != "3DVideo")
                {
                    SceneManager.LoadScene("BeeScene");
                    leftHand.transform.position = new Vector3((float)-9.9, 2, (float)-12.6);
                    Debug.Log("Left portal is pressed.");
                }
            }
        }
    }
}
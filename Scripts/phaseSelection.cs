using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using static FocusSphereRandomBreathing;

public class phaseSelection : MonoBehaviour
{
    private bool triggerValue;  // Store the trigger button press state
    public FocusSphereRandomBreathing script;
    public FocusExperimentManager managerScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

            // Check if the primary or secondary button on the right controller is pressed
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                script.breathingPhase = BreathingPhase.Phase1;
                managerScript.StartFirstSession();

            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                script.breathingPhase = BreathingPhase.Phase2;
                managerScript.StartFirstSession();

            }
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                script.breathingPhase = BreathingPhase.Phase3;
                managerScript.StartFirstSession();

            }
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                script.breathingPhase = BreathingPhase.Phase4;
                managerScript.StartFirstSession();

            }
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out triggerValue) && triggerValue)
            {
                // Reset the VideoController and load "Intro" scene
                script.breathingPhase = BreathingPhase.Phase5;
                managerScript.StartFirstSession();

            }


        }

       
    }
}

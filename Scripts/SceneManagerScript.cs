using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ControllerSceneLoader : MonoBehaviour
{
    public enum ControllerButton
    {
        PrimaryButton,
        SecondaryButton,
        TriggerButton,
        GripButton,
        TriggerAnalog,   // float
        GripAnalog       // float
    }

    [System.Serializable]
    public class ButtonSceneMapping
    {
        public XRNode hand;             // LeftHand or RightHand
        public ControllerButton button; // enum dropdown
        public string sceneName;        // Scene to load
        [Range(0.1f, 1f)] public float analogThreshold = 0.5f; // for analog presses
    }

    [Header("Mappings")]
    public List<ButtonSceneMapping> mappings = new List<ButtonSceneMapping>();

    [Header("Audio")]
    [SerializeField] private AudioClip pressSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        foreach (var mapping in mappings)
        {
            var devices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(mapping.hand, devices);

            if (devices.Count == 0) continue;

            UnityEngine.XR.InputDevice device = devices[0];
            bool pressed = false;

            switch (mapping.button)
            {
                case ControllerButton.PrimaryButton:
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out pressed);
                    break;

                case ControllerButton.SecondaryButton:
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out pressed);
                    break;

                case ControllerButton.TriggerButton:
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out pressed);
                    break;

                case ControllerButton.GripButton:
                    device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out pressed);
                    break;

                case ControllerButton.TriggerAnalog:
                    if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float triggerValue))
                        pressed = triggerValue >= mapping.analogThreshold;
                    break;

                case ControllerButton.GripAnalog:
                    if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out float gripValue))
                        pressed = gripValue >= mapping.analogThreshold;
                    break;
            }

            if (pressed)
            {
                LoadScene(mapping.sceneName);
                return; // prevent double trigger
            }
        }
    }

    void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        if (pressSound && audioSource) audioSource.PlayOneShot(pressSound);
        SceneManager.LoadScene(sceneName);
    }
}
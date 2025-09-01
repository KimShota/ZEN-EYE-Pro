using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.XR;
using System.Collections.Generic;

public class EyeGazeVisualizer : MonoBehaviour
{
    [Header("Refs")]
    public GameObject gazeSphere;
    public Transform hmd;

    [Header("Raycast & Debug")]
    public float maxDistance = 50f;
    public LayerMask hittableLayers = ~0;
    public float followSmooth = 20f;

    [System.Serializable]
    public class ToggleBinding
    {
        public XRNode controller = XRNode.RightHand;
        public ToggleButton button = ToggleButton.Primary;
    }

    public enum ToggleButton { Primary, Secondary, TriggerButton, GripButton, StickPress }

    [Header("Toggle Settings")]
    public List<ToggleBinding> toggleBindings = new List<ToggleBinding>()
    {
        new ToggleBinding { controller = XRNode.LeftHand, button = ToggleButton.StickPress },
        new ToggleBinding { controller = XRNode.RightHand, button = ToggleButton.StickPress }
    };

    private readonly RaycastHit[] hits = new RaycastHit[8];
    private int ignoreMask;
    private bool visualEnabled = false;
    private Dictionary<ToggleBinding, bool> lastPressed = new Dictionary<ToggleBinding, bool>();

    void Awake()
    {
        if (hmd == null) hmd = Camera.main ? Camera.main.transform : null;

        if (gazeSphere)
        {
            var col = gazeSphere.GetComponent<Collider>();
            if (col) col.enabled = false;
            gazeSphere.transform.SetParent(null, true);
            gazeSphere.GetComponent<Renderer>().enabled = false;
        }

        if (gazeSphere) ignoreMask |= 1 << gazeSphere.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0) ignoreMask |= 1 << playerLayer;

        // initialize button state tracking
        foreach (var b in toggleBindings)
            lastPressed[b] = false;
    }

    void Update()
    {
        // 1. Check all bindings for toggle
        foreach (var binding in toggleBindings)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(binding.controller, devices);

            if (devices.Count > 0 && devices[0].TryGetFeatureValue(GetUsage(binding.button), out bool pressed))
            {
                if (pressed && !lastPressed[binding])
                {
                    visualEnabled = !visualEnabled;
                    if (gazeSphere)
                        gazeSphere.GetComponent<Renderer>().enabled = visualEnabled;
                }
                lastPressed[binding] = pressed;
            }
        }

        // 2. If disabled → skip gaze updates
        if (!visualEnabled || hmd == null || gazeSphere == null) return;

        if (!PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 gazeDir)) return;

        Vector3 origin = hmd.position;
        Vector3 dir = hmd.TransformDirection(gazeDir);

        int mask = hittableLayers & ~ignoreMask;
        int count = Physics.RaycastNonAlloc(origin, dir, hits, maxDistance, mask);

        RaycastHit? best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var tr = hits[i].transform;
            if (!tr || tr == gazeSphere.transform || tr.IsChildOf(hmd) || hmd.IsChildOf(tr))
                continue;

            if (hits[i].distance < bestDist)
            {
                best = hits[i];
                bestDist = hits[i].distance;
            }
        }

        if (best.HasValue)
        {
            Vector3 target = best.Value.point;
            gazeSphere.transform.position = Vector3.Lerp(
                gazeSphere.transform.position,
                target,
                1f - Mathf.Exp(-followSmooth * Time.deltaTime)
            );
        }
    }

    // Map enum → XR InputFeatureUsage
    private InputFeatureUsage<bool> GetUsage(ToggleButton tb)
    {
        return tb switch
        {
            ToggleButton.Primary => CommonUsages.primaryButton,
            ToggleButton.Secondary => CommonUsages.secondaryButton,
            ToggleButton.TriggerButton => CommonUsages.triggerButton,
            ToggleButton.GripButton => CommonUsages.gripButton,
            ToggleButton.StickPress => CommonUsages.primary2DAxisClick,
            _ => CommonUsages.primaryButton
        };
    }
}

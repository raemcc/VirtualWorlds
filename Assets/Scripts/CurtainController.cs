using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CurtainController : MonoBehaviour
{
    [Header("Slide Settings")]
    [Tooltip("How far along local Z the curtain slides when fully open. " +
             "Left curtain: negative value. Right curtain: positive value.")]
    public float openLocalZ = -0.8f;
    [Tooltip("Tick ON for the curtain that slides in the negative Z direction. Leave OFF for the other.")]
    public bool slidesNegative = false;

    [Header("Lighting")]
    public Light windowLight;
    public float closedLightIntensity = 0f;
    public float openLightIntensity = 100f;

    [Header("Bunching")]
    [Tooltip("How squished the curtain (and child rings) look when fully open. 0.4-0.6 works well.")]
    public float openScaleZ = 0.45f;

    // --- private state ---
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isGrabbed;
    private Transform controller;

    private float closedLocalZ;       // captured on Start
    private float closedScaleZ;       // captured on Start
    private float grabStartCtrlZ;     // controller Z when grab began
    private float grabStartCurtainZ;  // curtain Z when grab began
    private float targetZ;            // where we lerp to after release

    void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Stop XR from teleporting the curtain to the hand
        grab.trackPosition = false;
        grab.trackRotation = false;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        // Record resting state
        closedLocalZ = transform.localPosition.z;
        closedScaleZ = transform.localScale.z;
        targetZ      = closedLocalZ;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed           = true;
        controller          = args.interactorObject.transform;
        grabStartCtrlZ      = controller.position.z;
        grabStartCurtainZ   = transform.localPosition.z;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed  = false;
        controller = null;

        // Snap to fully open or fully closed based on which side of the midpoint we're on
        float mid        = (closedLocalZ + openLocalZ) / 2f;
        float current    = transform.localPosition.z;
        bool  shouldOpen = slidesNegative ? current < mid : current > mid;

        targetZ = shouldOpen ? openLocalZ : closedLocalZ;
    }

    void Update()
    {
        // --- Move curtain ---
        float currentZ = transform.localPosition.z;

        if (isGrabbed && controller != null)
        {
            // Follow the controller's Z delta, clamped to valid range
            float delta   = controller.position.z - grabStartCtrlZ;
            float desired = grabStartCurtainZ + delta;

            desired = slidesNegative
                ? Mathf.Clamp(desired, openLocalZ,   closedLocalZ)
                : Mathf.Clamp(desired, closedLocalZ, openLocalZ);

            transform.localPosition = new Vector3(transform.localPosition.x,
                                                  transform.localPosition.y,
                                                  desired);
        }
        else
        {
            // Lerp to snap target when released
            float snappedZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * 4f);
            transform.localPosition = new Vector3(transform.localPosition.x,
                                                  transform.localPosition.y,
                                                  snappedZ);
        }

        // --- Bunching scale (also squishes child rings together) ---
        float travel  = Mathf.Abs(openLocalZ - closedLocalZ);
        float t       = travel > 0f
                        ? Mathf.Clamp01(Mathf.Abs(transform.localPosition.z - closedLocalZ) / travel)
                        : 0f;
        float newScaleZ = Mathf.Lerp(closedScaleZ, openScaleZ, t);
        transform.localScale = new Vector3(transform.localScale.x,
                                           transform.localScale.y,
                                           newScaleZ);

        // --- Window light ---
        if (windowLight != null)
        {
            windowLight.intensity = Mathf.Lerp(closedLightIntensity,
                                               openLightIntensity,
                                               t);
        }
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CurtainController : MonoBehaviour
{
    [Header("Slide Settings")]
    [Tooltip("How far along local X the curtain slides when fully open. " +
             "Left curtain: negative value. Right curtain: positive value.")]
    public float openLocalX = -0.8f;
    [Tooltip("Tick ON for the left curtain (slides negative X). Leave OFF for the right curtain.")]
    public bool slidesNegative = false;

    [Header("Lighting")]
    public Light directionalLight;
    public float closedLightIntensity = 0.1f;
    public float openLightIntensity = 1.0f;

    [Header("Bunching")]
    [Tooltip("How squished the curtain (and child rings) look when fully open. 0.4-0.6 works well.")]
    public float openScaleX = 0.45f;

    // --- private state ---
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isGrabbed;
    private Transform controller;

    private float closedLocalX;       // captured on Start
    private float closedScaleX;       // captured on Start
    private float grabStartCtrlX;     // controller X when grab began
    private float grabStartCurtainX;  // curtain X when grab began
    private float targetX;            // where we lerp to after release

    void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Stop XR from teleporting the curtain to the hand
        grab.trackPosition = false;
        grab.trackRotation = false;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        // Record resting state
        closedLocalX  = transform.localPosition.x;
        closedScaleX  = transform.localScale.x;
        targetX       = closedLocalX;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed           = true;
        controller          = args.interactorObject.transform;
        grabStartCtrlX      = controller.position.x;
        grabStartCurtainX   = transform.localPosition.x;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed  = false;
        controller = null;

        // Snap to fully open or fully closed based on which side of the midpoint we're on
        float mid        = (closedLocalX + openLocalX) / 2f;
        float current    = transform.localPosition.x;
        bool  shouldOpen = slidesNegative ? current < mid : current > mid;

        targetX = shouldOpen ? openLocalX : closedLocalX;
    }

    void Update()
    {
        // --- Move curtain ---
        float currentX = transform.localPosition.x;

        if (isGrabbed && controller != null)
        {
            // Follow the controller's X delta, clamped to valid range
            float delta   = controller.position.x - grabStartCtrlX;
            float desired = grabStartCurtainX + delta;

            desired = slidesNegative
                ? Mathf.Clamp(desired, openLocalX,   closedLocalX)
                : Mathf.Clamp(desired, closedLocalX, openLocalX);

            transform.localPosition = new Vector3(desired,
                                                  transform.localPosition.y,
                                                  transform.localPosition.z);
        }
        else
        {
            // Lerp to snap target when released
            float snappedX = Mathf.Lerp(currentX, targetX, Time.deltaTime * 4f);
            transform.localPosition = new Vector3(snappedX,
                                                  transform.localPosition.y,
                                                  transform.localPosition.z);
        }

        // --- Bunching scale (also squishes child rings together) ---
        float travel    = Mathf.Abs(openLocalX - closedLocalX);
        float t         = travel > 0f
                          ? Mathf.Clamp01(Mathf.Abs(transform.localPosition.x - closedLocalX) / travel)
                          : 0f;
        float newScaleX = Mathf.Lerp(closedScaleX, openScaleX, t);
        transform.localScale = new Vector3(newScaleX,
                                           transform.localScale.y,
                                           transform.localScale.z);

        // --- Directional light ---
        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(closedLightIntensity,
                                                    openLightIntensity,
                                                    t);
        }
    }
}

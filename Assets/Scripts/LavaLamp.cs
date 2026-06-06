using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LavaLamp : MonoBehaviour
{
    [Header("Light")]
    public Light lampLight;
    public float targetIntensity = 0.03f;
    public float fadeDuration = 2f;

    [Header("Blob Animation")]
    public Animator blobAnimator; // drag the LavaLamp Animator here

    private bool isOn = false;
    private XRSimpleInteractable interactable;

    void Start()
    {
        lampLight.intensity = 0f;
        lampLight.enabled = false;

        if (blobAnimator != null)
            blobAnimator.enabled = false;

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        isOn = !isOn;
        StopAllCoroutines();

        if (blobAnimator != null)
            blobAnimator.enabled = isOn;

        GetComponent<WorldSpacePopup>()?.Toggle();

        if (isOn)
            StartCoroutine(FadeLight(0f, targetIntensity, true));
        else
            StartCoroutine(FadeLight(lampLight.intensity, 0f, false));
    }

    System.Collections.IEnumerator FadeLight(float from, float to, bool enableAtStart)
    {
        if (enableAtStart) lampLight.enabled = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            lampLight.intensity = Mathf.Lerp(from, to, t);
            yield return null;
        }
        lampLight.intensity = to;
        if (!enableAtStart) lampLight.enabled = false;
    }
}
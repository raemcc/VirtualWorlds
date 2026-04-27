using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LavaLamp : MonoBehaviour
{
    public Light lampLight;
    public float targetIntensity = 2f;
    public LavaBlobMover[] blobs;

    private bool isOn = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        lampLight.intensity = 0f;
        lampLight.enabled = false;
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        if (!isOn)
        {
            isOn = true;
            foreach (LavaBlobMover blob in blobs)
            {
                blob.StartMoving();
            }
            StartCoroutine(FadeLight());
        }
    }

    System.Collections.IEnumerator FadeLight()
    {
        lampLight.enabled = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f;
            lampLight.intensity = Mathf.Lerp(0f, targetIntensity, t);
            yield return null;
        }
    }
    
}
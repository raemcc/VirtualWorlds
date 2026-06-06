using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AlarmClock : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip beepClip;
    private AudioSource audioSource;

    [Header("Trigger Mode")]
    public bool beepOnHover = true;
    public bool beepOnSelect = false;
    public bool beepOnInterval = false;

    [Header("Interval Settings (if beepOnInterval)")]
    public float intervalSeconds = 5f;

    private XRSimpleInteractable interactable;
    private float timer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = beepClip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            if (beepOnHover)
                interactable.hoverEntered.AddListener(OnHover);
            if (beepOnSelect)
                interactable.selectEntered.AddListener(OnSelect);
        }
    }

    void Update()
    {
        if (beepOnInterval)
        {
            timer += Time.deltaTime;
            if (timer >= intervalSeconds)
            {
                Beep();
                timer = 0f;
            }
        }
    }

    void OnHover(HoverEnterEventArgs args) => Beep();
    void OnSelect(SelectEnterEventArgs args) => Beep();

    void Beep()
    {
        if (beepClip != null && !audioSource.isPlaying)
            audioSource.Play();
    }
}

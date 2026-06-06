using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Full CD player interaction.
/// - Grab CD and release in StereoZone → plays album, shows backstory popup
/// - TogglePause() → play/pause (wire to a button near stereo)
/// - NextTrack() / PreviousTrack() → skip tracks (wire to buttons)
/// - Eject() → stops music, hides popup, returns CD to start position
///
/// Setup:
///   - CD: XRGrabInteractable + Rigidbody (Is Kinematic) + Collider + this script
///   - Stereo: child "StereoZone" with BoxCollider (Is Trigger), tagged "StereoZone"
///   - Stereo: AudioSource (Play On Awake off) assigned to stereoAudioSource
///   - Drag all track clips into the Tracks array in order
///   - Backstory popup: WorldSpacePopup on the stereo, drag into backstoryPopup slot
///   - Hint popup: WorldSpacePopup on the CD, wire via XRGrabInteractable Hover Entered/Exited
///   - Play/Pause button: XRSimpleInteractable → Select Entered → CDPlayer.TogglePause()
///   - Next button: XRSimpleInteractable → Select Entered → CDPlayer.NextTrack()
///   - Prev button: XRSimpleInteractable → Select Entered → CDPlayer.PreviousTrack()
///   - Eject: Stereo XRSimpleInteractable → Select Entered → CDPlayer.Eject()
/// </summary>
public class CDPlayer : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("AudioSource on the stereo.")]
    public AudioSource stereoAudioSource;

    [Tooltip("Drag all album tracks here in order.")]
    public AudioClip[] tracks;

    [Header("Popups")]
    [Tooltip("WorldSpacePopup on the stereo — shows backstory when CD is inserted.")]
    public WorldSpacePopup backstoryPopup;

    [Tooltip("Parent GameObject containing the play/pause, next, prev buttons. Hidden until CD is inserted.")]
    public GameObject buttonsParent;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isInZone = false;
    private bool isLoaded = false;
    private int currentTrack = 0;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
            grab.selectExited.AddListener(OnReleased);
        else
            Debug.LogWarning("CDPlayer: No XRGrabInteractable found on " + gameObject.name);
    }

    void OnDestroy()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {
        // Auto-advance to next track when current one finishes
        if (isLoaded && !stereoAudioSource.isPlaying)
            NextTrack();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StereoZone"))
            isInZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StereoZone"))
            isInZone = false;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (isInZone)
        {
            gameObject.SetActive(false);
            currentTrack = 0;
            PlayCurrentTrack();
            isLoaded = true;
            backstoryPopup?.Show();
            if (buttonsParent != null) buttonsParent.SetActive(true);
        }
    }

    void PlayCurrentTrack()
    {
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogWarning("CDPlayer: No tracks assigned.");
            return;
        }
        stereoAudioSource.clip = tracks[currentTrack];
        stereoAudioSource.Play();
    }

    /// <summary>Toggle play/pause. Wire to a button near the stereo.</summary>
    public void TogglePause()
    {
        if (!isLoaded) return;

        if (stereoAudioSource.isPlaying)
            stereoAudioSource.Pause();
        else
            stereoAudioSource.UnPause();
    }

    /// <summary>Skip to next track.</summary>
    public void NextTrack()
    {
        if (!isLoaded || tracks == null || tracks.Length == 0) return;
        currentTrack = (currentTrack + 1) % tracks.Length;
        PlayCurrentTrack();
    }

    /// <summary>Go to previous track.</summary>
    public void PreviousTrack()
    {
        if (!isLoaded || tracks == null || tracks.Length == 0) return;
        currentTrack = (currentTrack - 1 + tracks.Length) % tracks.Length;
        PlayCurrentTrack();
    }

    /// <summary>Eject — stops music, hides popup, returns CD to shelf.</summary>
    public void Eject()
    {
        if (stereoAudioSource != null)
            stereoAudioSource.Stop();

        isLoaded = false;
        backstoryPopup?.Hide();
        if (buttonsParent != null) buttonsParent.SetActive(false);

        gameObject.SetActive(true);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}

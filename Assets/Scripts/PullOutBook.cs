using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class PullOutBook : MonoBehaviour
{
    [Header("Popup")]
    public WorldSpacePopup popup;

    [Header("Return Settings")]
    public float returnSpeed = 3f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private Rigidbody _rb;
    private Vector3 _homePosition;
    private Quaternion _homeRotation;
    private bool _returning = false;

    void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();
        _homePosition = transform.position;
        _homeRotation = transform.rotation;
    }

    void OnEnable()
    {
        _grab.selectEntered.AddListener(OnGrabbed);
        _grab.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        _grab.selectEntered.RemoveListener(OnGrabbed);
        _grab.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {
        if (!_returning) return;

        transform.position = Vector3.MoveTowards(transform.position, _homePosition, returnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _homeRotation, returnSpeed * 90f * Time.deltaTime);

        if (Vector3.Distance(transform.position, _homePosition) < 0.001f)
        {
            transform.position = _homePosition;
            transform.rotation = _homeRotation;
            _returning = false;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _returning = false;
        if (popup != null) popup.Show();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (popup != null) popup.Hide();
        if (_rb != null) _rb.isKinematic = true;
        _returning = true;
    }
}

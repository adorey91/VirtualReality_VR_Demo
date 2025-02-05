using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class JoyStickController : MonoBehaviour
{
    [Header("Automation")]
    [Tooltip("If true, rigid body and grab interactable settings will be changed OnStart via SetJoystick & SetKnob methods (Determined by Is Knob in knob parameters).")]
    [SerializeField] bool _autoSettings = true;

    [Space]
    [Tooltip("Optional: Use this to avoid using the output Delegate, receiver componenet, and groups. A unique scriptable object will need to be created for each joystick.\n\nNOTE: Can be used in addition to Delegate.")]
    [SerializeField] JoystickValueSO joystick;

    [Header("Joystick Parameters")]
    [Tooltip("How fast the pivot rotates.")]
    [SerializeField][Min(0f)] float _sensitivity = 4f;
    [Tooltip("The amount the pivot can rotate.")]
    [SerializeField][Min(0f)] float _maxPivotRadius = .04f;

    [Space]
    [Tooltip("If true, output will equal Vector3.zero if Grab distance from this position is less than Dead Zone Radius")]
    [SerializeField] bool _useDeadZone = true;
    [SerializeField][Min(0.0001f)] float _deadZoneRadius;

    [Header("References")]
    [Tooltip("Transform using XR Grab Interactable.")]
    [SerializeField] Transform _grab;
    [Tooltip("Rotation point (Rotates based on Grab local position).")]
    [SerializeField] Transform _pivot;

    [Space]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    bool _isActive;
    bool _isNullRef;
    bool _isDeadZone;

    Quaternion _initPivitRot;

    Rigidbody _grabRb;
    XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabRb = _grab.GetComponent<Rigidbody>();
        _grabInteractable = _grab.GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        NullReferenceCheck();
        Initialize();
    }

    private void Update()
    {
        if (_isNullRef) return;

        if (_isActive)
        {
            Clamp();
            DeadZoneCheck();
            Move();
        }
    }

    private void Initialize()
    {
        if (joystick != null) { joystick.Value = Vector3.zero; }

        _initPivitRot = _pivot.rotation;

        if (_autoSettings)
        {
            SetJoystick();
        }
    }

    public void SetJoystick()
    {
        _grabRb.constraints &= ~RigidbodyConstraints.FreezeAll;
        _grabRb.constraints = RigidbodyConstraints.FreezeRotation;

        if (_grabInteractable != null)
        {
            _grabInteractable.trackPosition = true;
            _grabInteractable.trackRotation = false;
        }
    }

    private void Clamp()
    {
        _grab.position = transform.position + Vector3.ClampMagnitude(_grab.position - transform.position, _maxPivotRadius);
    }

    public void DeadZoneCheck()
    {
        if (!_useDeadZone) return;

        float distance = Vector3.Distance(transform.position, _grab.position);

        _isDeadZone = distance <= _deadZoneRadius;
    }

    private void Move()
    {
        _pivot.localRotation = new(_grab.localPosition.z * _sensitivity, _grab.localPosition.y, -_grab.localPosition.x * _sensitivity, 1);

        ProcOutput();
    }

    private void NullReferenceCheck()
    {
        if (_grab == null || _pivot == null)
        {
            _isNullRef = true;
        }
    }

    public void Activate()
    {
        _grab.parent = transform;
        _grab.rotation = transform.rotation;
        _isActive = true;
        OnActivate.Invoke();
    }

    public void Deactivate()
    {
        _grab.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        ProcOutput();
        _pivot.rotation = _initPivitRot;
        _isActive = false;
        OnDeactivate.Invoke();
    }

    private void ProcOutput()
    {
        if (_isDeadZone)
        {
            if (joystick != null) { joystick.Value = Vector3.zero; }
        }
        else
        {
            if (joystick != null) { joystick.Value = _grab.localPosition; }
        }
    }
}

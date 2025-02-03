using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class JoyStickController : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    [Serializable] public class LeverEvent : UnityEvent<Vector2> { }

    [SerializeField] private Transform handle;
    [SerializeField] private Transform crane; // Reference to crane object
    [SerializeField] private float minDistance = 0.1f; // Minimum distance from boundaries
    public LayerMask collision; // Layer mask for collision detection
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float moveSpeed = 2f; // Speed of crane movement
    [SerializeField] private Vector2 minBounds = new Vector2(-5, -5);
    [SerializeField] private Vector2 maxBounds = new Vector2(5, 5);
    [SerializeField] private LeverEvent onLeverMove;
    [SerializeField] private Transform leverHandle;

    private IXRSelectInteractor interactor;
    private Quaternion initialRotation;
    private Vector2 currentInput;


    private Rigidbody craneRB;

    protected override void Awake()
    {
        base.Awake();
        initialRotation = handle.localRotation;
        craneRB = crane.GetComponent<Rigidbody>();
    }



    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        selectExited.AddListener(EndGrab);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        selectExited.RemoveListener(EndGrab);
        base.OnDisable();
    }

    private void StartGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
    }

    private void EndGrab(SelectExitEventArgs args)
    {
        interactor = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected && interactor != null)
            {
                UpdateLeverPosition();
            }
            else
            {
                ResetLever();
            }
        }
    }

    private void UpdateLeverPosition()
    {
        Vector3 localOffset = transform.InverseTransformPoint(interactor.GetAttachTransform(this).position);
        localOffset.y = 0; // Ignore vertical movement

        float xAngle = Mathf.Clamp(localOffset.x * maxAngle, -maxAngle, maxAngle);
        float zAngle = Mathf.Clamp(localOffset.z * maxAngle, -maxAngle, maxAngle);

        handle.localRotation = Quaternion.Euler(xAngle, 0, zAngle);
        currentInput = new Vector2(xAngle / maxAngle, zAngle / maxAngle);

        // Debug to check direction
        //Debug.Log("Lever moved: " + currentInput);

        onLeverMove.Invoke(currentInput);

        // Example crane movement logic (replace with actual crane movement code)
        MoveCrane();
    }

    private void ResetLever()
    {
        handle.localRotation = Quaternion.Lerp(handle.localRotation, initialRotation, Time.deltaTime * returnSpeed);
        currentInput = Vector2.Lerp(currentInput, Vector2.zero, Time.deltaTime * returnSpeed);

        // Debug to confirm reset
        //Debug.Log("Lever resetting to center");

        onLeverMove.Invoke(currentInput);
    }
    private void MoveCrane()
    {
        if (crane == null) return;

        // Calculate new position
        Vector3 newPosition = new Vector3(currentInput.x, 0, currentInput.y);

        if (!CanMove(newPosition)) return;

        craneRB.MovePosition(crane.position + newPosition * moveSpeed * Time.deltaTime);

        //Debug.Log("Crane position: " + crane.position);
    }

    private bool CanMove(Vector3 direction)
    {
        if (Physics.Raycast(crane.transform.position, direction, out _, minDistance, collision))
        {
            return false; // Can't move if an obstacle is within minDistance
        }
        return true;
    }
}
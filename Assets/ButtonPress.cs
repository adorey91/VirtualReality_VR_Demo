using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonPress : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    public class ButtonEvent : UnityEngine.Events.UnityEvent<bool> { }

    [SerializeField] private Transform button;
    [SerializeField] private Transform crane;

    private IXRActivateInteractor interactor;

    private Vector3 initialButtonPosition;
    private Vector3 initialCranePosition;

    private bool isPressed = false;
    private bool hasCraneMoved = false;

    private float buttonPressDistance = 0.01f; // How far the button should move
    private float craneMoveDistance = 0.5f; // How far the crane should move

    protected override void Awake()
    {
        base.Awake();
        initialButtonPosition = button.localPosition;
        initialCranePosition = crane.position;
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
        interactor = (IXRActivateInteractor)args.interactorObject;
    }

    private void EndGrab(SelectExitEventArgs args)
    {
        isPressed = false;
        interactor = null;
        ResetButtonAndCrane(); // Reset button and crane when released
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (interactor != null)
        {
            if (isPressed)
            {
                // Ensure the button stays in the pressed position, only resetting on release


                button.localPosition = new Vector3(initialButtonPosition.x, initialButtonPosition.y - buttonPressDistance, initialButtonPosition.z);

                // Move the crane down
                crane.position = new Vector3(crane.position.x, initialCranePosition.y - craneMoveDistance, crane.position.z);
            }
            else
            {
                // Reset button and crane positions if not pressed
                ResetButtonAndCrane();
            }
        }
    }

    private void ResetButtonAndCrane()
    {
        // Reset button position to the initial position
        button.localPosition = initialButtonPosition;

        // Reset crane position to the initial position
        crane.position = initialCranePosition;
    }
}

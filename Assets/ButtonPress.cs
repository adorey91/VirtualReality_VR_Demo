using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonPress : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    [SerializeField] private Transform button;
    [SerializeField] private Transform crane;

    private Vector3 initialButtonPosition;
    private Vector3 initialCranePosition;

    private bool isPressed = false;
    private float buttonPressDistance = 0.01f; // Limit button movement
    private float craneMoveDistance = 0.5f; // Limit crane movement

    private float cranePauseTime = 1.5f; // Pause before moving up

    protected override void Awake()
    {
        base.Awake();
        initialButtonPosition = button.localPosition;
        initialCranePosition = crane.position;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(OnPress);
        selectExited.AddListener(OnRelease);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(OnPress);
        selectExited.RemoveListener(OnRelease);
        base.OnDisable();
    }

    private void OnPress(SelectEnterEventArgs args)
    {
        if (!isPressed)
        {
            isPressed = true;
            StartCoroutine(MoveCrane());
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isPressed = false;
        ResetButton();
    }

    private IEnumerator MoveCrane()
    {
        // Press button
        button.localPosition = new Vector3(initialButtonPosition.x, initialButtonPosition.y - buttonPressDistance, initialButtonPosition.z);

        // Move crane down directly below button
        Vector3 targetPosition = new Vector3(button.position.x, initialCranePosition.y - craneMoveDistance, button.position.z);
        float time = 0;
        while (time < 1f)
        {
            crane.position = Vector3.Lerp(initialCranePosition, targetPosition, time);
            time += Time.deltaTime;
            yield return null;
        }

        // Pause at bottom
        yield return new WaitForSeconds(cranePauseTime);

        // Move crane back up
        time = 0;
        while (time < 1f)
        {
            crane.position = Vector3.Lerp(targetPosition, initialCranePosition, time);
            time += Time.deltaTime;
            yield return null;
        }

        // Reset button
        ResetButton();
    }

    private void ResetButton()
    {
        button.localPosition = initialButtonPosition;
    }
}

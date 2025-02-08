using System;
using System.Collections;
using UnityEngine;

public class VR_Button : MonoBehaviour
{
    // time that the button is set inactive after releasing
    [SerializeField] private float deadTime = 1f;
    // bool used to lock down button during the set dead time
    private bool deadTimeActive = false;

    public static Action onPressed, onReleased;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Button" && !deadTimeActive)
        {
            onPressed.Invoke();
            Debug.Log("Button pressed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Button" && !deadTimeActive)
        {
            //onReleased.Invoke();
            Debug.Log("Button released");

            StartCoroutine(DeadTime());
        }
    }

    private IEnumerator DeadTime()
    {
        deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        deadTimeActive = false;
    }
}

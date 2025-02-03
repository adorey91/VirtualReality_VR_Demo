using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{

    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    bool isPressed = false;
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
    }

   

    private void Update()
    {
        // Check if the ray hits something
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!isPressed)
                {
                    button.transform.localPosition = new Vector3(0, 0.03f, 0);
                    presser = hit.collider.gameObject;
                    onPress.Invoke();
                    isPressed = true;
                }
            }
        }
        else
        {
            // If nothing is hit, reset the button
            if (isPressed)
            {
                ResetButton();
            }
        }

        // If the presser exists but isn't detected anymore, reset the button
        if (isPressed && presser != null && (!hit.collider || hit.collider.gameObject != presser))
        {
            ResetButton();
        }
    }

    private void ResetButton()
    {
        button.transform.localPosition = new Vector3(0, 0.5f, 0);
        onRelease.Invoke();
        isPressed = false;
        presser = null;
    }


    public void Spawn()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localPosition = new Vector3(0, 5f, 2);
        sphere.AddComponent<Rigidbody>();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ShipCollect : MonoBehaviour
{

    [SerializeField] private float dropHeight = 1f;
    [SerializeField] private float dropSpeed = 1f;
    [SerializeField] private float waitTime = 1f;

    [SerializeField] private Transform dropPos;
    private Vector3 shipStartPos;
    private Vector3 shipTargetPos;
    
    public GameObject cow;
    public static bool isHoldingCow = false;
    public static bool isDropping = false;
    private Coroutine dropCoroutine;

    private void Update()
    {
        if(isDropping)
        {
            if(dropCoroutine == null)
                dropCoroutine = StartCoroutine(DropReturn());
        }
    }

    private void FixedUpdate()
    {
        if(isHoldingCow && cow != null)
        {
            Rigidbody cowRb = cow.GetComponent<Rigidbody>();
            Vector3 targetPosition = transform.position + (Vector3.down * 0.5f);
            cowRb.MovePosition(targetPosition);
        }
    }

    private void OnEnable()
    {
        VR_Button.onPressed += DropCrane;
    }

    private void OnDisable()
    {
        VR_Button.onPressed -= DropCrane;
    }


    public void DropCrane()
    {
        isDropping = true;
        shipStartPos = transform.position;
        shipTargetPos = new Vector3(shipStartPos.x, shipStartPos.y - dropHeight, shipStartPos.z);

        Debug.Log(shipTargetPos);
    }


    private IEnumerator DropReturn()
    {
        // step 1. try to abuct cow
        yield return MoveToPosition(shipTargetPos); // Move down
        yield return MoveToPosition(shipStartPos); // Move back up

        // step 2. move cow to drop position if you have cow
        if (isHoldingCow)
        {
            yield return DropCowReturn(); // Move cow to drop position

            //move back to position where cow was picked up
        }

        isDropping = false;
        dropCoroutine = null;
    }


    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        while (Mathf.Abs(transform.position.y - targetPos.y) > 0.05f) // More precise condition
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, dropSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; // Snap to final position
    }

    private IEnumerator DropCowReturn()
    {
        while (Vector3.Distance(transform.position, dropPos.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dropPos.position, dropSpeed * Time.deltaTime);
            yield return null;
        }

        ReleaseCow();

        while(Vector3.Distance(transform.position, shipStartPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, shipStartPos, dropSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = shipStartPos;
    }

    private void ReleaseCow()
    {
        Debug.Log("Releasing cow at drop zone!");

        // Disable the collider to prevent immediate re-abduction
        //Collider cowCollider = cow.GetComponent<Collider>();
        //cowCollider.enabled = false;

        Rigidbody cowRB = cow.GetComponent<Rigidbody>();
        cowRB.isKinematic = false;
        cow.GetComponent<XRGrabInteractable>().enabled = true;
        cow.GetComponent<XRGeneralGrabTransformer>().enabled = true;

        isHoldingCow = false;

        cow = null;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cow"))
        {
            if(other.gameObject.GetComponent<CowScript>().wasPickedUp) return;

            Debug.Log("Cow abducted!");
            isHoldingCow = true;
            other.gameObject.GetComponent<CowScript>().PickedUp();
            cow = other.gameObject;
            cow.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}

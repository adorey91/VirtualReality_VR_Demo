using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{
    [SerializeField] private JoystickValueSO joystick;
    [SerializeField] private float moveSpeed = 2f; // Speed of crane movement
    [SerializeField] private float minDistance = 0.1f; // Minimum distance from boundaries
    [SerializeField] private LayerMask collision; // Layer mask for collision detection
    [SerializeField] private GameObject ship; // Crane object

    private GameObject crane;
    private Rigidbody craneRB;
    private Vector3 leverInput;
    bool isActive;

    private Coroutine dropCoroutine;

    private void Awake()
    {
        crane = this.gameObject;
        craneRB = crane.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isActive && !ShipCollect.isDropping && !ShipCollect.isHoldingCow)
        {
            leverInput = new(joystick.Value.x, 0, joystick.Value.z);
            MoveCrane();
        }
    }

    private void MoveCrane()
    {
        if (crane == null) return;

        if (!CanMove(leverInput)) return;

        craneRB.MovePosition(crane.transform.position + leverInput * moveSpeed * Time.deltaTime);
    }

    private bool CanMove(Vector3 direction)
    {
        if (Physics.Raycast(crane.transform.position, direction, out _, minDistance, collision))
        {
            return false; // Can't move if an obstacle is within minDistance
        }
        return true;
    }

    public void StartCrane()
    {
        isActive = true;
    }

    public void StopCrane()
    {
        isActive = false;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector2 movementInput;
    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    public void OnMove(InputValue value) => movementInput = value.Get<Vector2>();

    private void FixedUpdate()
    {
        if (movementInput == Vector2.zero) 
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 moveVelocity = new Vector3(movementInput.x, 0, movementInput.y) * speed;
        Vector3 nextPos = transform.position + moveVelocity * Time.fixedDeltaTime;

        if (IsFloorUnder(nextPos))
        {
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    bool IsFloorUnder(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 0.1f, Vector3.down, out hit, 1.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Floor");
        }
        return false;
    }
}
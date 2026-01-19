using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Impostazioni")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector2 movementInput;
    private Rigidbody rb;
    private bool isSliding = false;

    void Awake() => rb = GetComponent<Rigidbody>();

    public void OnMove(InputValue value)
    {
        if (!isSliding) movementInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (isSliding) return;

        Vector3 moveDir = new Vector3(movementInput.x, 0, movementInput.y);
        Vector3 moveVelocity = moveDir * speed;
        Vector3 nextPos = transform.position + moveVelocity * Time.fixedDeltaTime;

        if (IsFloorUnder(nextPos))
        {
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

            if (IsIceUnder(transform.position) && movementInput.magnitude > 0.1f)
            {
                StartCoroutine(IceSlideCoroutine(GetSnapDirection(moveDir)));
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private IEnumerator IceSlideCoroutine(Vector3 direction)
    {
        isSliding = true;
        rb.linearVelocity = Vector3.zero;

        while (isSliding)
        {
            Vector3 targetPos = transform.position + direction * gridSize;

            if (IsFloorUnder(targetPos) && !IsObstacleInWay(direction))
            {
                float elapsed = 0;
                Vector3 startPos = transform.position;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime * speed;
                    transform.position = Vector3.Lerp(startPos, targetPos, elapsed);
                    yield return null;
                }
                transform.position = targetPos;

                if (!IsIceUnder(transform.position)) isSliding = false;
            }
            else
            {
                isSliding = false; 
            }
        }
    }

    bool IsFloorUnder(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            return hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Ice");
        }
        return false;
    }

    bool IsIceUnder(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            return hit.collider.CompareTag("Ice");
        }
        return false;
    }

    bool IsObstacleInWay(Vector3 dir)
    {
        return Physics.Raycast
            (transform.position + Vector3.up * 0.2f,
            dir, 
            gridSize, obstacleLayer);
    }

    Vector3 GetSnapDirection(Vector3 move)
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.z)) return new Vector3(Mathf.Sign(move.x), 0, 0);
        return new Vector3(0, 0, Mathf.Sign(move.z));
    }
}
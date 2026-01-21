using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector2 inputVec;
    private bool isMoving = false;

    void Awake() => GetComponent<Rigidbody>().isKinematic = true;

    public void OnMove(InputValue value) => inputVec = value.Get<Vector2>();

    void Update()
    {
        if (!isMoving && inputVec.magnitude > 0.1f)
        {
            Vector3 moveDir = GetSnapDirection(new Vector3(inputVec.x, 0, inputVec.y));
            StartCoroutine(MoveRoutine(moveDir));
        }
    }

    private IEnumerator MoveRoutine(Vector3 direction)
    {
        isMoving = true;

        while (isMoving)
        {
            Vector3 targetPos = transform.position + direction * gridSize;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, gridSize, obstacleLayer))
            {
                PushableBlock block = hit.collider.GetComponent<PushableBlock>();
                if (block != null)
                {
                    if (!block.TryPush(direction)) { isMoving = false; break; }
                }
                else
                {
                    isMoving = false;
                    break;
                }
            }

            if (!IsFloorUnder(targetPos)) { isMoving = false; break; }

            yield return StartCoroutine(SmoothLerp(targetPos));

            if (!IsIceUnder(transform.position))
            {
                isMoving = false; 
            }
        }
    }

    private IEnumerator SmoothLerp(Vector3 target)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, target, elapsed);
            yield return null;
        }
        transform.position = target;
    }

    bool IsFloorUnder(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            return hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Ice");
        }
        return false;
    }

    bool IsIceUnder(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            return hit.collider.CompareTag("Ice");
        }
        return false;
    }

    Vector3 GetSnapDirection(Vector3 move)
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.z)) return new Vector3(Mathf.Sign(move.x), 0, 0);
        return new Vector3(0, 0, Mathf.Sign(move.z));
    }
}
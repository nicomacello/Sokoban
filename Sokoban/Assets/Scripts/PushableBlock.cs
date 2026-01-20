using UnityEngine;
using System.Collections;

public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private bool isMoving = false;

    public bool TryPush(Vector3 direction)
    {
        if (isMoving) return false;

        Vector3 targetPos = transform.position + direction * gridSize;

        if (CanMoveTo(targetPos, direction))
        {
            StartCoroutine(SmoothMove(targetPos, direction));
            return true;
        }
        return false;
    }

    private IEnumerator SmoothMove(Vector3 target, Vector3 direction)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0;

        //move on cell
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, target, elapsed);
            yield return null;
        }

        transform.position = target;

        //move on ice
        if (IsIceUnder(transform.position))
        {
            Vector3 nextTarget = transform.position + direction * gridSize;

            if (CanMoveTo(nextTarget, direction))
            {
                yield return StartCoroutine(SmoothMove(nextTarget, direction));
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

    private bool CanMoveTo(Vector3 pos, Vector3 dir)
    {
        bool hasFloor = Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, 1.5f, ~0); 
        bool hasObstacle = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, gridSize, obstacleLayer);

        return hasFloor && !hasObstacle;
    }

    private bool IsIceUnder(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1.5f))
        {
            return hit.collider.CompareTag("Ice");
        }
        return false;
    }
}
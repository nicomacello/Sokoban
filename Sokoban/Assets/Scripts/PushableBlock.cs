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

        if (IsFloorUnder(targetPos) && !IsObstacleInWay(direction))
        {
            StartCoroutine(SmoothMove(targetPos));
            return true;
        }
        return false;
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, target, elapsed);
            yield return null;
        }
        transform.position = target;

        if (IsIceUnder(transform.position))
        {
            Vector3 dir = (target - startPos).normalized;
            if (!TryPush(dir)) isMoving = false;
        }
        else
        {
            isMoving = false;
        }
    }

    bool IsFloorUnder(Vector3 pos) => Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit h, 1.5f) && (h.collider.CompareTag("Floor") || h.collider.CompareTag("Ice"));
    bool IsIceUnder(Vector3 pos) => Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit h, 1.5f) && h.collider.CompareTag("Ice");
    bool IsObstacleInWay(Vector3 dir) => Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, gridSize, obstacleLayer);
}
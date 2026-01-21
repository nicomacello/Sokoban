using UnityEngine;
using System.Collections;

public class HeavyRock : MonoBehaviour
{
    [Header("Impostazioni")]
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
            StartCoroutine(MoveRoutine(targetPos));
            return true;
        }
        return false;
    }

    private IEnumerator MoveRoutine(Vector3 target)
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

        
        isMoving = false;
    }

    private bool CanMoveTo(Vector3 pos, Vector3 dir)
    {
        bool hasFloor = Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, 1.5f);
        // Controlla ostacoli
        bool hasObstacle = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, gridSize, obstacleLayer);

        return hasFloor && !hasObstacle;
    }
}
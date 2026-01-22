using UnityEngine;
using System.Collections;

public class HeavyRock : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private bool isMoving = false;

    public bool TryPush(Vector3 direction)
    {
        if (isMoving) return false;

        StartCoroutine(PushRoutine(direction));
        return true;
    }

    private IEnumerator PushRoutine(Vector3 direction)
    {
        isMoving = true;
        bool keepMoving = true;

        while (keepMoving)
        {
            Vector3 targetPos = transform.position + direction * gridSize;

            if (!CanMoveTo(targetPos, direction))
            {
                keepMoving = false;
                break;
            }

            Vector3 startPos = transform.position;
            float elapsed = 0;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed);
                yield return null;
            }
            transform.position = targetPos; 

            if (IsOnIce(targetPos))
            {
                keepMoving = false;
            }
          
        }

        isMoving = false;
    }

    private bool CanMoveTo(Vector3 pos, Vector3 dir)
    {
        bool hasFloor = Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, 1.5f);
        bool hasObstacle = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, gridSize, obstacleLayer);

        return hasFloor && !hasObstacle;
    }

    private bool IsOnIce(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            if (hit.collider.CompareTag("Ice"))
            {
                return true;
            }
        }
        return false;
    }
}
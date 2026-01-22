using UnityEngine;
using System.Collections;

public class HeavyRock : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float gridSize = 1f;
    public LayerMask obstacleLayer;

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

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, gridSize, obstacleLayer))
            {
                keepMoving = false;
                break;
            }

            if (!Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1.5f))
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
                Debug.Log("Fermata su ghiaccio!");
            }
            else
            {
                keepMoving = true;
            }
        }

        isMoving = false;
    }

    private bool IsOnIce(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            return hit.collider.CompareTag("Ice");
        }
        return false;
    }
}
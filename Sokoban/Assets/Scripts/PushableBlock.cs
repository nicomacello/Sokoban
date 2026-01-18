using UnityEngine;
using System.Collections;

public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private bool isMoving = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isMoving || !collision.gameObject.CompareTag("Player")) return;

        Vector3 dir = transform.position - collision.transform.position;
        Vector3 moveDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? 
            new Vector3(Mathf.Sign(dir.x), 0, 0) : new Vector3(0, 0, Mathf.Sign(dir.z));

        TryMove(moveDir);
    }

    private void TryMove(Vector3 direction)
    {
        Vector3 targetPos = transform.position + (direction * gridSize);

        if (!IsFloorUnder(targetPos)) 
        {
            Debug.Log("Blocco: Niente pavimento lì!");
            return;
        }

        if (!Physics.Raycast(transform.position + direction * 0.1f, direction, gridSize - 0.2f, obstacleLayer))
        {
            StartCoroutine(SmoothMove(targetPos));
        }
    }

    bool IsFloorUnder(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit, 2f))
        {
            return hit.collider.CompareTag("Floor");
        }
        return false;
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float percent = 0;
        while (percent < 1f)
        {
            percent += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, target, percent);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }
}
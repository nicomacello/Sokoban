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
        if (isMoving) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Calcoliamo la direzione dal Player verso il Blocco
            Vector3 dir = transform.position - collision.transform.position;
            
            // Rendiamo la direzione secca (solo X o solo Z)
            Vector3 moveDir = Vector3.zero;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                moveDir = new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                moveDir = new Vector3(0, 0, Mathf.Sign(dir.z));

            Debug.Log("Tentativo di movimento in direzione: " + moveDir);
            TryMove(moveDir);
        }
    }

    private void TryMove(Vector3 direction)
    {
        Vector3 targetPos = transform.position + direction * gridSize;

        // Visualizza il raggio nel Scene View per debug
        Debug.DrawRay(transform.position, direction * gridSize, Color.red, 1f);

        if (!Physics.Raycast(transform.position, direction, gridSize, obstacleLayer))
        {
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            Debug.Log("Movimento bloccato da un ostacolo!");
        }
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
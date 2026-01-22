using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;
    private Vector3 moveDir;

    public void Setup(Vector3 dir)
    {
        moveDir = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player colpito!");
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}

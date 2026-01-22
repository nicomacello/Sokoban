using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 2f;
    public Vector3 fireDirection = Vector3.forward;
    
    public Transform firePoint;
    public LayerMask obstacleLayer; 

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            CheckAndFire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void CheckAndFire()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, fireDirection, out hit, 50f, obstacleLayer))
        {
            
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player avvistato! Fuoco!");
                SpawnProjectile();
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Debug.Log("Visuale torretta bloccata da: " + hit.collider.name);
            }
        }
        else
        {
            SpawnProjectile();
        }
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Bullet pScript = bullet.GetComponent<Bullet>();
            if (pScript != null) pScript.Setup(fireDirection);
        }
    }

}

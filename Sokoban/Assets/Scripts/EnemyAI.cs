using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public Transform firePoint;         
    public float fireRate = 2f;         
    
    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime && Time.timeScale > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        }
    }

}

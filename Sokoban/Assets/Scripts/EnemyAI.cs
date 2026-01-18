using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 5f;

    private float nextFireTime;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if(distanceToPlayer <= detectionRange)
        {
            Vector3 targetDir = player.position - transform.position;

            Quaternion lockRotation = Quaternion.LookRotation(targetDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, lockRotation, Time.deltaTime * rotationSpeed);

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
            
        }
    }
    
    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    private void OnGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, detectionRange);
    }

}

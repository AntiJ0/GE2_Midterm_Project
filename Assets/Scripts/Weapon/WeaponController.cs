using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("ÃÑ¾Ë ¼³Á¤")]
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 50f;
    public float fireRate = 0.2f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletSpeed;
    }
}
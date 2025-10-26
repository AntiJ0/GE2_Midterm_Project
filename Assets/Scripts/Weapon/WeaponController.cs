using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("총기 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float baseFireRate = 0.2f;
    public float baseReloadTime = 2f;
    public int maxAmmo = 30;
    public float baseAttackDamage = 10f;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        currentAmmo = maxAmmo;

        if (player != null)
        {
            player.currentAmmo = currentAmmo;
            player.magazineSize = maxAmmo;
            player.UpdateUI();
        }
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        float finalDamage = baseAttackDamage * player.attackMultiplier;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * 50f;

        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.damage = finalDamage;
        }

        currentAmmo--;
        player.currentAmmo = currentAmmo; 
        player.UpdateUI();

        float actualFireRate = baseFireRate / player.reloadSpeedMultiplier;
        nextFireTime = Time.time + actualFireRate;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("재장전 중...");

        float reloadTime = baseReloadTime / player.reloadSpeedMultiplier;

        player.isReloading = true;
        player.StartCoroutine(player.ReloadCoroutine(reloadTime));

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        player.currentAmmo = currentAmmo;
        player.isReloading = false;
        player.UpdateUI();
        isReloading = false;

        Debug.Log("재장전 완료!");
    }
}
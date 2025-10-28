using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("√—±‚ º≥¡§")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float baseFireRate = 0.2f;
    public float baseReloadTime = 2f;
    public int maxAmmo = 30;
    public float baseAttackDamage = 10f;

    [Header("¿Ã∆Â∆Æ º≥¡§")]
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashDuration = 0.05f;

    public Vector3 muzzleLocalPosition = Vector3.zero;
    public Vector3 muzzleLocalEuler = Vector3.zero;
    public bool keepPrefabWorldScale = false;

    private float nextFireTime = 0f;
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        player.currentAmmo = maxAmmo;
        player.magazineSize = maxAmmo;
        player.UpdateUI();
    }

    void Update()
    {
        if (player == null) return;
        if (player.isReloading) return;

        if (GameController.Instance != null && GameController.Instance.BlockGameplayInput)
            return;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && !player.isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        if (player.currentAmmo <= 0)
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
            bulletComp.damage = finalDamage;

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint, false);
            flash.transform.localPosition = muzzleLocalPosition;
            flash.transform.localRotation = Quaternion.Euler(muzzleLocalEuler);

            if (keepPrefabWorldScale)
            {
                Vector3 parentLossy = firePoint.lossyScale;
                Vector3 desired = muzzleFlashPrefab.transform.localScale;
                flash.transform.localScale = new Vector3(
                    desired.x / Mathf.Max(parentLossy.x, 1e-6f),
                    desired.y / Mathf.Max(parentLossy.y, 1e-6f),
                    desired.z / Mathf.Max(parentLossy.z, 1e-6f)
                );
            }

            Destroy(flash, muzzleFlashDuration);
        }

        player.ConsumeAmmo();

        float actualFireRate = baseFireRate;
        nextFireTime = Time.time + actualFireRate;
    }

    IEnumerator Reload()
    {
        if (player == null) yield break;
        if (player.isReloading) yield break;

        player.isReloading = true;
        yield return player.StartCoroutine(player.ReloadCoroutine());
    }
}
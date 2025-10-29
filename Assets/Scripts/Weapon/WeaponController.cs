using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class WeaponController : MonoBehaviour
{
    [Header("레지스트리")]
    public WeaponLibrary library; 

    [Header("상태")]
    public WeaponSO weaponData;         
    private PlayerController player;
    private float nextFireTime;
    private Transform firePoint;        
    private Camera cam;                 
    private GameObject spawnedModel;

    [Header("Aiming")]
    public LayerMask aimMask = ~0;     
    public string weaponLayerName = "Weapon"; 

    private void Start()
    {
        player = GetComponent<PlayerController>();
        cam = GetComponentInChildren<Camera>(); 

        WeaponType type = (WeaponType)PlayerPrefs.GetInt("CurrentWeapon", (int)WeaponType.Pistol);

        weaponData = (library != null) ? library.Get(type) : null;
        if (weaponData == null)
        {
            Debug.LogError($"[WeaponController] WeaponSO 로드 실패: {type}");
            enabled = false; return;
        }

        AttachModelAndBindFirePoint();

        player.magazineSize = weaponData.maxAmmo;
        player.currentAmmo = weaponData.maxAmmo;
        player.reloadTime = weaponData.reloadTime; 
        player.UpdateUI();
    }

    private void AttachModelAndBindFirePoint()
    {
        if (spawnedModel != null)
            Destroy(spawnedModel);

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("[WeaponController] Camera를 찾을 수 없음.");
                return;
            }
        }

        spawnedModel = Instantiate(weaponData.weaponModelPrefab, cam.transform);
        spawnedModel.transform.localPosition = weaponData.modelLocalPosition;
        spawnedModel.transform.localRotation = Quaternion.identity;

        Transform found = spawnedModel.transform.Find(weaponData.firePointPath);
        if (found != null)
        {
            firePoint = found;
            Debug.Log($"[WeaponController] firePoint 로드 완료: {firePoint.name} (Scene Instance)");
        }
        else
        {
            firePoint = spawnedModel.GetComponentInChildren<Transform>(true);
            foreach (Transform t in spawnedModel.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.ToLower().Contains("firepoint"))
                {
                    firePoint = t;
                    Debug.Log($"[WeaponController] firePoint 대체 탐색 성공: {t.name}");
                    break;
                }
            }

            if (firePoint == null)
            {
                Debug.LogWarning($"[WeaponController] firePoint 경로를 찾지 못함: '{weaponData.firePointPath}'. " +
                                 "모델 루트에서 firePoint 오브젝트를 만들어 주세요.");
            }
        }

        if (firePoint != null)
        {
            firePoint.localPosition = firePoint.localPosition; 
            firePoint.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        if (player == null || player.isReloading) return;
        if (GameController.Instance != null && GameController.Instance.BlockGameplayInput) return;

        bool wantFire = weaponData.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (wantFire && Time.time >= nextFireTime)
            Fire();

        if (Input.GetKeyDown(KeyCode.R) && !player.isReloading)
            StartCoroutine(Reload());
    }

    private void Fire()
    {
        if (firePoint == null) { Debug.LogError("[WeaponController] firePoint 없음"); return; }
        if (weaponData == null || weaponData.bulletPrefab == null)
        { Debug.LogError("[WeaponController] bulletPrefab 미할당"); return; }

        if (player.currentAmmo <= 0) { StartCoroutine(Reload()); return; }

        Vector3 targetPoint = GetAimPointFromCamera();
        Vector3 mainDir = (targetPoint - firePoint.position);
        if (mainDir.sqrMagnitude < 1e-6f) mainDir = cam.transform.forward;
        mainDir.Normalize();

        if (!weaponData.isShotgun)
        {
            ShootOne(mainDir, weaponData.baseDamage, weaponData.bulletLifetime);
        }
        else
        {
            for (int i = 0; i < weaponData.pelletCount; i++)
            {
                Vector3 dir = RandomDirectionInCone(mainDir, weaponData.pelletSpreadAngle);
                ShootOne(dir, weaponData.baseDamage, weaponData.bulletLifetime);
            }
        }

        SpawnMuzzleFlash();
        player.ConsumeAmmo();
        nextFireTime = Time.time + weaponData.fireRate;
    }

    private void ShootOne(Vector3 direction, float damage, float lifeTime)
    {
        if (direction.sqrMagnitude < 1e-6f) direction = cam.transform.forward;

        Quaternion rot;
        if (direction.sqrMagnitude < 1e-6f) rot = firePoint.rotation;
        else rot = Quaternion.LookRotation(direction, Vector3.up);

        GameObject bullet = Instantiate(weaponData.bulletPrefab, firePoint.position, rot);
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = direction.normalized * weaponData.bulletSpeed;

        var bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null) bulletComp.damage = damage;

        Destroy(bullet, Mathf.Max(0.01f, lifeTime));
    }

    private Vector3 GetAimPointFromCamera()
    {
        if (cam == null) cam = Camera.main;

        int ignoreMask = aimMask;
        int weaponLayer = LayerMask.NameToLayer(weaponLayerName);
        if (weaponLayer >= 0) ignoreMask &= ~(1 << weaponLayer);
        int playerLayer = gameObject.layer;
        ignoreMask &= ~(1 << playerLayer);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ignoreMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return cam.transform.position + cam.transform.forward * 1000f;
    }

    private Vector3 RandomDirectionInCone(Vector3 mainDir, float coneAngleDeg)
    {
        if (mainDir.sqrMagnitude < 1e-6f) mainDir = cam.transform.forward;

        Vector2 offset = Random.insideUnitCircle;
        float tan = Mathf.Tan(coneAngleDeg * Mathf.Deg2Rad);

        Vector3 right = cam.transform.right;
        Vector3 up = cam.transform.up;

        Vector3 dir = (mainDir + (right * offset.x + up * offset.y) * tan).normalized;
        if (dir.sqrMagnitude < 1e-6f) dir = mainDir;
        return dir;
    }

    private void ShootOne(Vector3 dir, float damage)
    {
        GameObject bullet = Instantiate(weaponData.bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = dir * weaponData.bulletSpeed;

        var b = bullet.GetComponent<Bullet>();
        if (b != null) b.damage = damage;

        Destroy(bullet, weaponData.bulletLifetime);
    }

    private void SpawnMuzzleFlash()
    {
        if (firePoint == null) return;

        GameObject flash = null;
        if (weaponData.muzzleFlashPrefab != null)
        {
            flash = Instantiate(weaponData.muzzleFlashPrefab, firePoint, false);
            flash.transform.localPosition = Vector3.forward * weaponData.muzzleForwardOffset;
            flash.transform.localRotation = Quaternion.identity;
            flash.transform.localScale = Vector3.one;
            Destroy(flash, 0.05f);
        }

        if (weaponData.muzzleLightPrefab != null)
        {
            GameObject lightObj = Instantiate(weaponData.muzzleLightPrefab, firePoint, false);
            lightObj.transform.localPosition = Vector3.forward * weaponData.muzzleForwardOffset;
            lightObj.transform.localRotation = Quaternion.identity;
            lightObj.transform.localScale = Vector3.one;
            Destroy(lightObj, 0.05f);
        }
    }

    private IEnumerator Reload()
    {
        if (player == null || player.isReloading) yield break;
        player.isReloading = true;
        yield return player.StartCoroutine(player.ReloadCoroutine()); 
    }

    public void SetWeapon(WeaponType type)
    {
        if (library == null) return;
        var next = library.Get(type);
        if (next == null) return;

        weaponData = next;
        AttachModelAndBindFirePoint();

        player.magazineSize = weaponData.maxAmmo;
        player.currentAmmo = weaponData.maxAmmo;
        player.reloadTime = weaponData.reloadTime;
        nextFireTime = 0f;
        player.UpdateUI();

        PlayerPrefs.SetInt("CurrentWeapon", (int)type);
        PlayerPrefs.Save();
    }
}
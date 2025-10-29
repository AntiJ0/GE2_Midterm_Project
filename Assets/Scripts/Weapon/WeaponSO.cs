using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Data", fileName = "NewWeaponData")]
public class WeaponSO : ScriptableObject
{
    public WeaponType weaponType;

    [Header("ÃÑ±â ÇÁ¸®ÆÕ")]
    public GameObject weaponModelPrefab;   
    public GameObject bulletPrefab;

    [Header("ÀÌÆåÆ® ÇÁ¸®ÆÕ")]
    public GameObject muzzleFlashPrefab;
    public GameObject muzzleLightPrefab;

    [Header("¸ðµ¨/ÃÑ±¸ À§Ä¡")]
    public Vector3 modelLocalPosition;      
    public string firePointPath = "firePoint"; 
    public float muzzleForwardOffset = 0.03f;  

    [Header("±âº» ½ºÅÈ")]
    public float baseDamage = 10f;
    public float fireRate = 0.2f;      
    public float reloadTime = 2f;      
    public int maxAmmo = 30;
    public float bulletSpeed = 50f;
    public float bulletLifetime = 2f;

    [Header("¹ß»ç ¸ðµå")]
    public bool isAutomatic = true;    
    public bool isShotgun = false;     

    [Header("¼¦°Ç ¿É¼Ç")]
    public int pelletCount = 10;         
    public float pelletSpreadAngle = 6f; 
}
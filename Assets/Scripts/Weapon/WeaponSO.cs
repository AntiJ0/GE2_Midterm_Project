using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Data", fileName = "NewWeaponData")]
public class WeaponSO : ScriptableObject
{
    public WeaponType weaponType;

    [Header("�ѱ� ������")]
    public GameObject weaponModelPrefab;   
    public GameObject bulletPrefab;

    [Header("����Ʈ ������")]
    public GameObject muzzleFlashPrefab;
    public GameObject muzzleLightPrefab;

    [Header("��/�ѱ� ��ġ")]
    public Vector3 modelLocalPosition;      
    public string firePointPath = "firePoint"; 
    public float muzzleForwardOffset = 0.03f;  

    [Header("�⺻ ����")]
    public float baseDamage = 10f;
    public float fireRate = 0.2f;      
    public float reloadTime = 2f;      
    public int maxAmmo = 30;
    public float bulletSpeed = 50f;
    public float bulletLifetime = 2f;

    [Header("�߻� ���")]
    public bool isAutomatic = true;    
    public bool isShotgun = false;     

    [Header("���� �ɼ�")]
    public int pelletCount = 10;         
    public float pelletSpreadAngle = 6f; 
}
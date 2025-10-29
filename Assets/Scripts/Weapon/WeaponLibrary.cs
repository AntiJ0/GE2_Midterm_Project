using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Library", fileName = "WeaponLibrary")]
public class WeaponLibrary : ScriptableObject
{
    public WeaponSO[] weapons; 

    public WeaponSO Get(WeaponType type)
    {
        if (weapons == null) return null;
        return weapons.FirstOrDefault(w => w != null && w.weaponType == type);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "Enemy/Zombie Data")]
public class ZombieSO : ScriptableObject
{
    public string zombieName;
    public float maxHP = 100f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public bool isBoss = false;
}
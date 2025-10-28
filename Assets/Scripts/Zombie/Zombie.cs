using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    private ZombieSO data;
    private NavMeshAgent agent;
    private Transform target;
    private float curHP;
    private bool isAttacking = false;
    private ZombieHealthBar healthBar;
    private Rigidbody rb;

    private Renderer[] renderers;
    private Color originalColor;
    private bool isFlashing = false;

    public void Initialize(ZombieSO so)
    {
        data = so;
        curHP = data.maxHP;

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        if (agent != null)
            agent.speed = data.moveSpeed;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = true;
            rb.freezeRotation = true;
        }

        healthBar = GetComponentInChildren<ZombieHealthBar>();
        if (healthBar != null)
            healthBar.UpdateHealthBar(curHP, data.maxHP);

        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
            originalColor = renderers[0].material.color;
    }

    void Update()
    {
        if (data == null) return;
        if (isAttacking) return;

        FindClosestPlayer();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= data.attackRange)
            {
                StartCoroutine(Attack());
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }

    void FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }

        target = nearest;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(1.5f);

        if (target != null)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(data.attackDamage);
            }
        }

        yield return new WaitForSeconds(data.attackCooldown - 1.5f);

        agent.isStopped = false;
        isAttacking = false;
    }

    public void TakeDamage(float amount)
    {
        curHP -= amount;
        if (healthBar != null)
            healthBar.UpdateHealthBar(curHP, data.maxHP);

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (curHP <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        if (target != null)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
                player.AddGold(data.rewardGold);
        }

        Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        isFlashing = true;

        foreach (Renderer rend in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_Color", Color.red);
            rend.SetPropertyBlock(block);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (Renderer rend in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_Color", originalColor);
            rend.SetPropertyBlock(block);
        }

        isFlashing = false;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
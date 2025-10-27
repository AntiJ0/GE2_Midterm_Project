using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Zombie z = other.GetComponent<Zombie>();
        if (z != null)
        {
            z.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
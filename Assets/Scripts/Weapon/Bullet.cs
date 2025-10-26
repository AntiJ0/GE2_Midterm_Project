using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    var target = collision.gameObject.GetComponent<IDamageable>();
    //    if (target != null)
    //    {
    //        target.TakeDamage(damage);
    //    }
    //
    //    Destroy(gameObject);
    //}
}
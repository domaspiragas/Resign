using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float range = 1f;

    // tracks how long the bullet lives
    private float m_lifeTimer;
    void Start()
    {
        // initialize a time at which the bullet will get destroyed (now + lifespan)
        m_lifeTimer = Time.time + range;
    }
    void Update()
    {
        // when now is later than the start+lifespan, bullet should be destroyed
        if(m_lifeTimer < Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //TODO: If you hit an inanimate object, destroy projectile here.
    }
    public void Hit()
    {
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }

}

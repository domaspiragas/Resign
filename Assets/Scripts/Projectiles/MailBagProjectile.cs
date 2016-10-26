using UnityEngine;
using System.Collections;

public class MailBagProjectile : MonoBehaviour {

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
        if (m_lifeTimer < Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "MovingEnemy")
        {
            col.gameObject.GetComponent<MovingEnemy>().TakeDamage(damage);
            Hit();
        }
        else if (col.tag == "StationaryEnemy")
        {
            col.gameObject.GetComponent<StationaryEnemy>().TakeDamage(damage);
            Hit();
        }
        else if(col.tag == "RatEnemy")
        {
            col.gameObject.GetComponent<RatEnemy>().TakeDamage(damage);
            Hit();
        }
        else if (col.tag == "InternEnemy")
        {
            col.gameObject.GetComponent<InternEnemy>().TakeDamage(damage);
            Hit();
        }
        else if (col.tag == "MailmanEnemy")
        {
            col.gameObject.GetComponent<MailmanEnemy>().TakeDamage(damage);
            Hit();
        }
        else if (col.tag == "PhysicalObject")
        {
            Hit();
        }
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

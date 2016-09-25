using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour

{
    public float damage;
    // how often player can press attack
    public float attackRate;
    // Delay before damage is dealt
    public float attackDelay;
    // how long the hitbox is active for.
    public float attackDuration;
    //the hitbox for the melee weapon
    public BoxCollider2D hitBox;

    // the time the last swing took place
    private float m_previousAttackTime;
    private float m_attackDelay;
    private float m_attackDuration;
    private bool m_attacking = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "StationaryEnemy")
        {
            col.gameObject.GetComponent<StationaryEnemy>().TakeDamage(damage);
        }
        else if(col.tag == "MovingEnemy")
        {
            col.gameObject.GetComponentInParent<MovingEnemy>().TakeDamage(damage);
        }
    }
    void Start()
    {
        m_attackDuration = attackDuration;
        m_attackDelay = attackDelay;
        m_previousAttackTime = Time.time;
        hitBox.enabled = false;
    }
    void Update()
    {

        if (m_attacking)
        {
            // if we're attacking start the timer for the attack delay
            m_attackDelay -= Time.deltaTime;
            if(m_attackDelay <= 0)
            {
                // when the delay has passed, activate the hit box
                hitBox.enabled = true;
                m_attackDuration -= Time.deltaTime;
                if (m_attackDuration <= 0)
                {
                   // after the attack duration reset values for the next attack turn off hitbox.
                    m_attacking = false;
                    m_attackDelay = attackDelay;
                    m_attackDuration = attackDuration;
                    hitBox.enabled = false;
                }
            }
        }
    }
    // if successful return true, if not return false
    public bool Swing(float currentTime)
    {
        if (currentTime - m_previousAttackTime > attackRate + attackDelay)
        {
            m_attacking = true;
            m_previousAttackTime = Time.time;
            return true;
        }
        else { return false; }

    }


}

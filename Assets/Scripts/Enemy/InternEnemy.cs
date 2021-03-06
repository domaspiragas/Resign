﻿using UnityEngine;
using System.Collections;
using Prime31;

public class InternEnemy : MonoBehaviour
{
    public float health = 50f;
    public bool patrol = false;
    public float patrolRange = 10;
    public float speed = 2f;
    public float chanceOfHealthDrop = 15f;
    public float pushbackDistance = .2f;
    public float pushbackSpeed = 10;
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;
    public GameObject healthPickUp;
    public GameObject healthBar;

    private InternMeleeWeapon m_meleeWeapon;
    private InternRangedWeapon m_rangedWeapon;
    private CharacterController2D m_controller;
    private AnimationController2D m_animator;

    private bool m_followPlayer = false;
    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private bool m_moveRight;
    private bool m_meleeAttack = false;
    private bool m_patrol;
    private bool m_pushedBack = false;
    private bool m_redFlash;
    private float m_meleeTimer;
    private float m_pushbackTimer;
    private float m_redFlashTimer;


    private float m_health;
    // Use this for initialization
    void Start()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_meleeWeapon = (InternMeleeWeapon)meleeWeapon.GetComponent(typeof(InternMeleeWeapon));
        m_rangedWeapon = (InternRangedWeapon)rangedWeapon.GetComponent(typeof(InternRangedWeapon));
        m_health = health;
        m_patrol = patrol;
        m_startingPosition = this.transform.position;
        m_animator.setFacing("Left");

    }


    // Update is called once per frame
    void Update()
    {
        CheckIfDead();

        Vector3 velocity = m_controller.velocity;
        velocity.x = 0;
        // Patrols left and right patrolRange distance
        if (!m_followPlayer && !m_pushedBack && m_patrol)
        {
            m_animator.setAnimation("MovingEnemyIdle");
            if (this.transform.position.x >= m_startingPosition.x + patrolRange && !m_meleeAttack)
            {
                m_moveRight = false;
                m_animator.setFacing("Left");
            }
            else if (this.transform.position.x <= m_startingPosition.x - patrolRange)
            {
                m_moveRight = true;
                m_animator.setFacing("Right");
            }
            if (m_moveRight)
            {
                velocity.x = speed;

            }
            else
            {
                velocity.x = -speed;
            }
        }
        else if (!m_meleeAttack && m_followPlayer && !m_pushedBack)
        {
            float positionDifference = this.transform.position.x - m_playerPosition.x;

            // our position - palyer position, if positive we're to the right of the palyer else we're to the left
            // if the player is farther away ranged attack
            //if (positionDifference > 1.5f && positionDifference < 8f)
            //{

            //    m_animator.setFacing("Left");
            //    // overlap the melee cooldown with ranged cooldown so you don't get shot immediately after running away.
            //    if (!m_meleeWeapon.OnCoolDown(Time.time))
            //    {
            //        m_rangedWeapon.EnemyShoot(Time.time, m_playerPosition);
            //    }
            //}
            //else if (positionDifference < -1.5f && positionDifference > -8f)
            //{
            //    m_animator.setFacing("Right");
            //    if (!m_meleeWeapon.OnCoolDown(Time.time))
            //    {
            //        m_rangedWeapon.EnemyShoot(Time.time, m_playerPosition);
            //    }
            //}
            // if the player is out of range, move toward them.
            if (positionDifference >= 1.5f)
            {
                m_animator.setFacing("Left");
                velocity.x = -speed;

            }
            else if (positionDifference <= -1.5f)
            {
                m_animator.setFacing("Right");
                velocity.x = speed;
            }

                // if the player is close melee
            else 
            {
                m_meleeAttack = true;
                m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
                if (!m_meleeWeapon.OnCoolDown(Time.time))
                {
                    m_meleeWeapon.Swing(Time.time);
                    m_animator.setAnimation("MovingEnemyMelee");
                }
            }
        }
        else if (m_pushedBack)
        {
            float positionDifference = this.transform.position.x - m_playerPosition.x;
            // if the enemy is to the right of the player get pushed further right, else pushed left
            if (positionDifference > 0)
            {
                velocity.x = pushbackSpeed;
            }
            else
            {
                velocity.x = -pushbackSpeed;
            }
        }
        // handles pushback from mop
        if (m_pushbackTimer < Time.time)
        {
            m_pushedBack = false;
        }
        if (m_meleeAttack)
        {
            if (m_meleeTimer < Time.time)
            {
                m_meleeAttack = false;
                m_animator.setAnimation("MovingEnemyIdle");
            }
        }
        // handles flashing red when damage has been taken
        if(m_redFlash)
        {
            if(m_redFlashTimer > .10f)
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                m_redFlash = false;
            }
            m_redFlashTimer += Time.deltaTime;
        }
        velocity.y += -30 * Time.deltaTime;
        m_controller.move(velocity * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        UpdateHealthUI();
        // flash red and start timer
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        m_redFlash = true;
        m_redFlashTimer = 0;
    }

    public void SetFollowPlayer(bool follow)
    {
        m_followPlayer = follow;
    }

    public void SetPlayerPosition(Vector3 position)
    {
        m_playerPosition = position;
    }
    private void CheckIfDead()
    {
        if (m_health <= 0)
        {
            if (healthPickUp != null)
            {
                DropHealth();
            }
            Destroy(gameObject);
        }
    }
    private void DropHealth()
    {
        // if we get a number within our percent drop we will spawn a health drop.
        if (Random.Range(1, 101) <= chanceOfHealthDrop)
        {
            GameObject healthDrop = (GameObject)Instantiate(healthPickUp, this.transform.position, Quaternion.identity);
            healthDrop.gameObject.GetComponent<HealthPickUp>().AssignLobDirection(Random.Range(-.3f, .3f));
        }
    }
    public void SetPushedBack()
    {
        m_pushedBack = true;
        // start the pushback timer
        m_pushbackTimer = Time.time + pushbackDistance;
    }
    public bool GetPushedBack()
    {
        return m_pushedBack;
    }
    private void UpdateHealthUI()
    {
        healthBar.transform.localScale = new Vector3((m_health / health), healthBar.transform.localScale.y, 0);
    }
}

﻿using UnityEngine;
using System.Collections;
using Prime31;

public class MovingEnemy : MonoBehaviour {


    public float health = 50f;
    public float patrolRange = 10;
    public float speed = 2f;
    public GameObject meleeWeapon;

    private MeleeWeapon m_meleeWeapon;
    private CharacterController2D m_controller;
    private AnimationController2D m_animator;

    private bool m_followPlayer = false;
    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private bool m_moveRight;
    private bool m_meleeAttack = false;
    private float m_meleeTimer;

    private float m_health;
    // Use this for initialization
    void Start ()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_meleeWeapon = (MeleeWeapon)meleeWeapon.GetComponent(typeof(MeleeWeapon));
        m_health = health;
        m_startingPosition = this.transform.position;
	}


    // Update is called once per frame
    void Update ()
    {
        CheckIfDead();

        Vector3 velocity = m_controller.velocity;
        velocity.x = 0;
        // Patrols left and right patrolRange distance
        if (!m_followPlayer)
        {
            m_animator.setAnimation("MovingEnemyIdle");
            if(this.transform.position.x >= m_startingPosition.x + patrolRange && !m_meleeAttack)
            {
                m_moveRight = false;
                m_animator.setFacing("Left");
            }
            else if(this.transform.position.x <= m_startingPosition.x - patrolRange)
            {
                m_moveRight = true;
                m_animator.setFacing("Right");
            }
            if(m_moveRight)
            {
                velocity.x = speed;

            }
            else
            {
                velocity.x = -speed;
            }
        }
        else if (!m_meleeAttack && m_followPlayer)
        {
            float positionDifference = this.transform.position.x - m_playerPosition.x;

            if (positionDifference > 0 && positionDifference > 1.5f)
            {
                velocity.x = -speed;
                m_animator.setFacing("Left");
            }
            else if (positionDifference < 0 && positionDifference < -1.5f)
            {
                velocity.x = speed;
                m_animator.setFacing("Right");
            }
            else
            {
                m_meleeAttack = true;
                m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
                if (m_meleeWeapon.Swing(Time.time))
                {
                    m_animator.setAnimation("MovingEnemyMelee");
                }
            }
        }
        if (m_meleeAttack)
        {
            if (m_meleeTimer < Time.time)
            {
                m_meleeAttack = false;
                m_animator.setAnimation("MovingEnemyIdle");
            }
        }
        velocity.y += -30 * Time.deltaTime;
        m_controller.move(velocity * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
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
            Destroy(gameObject);
        }
    }
}
﻿using UnityEngine;
using System.Collections;
using Prime31;

public class MailmanEnemy : MonoBehaviour {

    public float health = 50f;
    public bool patrol = false;
    public float patrolRange = 10;
    public float speed = 2f;
    public float chanceOfHealthDrop = 15f;
    public float pushbackDistance = .2f;
    public float pushbackSpeed = 10;
    public GameObject rangedWeapon;
    public GameObject healthPickUp;
    public GameObject healthBar;

    private MailmanRangedWeapon m_rangedWeapon;
    private CharacterController2D m_controller;
    private AnimationController2D m_animator;

    // handles enemy movement 
    private bool m_followPlayer = false;
    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private bool m_moveRight;
    private bool m_pushedBack = false;
    private float m_pushbackTimer;
    private bool m_patrol;

    // handles flashing red when taking damage
    private bool m_redFlash;
    private float m_redFlashTimer;

    private string m_idle = "mailroom_stationary";
    private string m_attack = "mailroom_attack";

    private float m_health;
    // Use this for initialization
    void Start()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_rangedWeapon = (MailmanRangedWeapon)rangedWeapon.GetComponent(typeof(MailmanRangedWeapon));
        m_health = health;
        m_patrol = patrol;
        m_startingPosition = this.transform.position;
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
            m_animator.setAnimation(m_idle);
            if (this.transform.position.x >= m_startingPosition.x + patrolRange)
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
        else if (m_followPlayer && !m_pushedBack)
        {
            float positionDifference = this.transform.position.x - m_playerPosition.x;
            
            // our position - palyer position, if positive we're to the right of the palyer else we're to the left
            // if the player is farther away ranged attack
            if (positionDifference > 8f && positionDifference < 13f)
            {

                m_animator.setFacing("Left");
                m_rangedWeapon.EnemyShoot(Time.time, m_playerPosition);
                m_animator.setAnimation(m_attack);
            }
            else if (positionDifference < -8f && positionDifference > -13f)
            {
                m_animator.setFacing("Right");

                    m_rangedWeapon.EnemyShoot(Time.time, m_playerPosition);
                m_animator.setAnimation(m_attack);

            }
            // if the player is out of range, move away form them.
            else if (positionDifference <= 8f && positionDifference >= 0)
            {
                m_animator.setFacing("Right");
                velocity.x = speed;
                m_animator.setAnimation(m_idle);

            }
            else if (positionDifference >= -8f && positionDifference <= 0)
            {
                m_animator.setFacing("Left");
                velocity.x = -speed;
                m_animator.setAnimation(m_idle);
            }

            else if (positionDifference >=13f)
            {
                m_animator.setFacing("Left");
                velocity.x = -speed;
                m_animator.setAnimation(m_idle);
            }
            else
            {
                m_animator.setFacing("Right");
                velocity.x = speed;
                m_animator.setAnimation(m_idle);
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
        // handles flashing red when damage has been taken
        if (m_redFlash)
        {
            if (m_redFlashTimer > .10f)
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                m_redFlash = false;
            }
            m_redFlashTimer += Time.deltaTime;
        }
        if (m_pushbackTimer < Time.time)
        {
            m_pushedBack = false;
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

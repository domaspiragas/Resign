using UnityEngine;
using System.Collections;

public class JanitorBoss : MonoBehaviour
{
    public float health = 50f;
    public float speed = 9f;
    public float sweepSpeed = 15f;
    public float chanceOfHealthDrop = 15f;
    public GameObject meleeWeapon;
    public GameObject healthPickUp;
    public GameObject waterTrap;

    private JanitorMeleeWeapon m_meleeWeapon;
    private JanitorCharacterController2D m_controller;
    private AnimationController2D m_animator;

    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private Vector3 m_leftJumpPosition = new Vector3(138f, 6f, 0);
    private Vector3 m_rightJumpPosition = new Vector3(175f, 6f, 0);
    private Vector3 m_controlPosition = new Vector3(157f, 17f, 0);
    private bool m_moveRight;
    private bool m_meleeAttack;
    private bool m_jumpLeft;
    private bool m_jumpRight;
    private bool m_jumpBool;
    private bool m_chasePlayer;
    private bool m_sweepAttack;
    private bool m_sweepLeft;
    private bool m_sweepRight;
    private bool m_waterTrap;
    private bool m_seePlayer;
    private bool m_whatNext = true;
    private float m_meleeTimer;
    private float m_jumpTimer;
    private float m_jumpDelay;
    private float m_chasePlayerTimer;
    private float m_trapCoolDown;

    //Animations
    private string m_idle = "janitor_walk";
    private string m_attack = "janitor_attack";
    private string m_jump = "janitor_jump";
    private string m_walk = "janitor_walk";
    private string m_sweep = "janitor_walk";




    private float m_health;
    // Use this for initialization
    void Start()
    {
        m_controller = gameObject.GetComponent<JanitorCharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_meleeWeapon = (JanitorMeleeWeapon)meleeWeapon.GetComponent(typeof(JanitorMeleeWeapon));
        m_health = health;
        m_startingPosition = this.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_seePlayer)
        {
            CheckIfDead();

            Vector3 velocity = m_controller.velocity;
            velocity.x = 0;
            // cooldown for trap attack.
            m_trapCoolDown += Time.deltaTime;
            // determine the boss's next move. 
            if (m_whatNext)
            {
                int nextMove = Random.Range(1, 101);
                if (nextMove <= 25)
                {
                    m_jumpRight = true;
                    m_jumpBool = true;
                }
                else if (nextMove > 25 && nextMove <= 50)
                {
                    m_chasePlayer = true;

                }
                else if (nextMove > 50 && nextMove <= 75)
                {
                    m_sweepAttack = true;
                }
                else
                {
                    m_waterTrap = true;
                    m_trapCoolDown = 0;
                }

                m_whatNext = false;

            }
            else
            {
                if (m_jumpRight)
                {
                    m_jumpDelay += Time.deltaTime;
                    if (m_jumpDelay > 2)
                    {
                        if (m_jumpBool)
                        {
                            velocity.y = Mathf.Sqrt(2f * 10 * 50);
                            m_animator.setAnimation(m_jump);
                            m_animator.setFacing("Right");
                            m_jumpBool = false;
                        }
                        // jump to the position on the Right side of the boss room
                        velocity.y += -50 * Time.deltaTime;
                        velocity.x = 30;
                        m_controller.move(velocity * Time.deltaTime);
                        m_jumpTimer += Time.deltaTime;
                        // if we've reached the side, prepare to jump to the other side
                        if (this.transform.position.x > m_rightJumpPosition.x)
                        {
                            m_animator.setAnimation(m_idle);
                            m_animator.setFacing("Left");
                            m_jumpRight = false;
                            m_jumpLeft = true;
                            m_jumpBool = true;
                            m_jumpTimer = 0;
                            m_jumpDelay = 0;
                        }
                    }
                }
                else if (m_jumpLeft)
                {

                    // delay the jump to the other side by 2 seconds
                    m_jumpDelay += Time.deltaTime;
                    if (m_jumpDelay > 2)
                    {
                        if (m_jumpBool)
                        {
                            velocity.y = Mathf.Sqrt(2f * 10 * 50);
                            m_animator.setAnimation(m_jump);
                            m_animator.setFacing("Left");
                            m_jumpBool = false;
                        }
                        velocity.x = -30;

                        m_jumpTimer += Time.deltaTime;
                        if (this.transform.position.x < m_leftJumpPosition.x && m_controller.isGrounded)
                        {
                            m_animator.setAnimation(m_idle);
                            m_animator.setFacing("Right");
                            m_jumpLeft = false;
                            m_jumpTimer = 0;
                            m_whatNext = true;
                            m_jumpDelay = 0;
                        }
                    }
                    velocity.y -= 50 * Time.deltaTime;
                    m_controller.move(velocity * Time.deltaTime);
                }
                else if (m_chasePlayer)
                {
                    m_chasePlayerTimer += Time.deltaTime;

                    float positionDifference = this.transform.position.x - m_playerPosition.x;
                    float positionDifferenceY = this.transform.position.y - m_playerPosition.y;

                    // our position - palyer position, if positive we're to the right of the palyer else we're to the left
                    if (positionDifference > 0 && positionDifference < 1.5f && Mathf.Abs(positionDifferenceY) < 2)
                    {
                        m_animator.setFacing("Left");

                        if (m_meleeWeapon.Swing(Time.time))
                        {
                            m_animator.setAnimation(m_attack);
                            m_meleeAttack = true;
                            m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
                        }
                    }
                    else if (positionDifference < 0 && positionDifference > -1.5f && Mathf.Abs(positionDifferenceY) < 2)
                    {
                        m_animator.setFacing("Right");

                        if (m_meleeWeapon.Swing(Time.time))
                        {
                            m_animator.setAnimation(m_attack);
                            m_meleeAttack = true;
                            m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
                        }
                    }
                    else if (positionDifference >= 1.5f && !m_meleeAttack)
                    {
                        velocity.x = -speed;
                        m_animator.setFacing("Left");
                        m_animator.setAnimation(m_walk);
                    }
                    else if (!m_meleeAttack)
                    {
                        velocity.x = speed;
                        m_animator.setFacing("Right");
                        m_animator.setAnimation(m_walk);
                    }

                    if (m_meleeAttack)
                    {
                        if (m_meleeTimer < Time.time)
                        {
                            m_meleeAttack = false;
                            m_animator.setAnimation(m_idle);
                        }
                    }
                    if (m_chasePlayerTimer > 10)
                    {
                        m_meleeAttack = false;
                        m_animator.setAnimation(m_walk);
                        m_chasePlayerTimer = 0;
                        m_chasePlayer = false;
                        m_whatNext = true;
                    }
                    velocity.y += -50 * Time.deltaTime;
                    m_controller.move(velocity * Time.deltaTime);
                }
                else if (m_sweepAttack)
                {
                    float positionDifference = this.transform.position.x - m_playerPosition.x;

                    if (positionDifference < 0 && !m_sweepLeft && !m_sweepRight)
                    {
                        m_sweepRight = true;
                    }
                    else if (positionDifference > 0 && !m_sweepLeft && !m_sweepRight)
                    {
                        m_sweepLeft = true;
                    }

                    if (m_sweepLeft)
                    {
                        //TODO : add sweep animation here.
                        m_animator.setAnimation(m_sweep);
                        m_animator.setFacing("Left");
                        velocity.x = -sweepSpeed;
                        if (this.transform.position.x <= m_leftJumpPosition.x)
                        {
                            m_sweepLeft = false;
                            m_sweepRight = true;
                            m_animator.setFacing("Right");
                            m_animator.setAnimation(m_idle);
                        }
                    }
                    else if (m_sweepRight)
                    {

                        m_animator.setAnimation(m_sweep);
                        m_animator.setFacing("Right");
                        velocity.x = sweepSpeed;
                        if (this.transform.position.x >= m_rightJumpPosition.x)
                        {
                            m_sweepRight = false;
                            m_sweepAttack = false;
                            m_whatNext = true;
                            m_animator.setFacing("Left");
                            m_animator.setAnimation(m_idle);

                        }
                    }
                    velocity.y += -50 * Time.deltaTime;
                    m_controller.move(velocity * Time.deltaTime);
                }
                else if (m_waterTrap)
                {
                    // pick a random spot within the range
                    float positionDifference = this.transform.position.x - m_playerPosition.x;
                    // walk to the spot
                    if (positionDifference >= 6f)
                    {
                        velocity.x = -speed;
                        m_animator.setFacing("Left");
                    }
                    else if (positionDifference <= -6f)
                    {
                        velocity.x = speed;
                        m_animator.setFacing("Right");
                    }
                    // spawn the trap at our spot
                    else
                    {
                        GameObject trap = (GameObject)Instantiate(waterTrap, this.transform.position, Quaternion.identity);
                        m_waterTrap = false;
                        m_whatNext = true;
                    }

                    velocity.y += -50 * Time.deltaTime;
                    m_controller.move(velocity * Time.deltaTime);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
    }

    public void SetPlayerPosition(Vector3 position)
    {
        m_playerPosition = position;
    }
    public void SetSeePlayer()
    {
        m_seePlayer = true;
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

    private Vector3 CalculateBezierPoint(float t, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint)
    {
        float u = 1 - t;
        float uu = u * u;

        Vector3 point = uu * startPosition;
        point += 2 * u * t * controlPoint;
        point += t * t * endPosition;

        return point;
    }
}

using UnityEngine;
using System.Collections;

public class JanitorBoss : MonoBehaviour
{
    public float health = 50f;
    public float speed = 2f;
    public float chanceOfHealthDrop = 15f;
    public GameObject meleeWeapon;
    public GameObject healthPickUp;

    //private JanitorMeleeWeapon m_meleeWeapon;
    private JanitorCharacterController2D m_controller;
    private AnimationController2D m_animator;

    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private Vector3 m_leftJumpPosition = new Vector3(138f, 6f, 0);
    private Vector3 m_rightJumpPosition = new Vector3(176f, 6f, 0);
    private Vector3 m_controlPosition = new Vector3(157f, 17f, 0);
    private bool m_moveRight;
    private bool m_meleeAttack = false;
    private bool m_jumpLeft = true;
    private bool m_jumpRight;
    private float m_meleeTimer;
    private float m_jumpTimer;
    private float m_jumpDelay;



    private float m_health;
    // Use this for initialization
    void Start()
    {
        m_controller = gameObject.GetComponent<JanitorCharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        // m_meleeWeapon = (JanitorMeleeWeapon)meleeWeapon.GetComponent(typeof(JanitorMeleeWeapon));
        m_health = health;
        m_startingPosition = this.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        CheckIfDead();

        Vector3 velocity = m_controller.velocity;
        velocity.x = 0;

        if (!m_meleeAttack)
        {
            if (m_jumpLeft)
            {
                this.transform.position = CalculateBezierPoint(m_jumpTimer / 1.5f, this.transform.position, m_rightJumpPosition, m_controlPosition);
                m_jumpTimer += Time.deltaTime;
                if (this.transform.position.x > m_rightJumpPosition.x)
                {
                    m_jumpLeft = false;
                    m_jumpRight = true;
                    m_jumpTimer = 0;
                }
            }
            else if (m_jumpRight)
            {
                m_jumpDelay += Time.deltaTime;
                if (m_jumpDelay > 2)
                {

                    this.transform.position = CalculateBezierPoint(m_jumpTimer / 1.5f, this.transform.position, m_leftJumpPosition, m_controlPosition);
                    m_jumpTimer += Time.deltaTime;
                    if (this.transform.position.x < m_leftJumpPosition.x)
                    {
                        m_jumpRight = false;
                        m_jumpTimer = 0;
                        m_jumpDelay = 0;
                    }
                }
            }
            //float positionDifference = this.transform.position.x - m_playerPosition.x;

            //// our position - palyer position, if positive we're to the right of the palyer else we're to the left
            //if (positionDifference > 0 && positionDifference > 1.5f)
            //{
            //    velocity.x = -speed;
            //    m_animator.setFacing("Left");
            //}
            //else if (positionDifference < 0 && positionDifference < -1.5f)
            //{
            //    velocity.x = speed;
            //    m_animator.setFacing("Right");
            //}
            //else
            //{
            //    m_meleeAttack = true;
            //    m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
            //    if (m_meleeWeapon.Swing(Time.time))
            //    {
            //        m_animator.setAnimation("MovingEnemyMelee");
            //    }
            //}
        }
        if (m_meleeAttack)
        {
            if (m_meleeTimer < Time.time)
            {
                m_meleeAttack = false;
                m_animator.setAnimation("MovingEnemyIdle");
            }
        }

        velocity.y += -50 * Time.deltaTime;
        m_controller.move(velocity * Time.deltaTime);

    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
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

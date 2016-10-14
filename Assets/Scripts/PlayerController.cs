using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prime31;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    // Weapons
    //TODO: Get rid of these and just use array index 0 for starter weapons
    private MeleeWeapon m_meleeWeapon;
    private RangedWeapon m_rangedWeapon;
    //health ui
    private GameObject m_healthUI;
    //object for raycasting
    private CharacterController2D m_controller;
    //object for animations
    private AnimationController2D m_animator;
    // player's hitbox
    private BoxCollider2D m_playerHitBox;
    //track players health inside class
    private float m_playerHealth;
    //bools for player state
    private bool m_playerControl = true;
    private bool m_roll = false;
    private bool m_meleeAttack = false;
    private bool m_touchingClimbable = false;
    private bool m_isClimbing = false;
    private bool m_touchingStairway = false;

    //melee animation timer
    private float m_meleeTimer = 0;
    //roll cooldown
    private float m_rollTimer = 0;
    private float m_rollCooldownTimestamp;
    private float m_rollCount = 3;

    private Vector3 m_stairwayDestionation;

    //reference to the main camera
    public GameObject playerCamera;
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;

    /* ADJUSTABLE IN UNITY VALUES */

    public float health = 100f;
    // movement
    public float movementSpeed = 6f;

    // jump
    public float jumpHeight = 2f;

    // roll
    public float rollTime = 2f;
    public float rollSpeed = 0.5f;
    public float rollCooldown = 3f;

    public float gravity = -30f;

    public MeleeWeapon[] meleeWeapons = new MeleeWeapon[5];
    public RangedWeapon[] rangedWeapons = new RangedWeapon[5];

    // Use this for initialization
    void Start()
    {

        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_playerHitBox = gameObject.GetComponent<BoxCollider2D>();
        //Initial weapons loaded in
        m_rangedWeapon = (RangedWeapon)rangedWeapon.GetComponent(typeof(RangedWeapon));
        m_meleeWeapon = (MeleeWeapon)meleeWeapon.GetComponent(typeof(MeleeWeapon));
        //attach camera and start following our player
        playerCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);
        m_rollCooldownTimestamp = Time.time;
        m_playerHealth = health;
        //get the health ui object
        m_healthUI = GameObject.Find("Health");

    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerControl)
        {
            PlayerMovement();
            HandleRoll();
            HandleAttack();
            HandleUsingStairway();
        }
        Debug.Log(m_rollCount);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Climbable")
        {
            m_touchingClimbable = true;
        }
        else if (col.tag == "Stairway")
        {
            m_touchingStairway = true;
            m_stairwayDestionation = col.gameObject.GetComponent<Stairway>().GetDestination();
        }
        /* Section for taking Damage*/
        if (!m_roll)
        {
            if (col.tag == "EnemyProjectile")
            {
                TakeDamage(col.gameObject.GetComponent<Projectile>().damage);
                col.gameObject.GetComponent<Projectile>().Hit();
            }
            else if (col.tag == "Trap")
            {
                TakeDamage(col.gameObject.GetComponent<Trap>().damage);
            }
            else if (col.tag == "EnemyWeapon")
            {
                TakeDamage(col.gameObject.GetComponent<EnemyMeleeWeapon>().damage);
            }
            // after taking damage we need to know if we're dead.
            CheckIfShouldDie();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Stairway" && !m_touchingStairway)
        {
            m_touchingStairway = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Climbable")
        {
            m_touchingClimbable = false;
            m_isClimbing = false;
        }
        else if (col.tag == "Stairway")
        {
            m_touchingStairway = false;
        }
    }
    // Handles the players movement (Left and Right).
    private void PlayerMovement()
    {
        //current velocity
        Vector3 velocity = m_controller.velocity;
        //elilminate horizontal sliding
        velocity.x = 0;
        // if we're climbing eliminate vertical sliding
        if (m_isClimbing)
        {
            velocity.y = 0;
        }

        if (!m_roll && !m_meleeAttack)
        {
            // D runs right
            if (Input.GetKey(KeyCode.D))
            {
                velocity.x = movementSpeed;
                //used to determine direction of animation, and roll
                m_animator.setFacing("Right");
            }
            // A runs left
            else if (Input.GetKey(KeyCode.A))
            {


                velocity.x = -movementSpeed;

                //used to determine direction of animation, and roll
                m_animator.setFacing("Left");
            }
            // Space Jumps if player is on the ground or is on a climbable object
            if (Input.GetKeyDown(KeyCode.Space) && (m_controller.isGrounded || m_isClimbing))
            {
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                //jumping detaches us from climbable object.
                m_isClimbing = false;
            }

            if (Input.GetKey(KeyCode.S) && m_touchingClimbable)
            {
                if(!m_isClimbing)
                {
                    m_isClimbing = true;
                }
                velocity.y = -movementSpeed;

            }

            // climbing
            if (Input.GetKey(KeyCode.W) && m_touchingClimbable)
            {
                if (!m_isClimbing)
                {
                    m_isClimbing = true;
                }
                velocity.y = movementSpeed;
            }

            // idle animations
            m_animator.setAnimation("Idle");

        }
        //apply gravity if we're not climbing
        if (!m_isClimbing)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        // perform movement
        m_controller.move(velocity * Time.deltaTime);
    }

    // Checks if player wants to roll, if so rolls
    private void HandleRoll()
    {
        //// We only want to roll if we're on the ground, and there is no cooldown.
        //if (Input.GetKeyDown(KeyCode.LeftShift) && m_controller.isGrounded && m_rollCooldownTimestamp < Time.time)
        //{
        //    m_roll = true;
        //    m_rollCooldownTimestamp = Time.time + rollCooldown;
        //}
        //if (m_roll)
        //{
        //    Vector3 velocity = m_controller.velocity;
        //    m_rollTimer += Time.deltaTime * rollTime;
        //    if (m_animator.getFacing() == "Right")

        //    {
        //        velocity.x = rollSpeed;
        //    }
        //    else
        //    {
        //        velocity.x = -rollSpeed;
        //    }

        //    if (m_rollTimer > 1)
        //    {
        //        m_roll = false;
        //        m_rollTimer = 0f;
        //    }

        //    m_animator.setAnimation("Roll");

        //    //apply gravity
        //    velocity.y += gravity * Time.deltaTime;

        //    // perform movement
        //    m_controller.move(velocity * Time.deltaTime);

        //}
        //// We only want to roll if we're on the ground, and there is no cooldown.
        //if (Input.GetKeyDown(KeyCode.LeftShift) && m_controller.isGrounded && m_rollCooldownTimestamp < Time.time)
        //{
        //    m_roll = true;
        //    m_rollCooldownTimestamp = Time.time + rollCooldown;
        //}
        //if (m_roll)
        //{
        //    Vector3 velocity = m_controller.velocity;
        //    m_rollTimer += Time.deltaTime * rollTime;
        //    if (m_animator.getFacing() == "Right")

        //    {
        //        velocity.x = rollSpeed;
        //    }
        //    else
        //    {
        //        velocity.x = -rollSpeed;
        //    }

        //    if (m_rollTimer > 1)
        //    {
        //        m_roll = false;
        //        m_rollTimer = 0f;
        //    }

        //    m_animator.setAnimation("Roll");

        //    //apply gravity
        //    velocity.y += gravity * Time.deltaTime;

        //    // perform movement
        //    m_controller.move(velocity * Time.deltaTime);

        //}
        // We only want to roll if we're on the ground, and there is no cooldown.
        if (Input.GetKeyDown(KeyCode.LeftShift) && m_controller.isGrounded && m_rollCount > 0)
        {
            m_roll = true;
            m_rollCount--;
            // We create a timer that will increment the roll count variable after (rollCooldown) seconds
            // the timer then deletes itself once it has incremented the roll count.
            // TODO: add GUI update.
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                m_rollCount++;
                timer.Dispose();
            }, null, (int)(rollCooldown * 1000), System.Threading.Timeout.Infinite);
        }
        if (m_roll)
        {
            Vector3 velocity = m_controller.velocity;
            m_rollTimer += Time.deltaTime * rollTime;
            if (m_animator.getFacing() == "Right")

            {
                velocity.x = rollSpeed;
            }
            else
            {
                velocity.x = -rollSpeed;
            }

            if (m_rollTimer > 1)
            {
                m_roll = false;
                m_rollTimer = 0f;
            }

            m_animator.setAnimation("Roll");

            //apply gravity
            velocity.y += gravity * Time.deltaTime;

            // perform movement
            m_controller.move(velocity * Time.deltaTime);

        }
    }

    private void HandleAttack()
    {
        //shoot ranged (left click)
        if (Input.GetMouseButton(0))
        {
            m_rangedWeapon.Shoot(Time.time, m_animator.getFacing());
        }
        //swing melee (right click)
        else if (Input.GetMouseButtonDown(1) && !m_meleeAttack)
        {
            m_meleeAttack = true;
            m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
            if (m_meleeWeapon.Swing(Time.time))
            {
                m_animator.setAnimation("Melee");
            }
        }
        if (m_meleeAttack)
        {
            if (m_meleeTimer < Time.time)
            {
                m_meleeAttack = false;

                m_animator.setAnimation("Idle");
            }
        }

    }

    private void HandleUsingStairway()
    {
        if (m_touchingStairway && Input.GetKeyDown(KeyCode.E))
        {
            this.transform.position = m_stairwayDestionation;
        }
    }
    public void TakeDamage(float damage)
    {
        m_playerHealth -= damage;
        UpdateHealthUI();
    }

    private void CheckIfShouldDie()
    {
        if (m_playerHealth <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(0);
            // TODO: stuff when you've died
        }
    }
    private void UpdateHealthUI()
    {
        m_healthUI.GetComponent<Text>().text = "" + m_playerHealth;
    }
}

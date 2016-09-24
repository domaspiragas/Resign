using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {

    // Weapons
    //TODO: Get rid of these and just use array index 0 for starter weapons
    private MeleeWeapon m_meleeWeapon;
    private RangedWeapon m_rangedWeapon;
    //object for raycasting
    private CharacterController2D m_controller;
    //object for animations
    private AnimationController2D m_animator;
    // player's hitbox
    private BoxCollider2D m_playerHitBox;
    //bools for player state
    private bool m_playerControl = true;
    private bool m_roll = false;
    private bool m_crouch = false;
    private bool m_meleeAttack = false;

    //melee animation timer
    private float m_meleeTimer = 0;
    //roll cooldown
    private float m_rollTimer = 0;
    private float m_rollCooldownTimestamp;


    //reference to the main camera
    public GameObject playerCamera;
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;

    /* ADJUSTABLE IN UNITY VALUES */
    // movement
    public float movementSpeed = 6f;
    public float crouchMovementSpeed = 3f;

    // jump
    public float jumpHeight = 2f;
    public float crouchJumpHeight = 1f;

    // roll
    public float rollTime = 2f;
    public float rollSpeed = 0.5f;
    public float rollCooldown = 3f;

    public float gravity = -30f;

    public MeleeWeapon[] meleeWeapons = new MeleeWeapon[5];
    public RangedWeapon[] rangedWeapons = new RangedWeapon[5];
    
	// Use this for initialization
	void Start ()
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

	}
	
	// Update is called once per frame
	void Update ()
    {
        if(m_playerControl)
        {
            PlayerMovement();
            HandleRoll();
            HandleAttack();
        }

	}

    // Handles the players movement (Left and Right).
    private void PlayerMovement()
    {
        //current velocity
        Vector3 velocity = m_controller.velocity;
        //elilminate sliding
        velocity.x = 0;

        if (!m_roll && !m_meleeAttack)
        {
            // D runs right
            if (Input.GetKey(KeyCode.D))
            {
                // if we're not crouching move normal speed
                if (!m_crouch)
                {
                    velocity.x = movementSpeed;
                }
                else
                {
                    velocity.x = crouchMovementSpeed;
                }
                //used to determine direction of animation, and roll
                m_animator.setFacing("Right");
            }
            // A runs left
            else if(Input.GetKey(KeyCode.A))
            {
                // if we're not crouching move normal speed
                if (!m_crouch)
                {
                    velocity.x = -movementSpeed;
                }
                else
                {
                    velocity.x = -crouchMovementSpeed;
                }
                //used to determine direction of animation, and roll
                m_animator.setFacing("Left");
            }
            // Space Jumps if player is on the ground
            if(Input.GetKeyDown(KeyCode.Space) && m_controller.isGrounded)
            {
                // if we're not crouching jump normal height
                if (!m_crouch)
                {
                    velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                }
                else
                {
                    velocity.y = Mathf.Sqrt(2f * crouchJumpHeight * -gravity);
                }
            }

            if(Input.GetKeyDown(KeyCode.S) && m_controller.isGrounded)
            {
                m_crouch = true;
                m_animator.setAnimation("Crouch");
                // TODO: Check for a better way to adjust crouching hitbox. This way is not too bad
                this.transform.position = this.transform.position + new Vector3(0f, -.25f, 0f);
                m_playerHitBox.size = new Vector2(.5f, .5f);
            }
            if(Input.GetKeyUp(KeyCode.S))
            {
                m_crouch = false;
                // TODO: Check for a better way to adjust crouching hitbox. This way is not too bad
                this.transform.position = this.transform.position + new Vector3(0f, .25f, 0f);
                m_playerHitBox.size = new Vector2(.5f, 1f);
            }
                //here
            if (!m_crouch)
            {
                m_animator.setAnimation("Idle");
            }
            else
            {
                m_animator.setAnimation("Crouch");
            }
        }
        //apply gravity
        velocity.y += gravity * Time.deltaTime;
        // perform movement
        m_controller.move(velocity * Time.deltaTime);
    }

    // Checks if player wants to roll, if so rolls
    private void HandleRoll()
    {
        // We only want to roll if we're on the ground, and there is no cooldown.
        if(Input.GetKeyDown(KeyCode.LeftShift) && m_controller.isGrounded && m_rollCooldownTimestamp < Time.time)
        {
            m_roll = true;
            m_rollCooldownTimestamp = Time.time + rollCooldown;
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
        else if(Input.GetMouseButtonDown(1) && !m_meleeAttack)
        {
            m_meleeAttack = true;
            m_meleeTimer = Time.time + (m_meleeWeapon.attackDelay + m_meleeWeapon.attackDuration);
            if (m_meleeWeapon.Swing(Time.time))
            {
                m_animator.setAnimation("Melee");
            }
        }
        if(m_meleeAttack)
        {
            if (m_meleeTimer < Time.time)
            {
                m_meleeAttack = false;

                m_animator.setAnimation("Idle");
            }
        }

    }
}

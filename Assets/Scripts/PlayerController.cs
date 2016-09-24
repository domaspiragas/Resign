using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {


    private MeleeWeapon m_meleeWeapon;
    private RangedWeapon m_rangedWeapon;
    //object for raycasting
    private CharacterController2D m_controller;
    //object for animations
    private AnimationController2D m_animator;
    private bool m_playerControl = true;
    private bool m_roll = false;
    private float m_rollTimer = 0;
    private float m_rollCooldownTimestamp;


    //reference to the main camera
    public GameObject playerCamera;
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;

    public float runSpeed = 3f;
    public float jumpHeight = 2f;
    public float rollTime = 2f;
    public float rollSpeed = 0.5f;
    public float rollCooldown = 3f;
    public float gravity = -30f;
    
	// Use this for initialization
	void Start ()
    {

        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
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

        if (!m_roll)
        {
            // D runs right
            if (Input.GetKey(KeyCode.D))
            {
                velocity.x = runSpeed;
                //used to determine direction of animation, and roll
                m_animator.setFacing("Right");
            }
            // A runs left
            else if(Input.GetKey(KeyCode.A))
            {
                velocity.x = -runSpeed;
                //used to determine direction of animation, and roll
                m_animator.setFacing("Left");
            }
            // Space Jumps if player is on the ground
            if(Input.GetKeyDown(KeyCode.Space) && m_controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            }
            //apply gravity
            velocity.y += gravity * Time.deltaTime;
            // perform movement
            m_controller.move(velocity * Time.deltaTime);
            m_animator.setAnimation("Idle");
        }
    }

    // Checks if player wants to roll, if so rolls
    private void HandleRoll()
    {
        // We only want to roll if we're on the ground, and there is no cooldown.
        if(Input.GetKeyDown(KeyCode.LeftAlt) && m_controller.isGrounded && m_rollCooldownTimestamp < Time.time)
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
        else if(Input.GetMouseButtonDown(1))
        {
            m_meleeWeapon.Swing(Time.time);
        }
    }
}

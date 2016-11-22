using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prime31;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Weapons
    //Melee
    private StarterBagWeapon m_starterBagWeapon;
    private MopWeapon m_mopWeapon;
    //Ranged
    private MailBagWeapon m_mailBagWeapon;
    private RangedWeapon m_starterWeapon;

    //Keep track of whether or not we own a weapon
    private bool[] m_ownMeleeWeapon = { true, true, false, false };
    private bool[] m_ownRangedWeapon = { true, true, false, false };
    // Our current weapon
    private int m_curMeleeWeapon = 0;
    private int m_curRangedWeapon = 0;
    // if true we're changing melee, if false we're changing ranged
    private bool m_toggleMelee = true;
    //health/roll/lives ui
    private GameObject m_livesUI;
    private GameObject m_meleeUI;
    private GameObject m_rangedUI;
    private GameObject m_playerHealthText;
    //object for raycasting
    private PlayerCharacterController2D m_controller;
    //object for animations
    private AnimationController2D m_animator;
    // player's hitbox
    private BoxCollider2D m_playerHitBox;
    //track player's health inside class
    private float m_playerHealth;
    //track player's lives inside class
    private int m_lives = 3;
    //current respawn point
    private Vector3 m_respawnPoint;
    //bools for player state
    private bool m_playerControl = true;
    private bool m_roll = false;
    private bool m_meleeAttack = false;
    private bool m_touchingClimbable = false;
    private bool m_touchingStairway = false;
    private bool m_isClimbing = false;
    private bool m_redFlash = false;

    //melee animation timer
    private float m_meleeTimer = 0;
    //timer for red take damage flash
    private float m_redFlashTimer = 0;
    //roll cooldown
    private float m_rollTimer = 0;
    private float m_rollCooldownTimestamp;
    private float m_rollCount;

    private Vector3 m_stairwayDestionation;

    //Current Animations TODO: UPDATE TO real starting animation names
    private string m_idleAnim = "Idle";
    private string m_rollAnim = "OldRoll";
    private string m_leftWalkAnim;
    private string m_rightWalkAnim;
    private string m_meleeAnim;
    private string m_rangeAnim;
    private string m_climbAnim;

    // reference to the main camera
    public GameObject playerCamera;
    // player's health bar
    public GameObject healthBar;
    // the UI stamina orbs
    public GameObject[] staminaOrbs = new GameObject[3];
    // Weapon UI 
    public GameObject meleeWeaponUI;
    public GameObject rangedWeaponUI;
    /* ADJUSTABLE IN UNITY VALUES */

    public float maxHealth = 100f;
    // movement
    public float movementSpeed = 6f;
    public float airMovementVal = 1f;

    // jump
    public float jumpHeight = 2f;

    // roll
    public float rollTime = 2f;
    public float rollSpeed = 0.5f;
    public float rollCooldown = 3f;
    public int maxRollCount = 3;

    public float gravity = -30f;

    //All of the weapons
    public GameObject[] meleeWeapons = new GameObject[4];
    public GameObject[] rangedWeapons = new GameObject[4];

    // Use this for initialization
    void Start()
    {

        m_controller = gameObject.GetComponent<PlayerCharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_playerHitBox = gameObject.GetComponent<BoxCollider2D>();
        // weapons loaded in
        //melee
        m_starterBagWeapon = (StarterBagWeapon)meleeWeapons[0].GetComponent(typeof(StarterBagWeapon));
        m_mopWeapon = (MopWeapon)meleeWeapons[1].GetComponent(typeof(MopWeapon));
        //ranged
        m_starterWeapon = (RangedWeapon)rangedWeapons[0].GetComponent(typeof(RangedWeapon));
        m_mailBagWeapon = (MailBagWeapon)rangedWeapons[1].GetComponent(typeof(MailBagWeapon));
        //attach camera and start following our player
        playerCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);
        m_rollCooldownTimestamp = Time.time;
        m_playerHealth = maxHealth;
        m_rollCount = maxRollCount;
        //get the health ui object
        m_livesUI = GameObject.Find("Lives");
        m_meleeUI = GameObject.Find("Melee");
        m_rangedUI = GameObject.Find("Ranged");
        m_playerHealthText = GameObject.Find("PlayerHealth");
        //make sure the correct health/roll values are displayed. 
        UpdateHealthUI();
        UpdateRollUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerControl)
        {
            PlayerMovement();
            HandleRoll();
            HandleAttack();
            HandleInteract();
            HandleToggleChangeWeapon();
            HandleChangeWeapon();
            HandleRedDamageFlash();
        }
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
        else if (col.tag == "HealthPickUp")
        {
            // if we are damaged pick up the health drop.
            if (m_playerHealth < maxHealth)
            {
                PickUpHealth(col.gameObject.GetComponent<HealthPickUp>().healthValue);
                col.gameObject.GetComponent<HealthPickUp>().PickUp();
            }
        }
        else if (col.tag == "LifePickUp")
        {
            PickUpLife();
            col.gameObject.GetComponent<LifePickUp>().PickUp();
        }
        else if (col.tag == "RespawnPoint")
        {
            m_respawnPoint = col.gameObject.GetComponent<RespawnPoint>().GetPosition();
        }
        /* Section for taking Damage*/
        if (!m_roll)
        {
            //take intern damage
            if (col.tag == "InternEnemyProjectile")
            {
                TakeDamage(col.gameObject.GetComponent<InternEnemyProjectile>().damage);
                col.gameObject.GetComponent<InternEnemyProjectile>().Hit();
            }
            else if (col.tag == "InternMeleeWeapon")
            {
                TakeDamage(col.gameObject.GetComponent<InternMeleeWeapon>().damage);
            }
            // take rat damage
            else if (col.tag == "RatMeleeWeapon")
            {
                TakeDamage(col.gameObject.GetComponent<RatMeleeWeapon>().damage);
            }
            //take mailman damage
            else if (col.tag == "MailmanProjectile")
            {
                TakeDamage(col.gameObject.GetComponent<MailmanEnemyProjectile>().damage);
                col.gameObject.GetComponent<MailmanEnemyProjectile>().Hit();
            }
            // take mailman mini boss damage
            else if (col.tag == "PackageProjectile")
            {
                TakeDamage(col.gameObject.GetComponent<PackageProjectile>().damage);
                col.gameObject.GetComponent<PackageProjectile>().Hit();
            }
            else if (col.tag == "MailmanMiniBossBagProjectile")
            {
                TakeDamage(col.gameObject.GetComponent<MailmanMiniBossBagProjectile>().damage);
                col.gameObject.GetComponent<MailmanMiniBossBagProjectile>().Hit();
            }
            else if (col.tag == "Trap")
            {
                TakeDamage(col.gameObject.GetComponent<Trap>().damage);
            }
            else if (col.tag == "JanitorTrap")
            {
                TakeDamage(col.gameObject.GetComponent<JanitorTrap>().damage);
            }
            else if (col.tag == "JanitorBoss")
            {
                // when touching the boss TODO: change so charge etc only do damage 
                TakeDamage(10);
            }
            else if (col.tag == "JanitorBossMelee")
            {
                TakeDamage(col.gameObject.GetComponent<JanitorMeleeWeapon>().damage);
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
        if (m_controller.isGrounded || m_isClimbing)
        {
            velocity.x = 0;
        }
        // if we're climbing eliminate vertical sliding
        if (m_isClimbing)
        {
            velocity.y = 0;
        }

        if (!m_roll)
        {
            if (!m_meleeAttack)
            {
                // D runs right
                if (Input.GetKey(KeyCode.D))
                {
                    if (m_controller.isGrounded || m_isClimbing)
                    {
                        velocity.x = movementSpeed;
                    }
                    else
                    {
                        if (velocity.x < movementSpeed)
                        {
                            velocity.x += airMovementVal;
                        }
                    }
                    //used to determine direction of animation, and roll
                    m_animator.setFacing("Right");
                }
                // A runs left
                else if (Input.GetKey(KeyCode.A))
                {

                    if (m_controller.isGrounded || m_isClimbing)
                    {
                        velocity.x = -movementSpeed;
                    }
                    else
                    {
                        if (velocity.x > -movementSpeed)
                        {
                            velocity.x -= airMovementVal;

                        }
                    }

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
                    if (!m_isClimbing)
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
                m_animator.setAnimation(m_idleAnim);
            }
            // Move while attacking in air.
            else if (!m_controller.isGrounded)
            {
                // D runs right
                if (Input.GetKey(KeyCode.D))
                {
                    if (velocity.x < movementSpeed)
                    {
                        velocity.x += airMovementVal;
                    }
                    //used to determine direction of animation, and roll
                    m_animator.setFacing("Right");
                }
                // A runs left
                else if (Input.GetKey(KeyCode.A))
                {
                    if (velocity.x > -movementSpeed)
                    {
                        velocity.x -= airMovementVal;
                    }
                    //used to determine direction of animation, and roll
                    m_animator.setFacing("Left");
                }
            }
            //apply gravity if we're not climbing
            if (!m_isClimbing)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            // perform movement
            m_controller.move(velocity * Time.deltaTime);
        }
    }

    // Checks if player wants to roll, if so rolls
    private void HandleRoll()
    {
        // We only want to roll if we're on the ground, and there is no cooldown.
        if (Input.GetKeyDown(KeyCode.LeftShift) && m_controller.isGrounded && m_rollCount > 0 && !m_roll)
        {
            m_roll = true;
            m_rollCount--;
            UpdateRollUI();
            m_rollCooldownTimestamp = Time.time + rollCooldown;
        }
        if (m_roll)
        {
            Vector3 velocity = m_controller.velocity;
            m_rollTimer += Time.deltaTime * rollTime;
            if (m_controller.isGrounded)
            {
                if (m_animator.getFacing() == "Right")

                {
                    velocity.x = rollSpeed;
                }
                else
                {
                    velocity.x = -rollSpeed;
                }


            }
            if (m_rollTimer > 1)
            {
                m_roll = false;
                m_rollTimer = 0f;
            }

            m_animator.setAnimation(m_rollAnim);

            velocity.y += gravity * Time.deltaTime;
            // perform movement
            m_controller.move(velocity * Time.deltaTime);

        }
        if (Time.time >= m_rollCooldownTimestamp && m_rollCount < maxRollCount)
        {
            m_rollCount++;
            UpdateRollUI();
            if (m_rollCount < maxRollCount)
            {
                m_rollCooldownTimestamp = Time.time + rollCooldown;
            }
        }
    }

    private void HandleAttack()
    {
        // don't attack if we're rolling
        if (!m_roll)
        {
            //shoot ranged (left click)
            if (Input.GetMouseButton(0))
            {
                switch (m_curRangedWeapon)
                {
                    //Starter
                    case 0:
                        m_starterWeapon.Shoot(Time.time, m_animator.getFacing());
                        break;
                    //MailBag
                    case 1:
                        m_mailBagWeapon.Shoot(Time.time, m_animator.getFacing());
                        break;
                    //Samsung Galaxy Note7
                    case 2:
                        break;
                    //TBD
                    case 3:
                        break;
                }
                //  m_rangedWeapon.Shoot(Time.time, m_animator.getFacing());
            }
            //swing melee (right click)
            else if (Input.GetMouseButtonDown(1) && !m_meleeAttack)
            {
                m_meleeAttack = true;
                switch (m_curMeleeWeapon)
                {

                    //Bag Weapon
                    case 0:
                        m_meleeTimer = Time.time + (m_starterBagWeapon.attackDelay + m_starterBagWeapon.attackDuration);
                        if (m_starterBagWeapon.Swing(Time.time))
                        {
                            m_animator.setAnimation("Melee");
                        }
                        break;
                    //Mop Weapon
                    case 1:
                        m_meleeTimer = Time.time + (m_mopWeapon.attackDelay + m_mopWeapon.attackDuration);
                        if (m_mopWeapon.Swing(Time.time))
                        {
                            m_animator.setAnimation("Melee");
                        }
                        break;
                    //Mouse Whip
                    case 2:
                        break;
                    //TBD
                    case 3:
                        break;
                }

            }
            if (m_meleeAttack)
            {
                if (m_meleeTimer < Time.time)
                {
                    m_meleeAttack = false;

                    m_animator.setAnimation(m_idleAnim);
                }
            }
        }
    }


    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (m_touchingStairway)
            {
                this.transform.position = m_stairwayDestionation;
            }
            //TODO: put the pickup weapons/loot chest stuff here.
        }


    }
    public void TakeDamage(float damage)
    {
        m_playerHealth -= damage;
        UpdateHealthUI();
        // flash red and start timer
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        m_redFlash = true;
        m_redFlashTimer = 0;
    }

    public void PickUpHealth(float health)
    {
        // avoid getting more than max health
        if (m_playerHealth + health > maxHealth)
        {
            m_playerHealth = maxHealth;
        }
        else
        {
            m_playerHealth += health;
        }
        UpdateHealthUI();
    }
    public void PickUpLife()
    {
        m_lives++;
        UpdateLivesUI();
    }

    private void CheckIfShouldDie()
    {
        if (m_playerHealth <= 0)
        {
            m_lives--;
            if (m_lives < 0)
            {
                //TODO: Game Over Screen
                SceneManager.LoadScene(0);
            }
            else
            {
                RespawnPlayer();
            }
            //Destroy(gameObject);
            //SceneManager.LoadScene(0);
            // TODO: stuff when you've died
        }
    }
    // Swap from changing melee to changing ranged.
    private void HandleToggleChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_toggleMelee = !m_toggleMelee;
            UpdateToggleUI();
        }
    }
    private void HandleChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeWeapon(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeWeapon(false);
        }
    }
    // handles flashing red when damage has been taken
    private void HandleRedDamageFlash()
    {

        if (m_redFlash)
        {
            if (m_redFlashTimer > .10f)
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                m_redFlash = false;
            }
            m_redFlashTimer += Time.deltaTime;
        }
    }
    // The parameter determines whether you're cycling right or not through the list
    private void ChangeWeapon(bool right)
    {
        // Change melee weapon
        if (m_toggleMelee)
        {
            int temp = m_curMeleeWeapon;
            if (right)
            {
                // Look for the next weapon we have to the right
                while (!m_ownMeleeWeapon[temp + 1] && temp + 1 <= 3)
                {
                    if (temp + 1 == 3)
                    {
                        // if we're at the end with no weapon found we just equip the starting weapon
                        m_curMeleeWeapon = 0;
                        LoadWeapon(0);
                        return;
                    }
                    temp++;
                }
                // Equip the new weapon
                m_curMeleeWeapon = temp + 1;
                LoadWeapon(temp + 1);
            }
            else
            {
                if (temp == 0)
                {
                    temp = 4;
                }
                // Look for the next weapon we have to the left
                while (!m_ownMeleeWeapon[temp - 1] && temp - 1 >= 0)
                {
                    temp--;
                }
                m_curMeleeWeapon = temp - 1;
                LoadWeapon(temp - 1);
            }
        }
        //Change Ranged Weapon
        else
        {
            int temp = m_curRangedWeapon;
            if (right)
            {
                // Look for the next weapon we have to the right
                while (!m_ownRangedWeapon[temp + 1] && temp + 1 <= 3)
                {
                    if (temp + 1 == 3)
                    {
                        // if we're at the end with no weapon found we just equip the starting weapon
                        m_curRangedWeapon = 0;
                        LoadWeapon(0);
                        return;
                    }
                    temp++;
                }
                // Equip the new weapon
                m_curRangedWeapon = temp + 1;

                LoadWeapon(temp + 1);
            }
            else
            {
                if (temp == 0)
                {
                    temp = 4;
                }
                // Look for the next weapon we have to the left
                while (!m_ownRangedWeapon[temp - 1] && temp - 1 >= 0)
                {
                    temp--;
                }
                m_curRangedWeapon = temp - 1;
                LoadWeapon(temp - 1);
            }
        }
    }
    // For loading animations
    private void LoadWeapon(int index)
    {
        // Change Melee Weapon
        if (m_toggleMelee)
        {
            UpdateMeleeUI();
            switch (index)
            {
                //ShoulderBag
                case 0:
                    break;
                //Mop
                case 1:
                    break;
                //TBD
                case 2:
                    break;
                //Mouse
                case 3:
                    break;
            }
        }
        //Change Ranged Weapon
        else
        {
            UpdateRangedUI();
            switch (index)
            {
                //Starter
                case 0:
                    break;
                //MailBag
                case 1:
                    break;
                //TBD
                case 2:
                    break;
                //TBD
                case 3:
                    break;
            }
        }
    }

    private void RespawnPlayer()
    {
        m_playerHealth = maxHealth;
        UpdateHealthUI();
        UpdateLivesUI();
        this.transform.position = m_respawnPoint;
    }
    private void UpdateHealthUI()
    {
        healthBar.transform.localScale = new Vector3(m_playerHealth / maxHealth, healthBar.transform.localScale.y, 0);
        m_playerHealthText.GetComponent<Text>().text = "" + m_playerHealth + " / " + maxHealth;
    }
    // Sets the orb to active depending on how many rolls the player has available. 
    private void UpdateRollUI()
    {
        if (m_rollCount == 3)
        {
            staminaOrbs[0].SetActive(true);
            staminaOrbs[1].SetActive(true);
            staminaOrbs[2].SetActive(true);
        }
        else if (m_rollCount == 2)
        {
            staminaOrbs[0].SetActive(true);
            staminaOrbs[1].SetActive(true);
            staminaOrbs[2].SetActive(false);
        }
        else if (m_rollCount == 1)
        {
            staminaOrbs[0].SetActive(true);
            staminaOrbs[1].SetActive(false);
            staminaOrbs[2].SetActive(false);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                staminaOrbs[i].SetActive(false);
            }
        }
    }
    private void UpdateLivesUI()
    {
        m_livesUI.GetComponent<Text>().text = "x" + m_lives;
    }
    private void UpdateRangedUI()
    {
        //m_rangedUI.GetComponent<Text>().text = "R" + m_curRangedWeapon;
        rangedWeaponUI.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = rangedWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().weapons[m_curRangedWeapon];
    }
    private void UpdateMeleeUI()
    {
        //m_meleeUI.GetComponent<Text>().text = "M" + m_curMeleeWeapon;
        meleeWeaponUI.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = meleeWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().weapons[m_curMeleeWeapon];
    }
    private void UpdateToggleUI()
    {
        if (m_toggleMelee)
        {
            meleeWeaponUI.gameObject.GetComponent<Image>().sprite = meleeWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().selectedBackground;
            rangedWeaponUI.gameObject.GetComponent<Image>().sprite = rangedWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().unselectedBackground;
        }
        else
        {
            // set melee box to unselected
            meleeWeaponUI.gameObject.GetComponent<Image>().sprite = meleeWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().unselectedBackground;
            // set ranged box to selected
            rangedWeaponUI.gameObject.GetComponent<Image>().sprite = rangedWeaponUI.gameObject.GetComponent<WeaponUISpriteStorage>().selectedBackground;
        }
    }
    public bool IsClimbing()
    {
        return m_isClimbing;
    }
}

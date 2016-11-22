using UnityEngine;
using System.Collections;
using Prime31;

public class MailmanMiniBoss : MonoBehaviour
{

    public float health = 300f;
    public float speed = 2f;
    public float chanceOfHealthDrop = 100f;
    public GameObject mailBagRangedWeapon;
    public GameObject packageRangedWeapon;
    public GameObject healthPickUp;
    public GameObject healthBar;
    public GameObject weaponPrefab;

    private MailmanMiniBossPackageWeapon m_packageWeapon;
    private MailmanMinibossBagWeapon m_bagWeapon;
    private CharacterController2D m_controller;
    private AnimationController2D m_animator;

    private bool m_followPlayer = false;
    private Vector3 m_startingPosition;
    private Vector3 m_playerPosition;
    private bool m_moveRight;

    private GameObject m_healthUI;

    private float m_health;

    // for handling flashing red when taking damage
    private bool m_redFlash;
    private float m_redFlashTimer;
    // Use this for initialization
    void Start()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_animator = gameObject.GetComponent<AnimationController2D>();
        m_packageWeapon = (MailmanMiniBossPackageWeapon)packageRangedWeapon.GetComponent(typeof(MailmanMiniBossPackageWeapon));
        m_bagWeapon = (MailmanMinibossBagWeapon)mailBagRangedWeapon.GetComponent(typeof(MailmanMinibossBagWeapon));
        m_health = health;
        m_startingPosition = this.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        CheckIfDead();

        Vector3 velocity = m_controller.velocity;
        velocity.x = 0;

        if (m_followPlayer)
        {
            float positionDifference = this.transform.position.x - m_playerPosition.x;

            // our position - palyer position, if positive we're to the right of the palyer else we're to the left
            // if the player is farther away ranged attack
            if (positionDifference > 5f)
            {

                m_animator.setFacing("Left");
                m_packageWeapon.Shoot(Time.time, m_playerPosition);
                
            }
            else if (positionDifference < -5f)
            {
                m_animator.setFacing("Right");
                m_packageWeapon.Shoot(Time.time, m_playerPosition);
               
            }
            else if (positionDifference >0 && positionDifference <=5)
            {
                m_animator.setFacing("Left");
                m_bagWeapon.Shoot(Time.time, "Left");
            }
            else if (positionDifference >= -5 && positionDifference <= 0)
            {
                m_animator.setFacing("Right");
                m_bagWeapon.Shoot(Time.time, "Right");
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
            DropWeapon();
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

    private void DropWeapon()
    {
        GameObject weapon = (GameObject)Instantiate(weaponPrefab, this.transform.position, Quaternion.identity);
        weapon.gameObject.GetComponent<WeaponPickup>().AssignLobDirection(Random.Range(-.3f, .3f));
    }
    private void UpdateHealthUI()
    {
        healthBar.transform.localScale = new Vector3((m_health / health), healthBar.transform.localScale.y, 0);
    }
}

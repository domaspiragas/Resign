using UnityEngine;
using System.Collections;

public class StationaryEnemy : MonoBehaviour
{

    public float health = 50;
    public GameObject rangedWeapon;
    private RangedWeapon m_rangedWeapon;

    private bool m_shooting = false;
    private float m_health;

    private Vector3 m_playerPosition;
    // Use this for initialization
    void Start()
    {
        m_rangedWeapon = (RangedWeapon)rangedWeapon.GetComponent(typeof(RangedWeapon));
        m_health = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_health <= 0)
        {
            Destroy(gameObject);
        }
        if(m_shooting)
        {
            m_rangedWeapon.EnemyShoot(Time.time, m_playerPosition);
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        m_playerPosition = position;
    }
    public void TakeDamage(float damage)
    {
        m_health -= damage;
    }
    public void SetShooting(bool shooting)
    {
        m_shooting = shooting;
    }
}

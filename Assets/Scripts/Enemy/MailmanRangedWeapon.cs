using UnityEngine;
using System.Collections;

public class MailmanRangedWeapon : MonoBehaviour {

    // The projectile it fires
    public GameObject projectilePrefab;

    // how often if fires while holding
    public float attackRate;
    // speed of the projectile
    public float projectileSpeed;

    private float m_previousShotTime;

    void Awake()
    {
        m_previousShotTime = Time.time - attackRate;
    }

    public void EnemyShoot(float currentTime, Vector3 playerPosition)
    {
        if (currentTime - m_previousShotTime > attackRate)
        {
            GameObject projectileFast = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            GameObject projectileSlow = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            float direction = this.transform.position.x - playerPosition.x;
            if (direction < 0)
            {
                projectileFast.GetComponent<Rigidbody2D>().velocity += new Vector2(projectileSpeed, 0f);
                projectileSlow.GetComponent<Rigidbody2D>().velocity += new Vector2(projectileSpeed * .75f, 0f);
            }
            else
            {
                projectileFast.GetComponent<Rigidbody2D>().velocity += new Vector2(-projectileSpeed, 0f);
                projectileSlow.GetComponent<Rigidbody2D>().velocity += new Vector2(-projectileSpeed * .75f, 0f);
            }
            m_previousShotTime = Time.time;
        }

    }
}

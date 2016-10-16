using UnityEngine;
using System.Collections;

public class MailBagWeapon : MonoBehaviour {

    // The projectile it fires
    public GameObject projectilePrefab;

    // how often if fires while holding
    public float attackRate;
    // speed of the projectile
    public float projectileSpeed;
    public float projectileUpwardSpread;
    public float projectileDownwardSpread;

    private float m_previousShotTime;

    void Start()
    {
        m_previousShotTime = Time.time;
    }

    // Gets the time the method was called, and the direction the character is facing
    public void Shoot(float currentTime, string direction)
    {
        if (currentTime - m_previousShotTime > attackRate)
        {
            GameObject projectileTop = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            GameObject projectileMid = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            GameObject projectileBot = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            if (direction == "Right")
            {
                projectileTop.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileSpeed, projectileUpwardSpread);
                projectileMid.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileSpeed, 0f);
                projectileBot.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileSpeed, -projectileDownwardSpread);


            }
            else
            {
                projectileTop.GetComponent<Rigidbody2D>().velocity = new Vector3(-projectileSpeed, projectileUpwardSpread);
                projectileMid.GetComponent<Rigidbody2D>().velocity = new Vector3(-projectileSpeed, 0f);
                projectileBot.GetComponent<Rigidbody2D>().velocity = new Vector3(-projectileSpeed, -projectileDownwardSpread);
            }
            m_previousShotTime = Time.time;
        }
    }
}

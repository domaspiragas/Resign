using UnityEngine;
using System.Collections;

public class MailmanMiniBossPackageWeapon : MonoBehaviour {

    // The projectile it fires
    public GameObject projectilePrefab;

    // how often if fires while holding
    public float attackRate;

    private float m_previousShotTime;

    void Awake()
    {
        m_previousShotTime = Time.time - attackRate;
    }

    public void Shoot(float currentTime, Vector3 playerPosition)
    {
        if (currentTime - m_previousShotTime > attackRate)
        {
            // thrown toward the player's position. 
            GameObject projectile1 = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
            // mid point of start and end X plus more Y
            Vector3 controlPosition = new Vector3((this.transform.position.x + playerPosition.x) / 2, this.transform.position.y + 10, 0);
            projectile1.gameObject.GetComponent<PackageProjectile>().SetBezeierInformation(1.5f, this.transform.position, playerPosition, controlPosition);

            int rand = Random.Range(1, 11);
            if (rand < 4)
            {
                GameObject projectile2 = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);

                Vector3 controlPosition2 = new Vector3((this.transform.position.x + playerPosition.x) / 2, this.transform.position.y + 8, 0);
                // offset location package is thrown.
                projectile2.gameObject.GetComponent<PackageProjectile>().SetBezeierInformation(1.25f, this.transform.position, playerPosition+new Vector3(5,0,0),
                    controlPosition2);
            }
            if (rand < 7)
            {
                GameObject projectile3 = (GameObject)Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);

                Vector3 controlPosition3 = new Vector3((this.transform.position.x + playerPosition.x) / 2, this.transform.position.y + 9, 0);
                // offset location package is thrown.
                projectile3.gameObject.GetComponent<PackageProjectile>().SetBezeierInformation(1f, this.transform.position, playerPosition + new Vector3(-3, 0, 0),
                    controlPosition3);
            }
            m_previousShotTime = Time.time;
        }

    }
}

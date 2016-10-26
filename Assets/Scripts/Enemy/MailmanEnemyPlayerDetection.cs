using UnityEngine;
using System.Collections;

public class MailmanEnemyPlayerDetection : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanEnemy>().SetFollowPlayer(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanEnemy>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanEnemy>().SetFollowPlayer(false);

        }

    }
}

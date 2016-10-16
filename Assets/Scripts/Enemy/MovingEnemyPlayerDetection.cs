using UnityEngine;
using System.Collections;

public class MovingEnemyPlayerDetection : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MovingEnemy>().SetFollowPlayer(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MovingEnemy>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MovingEnemy>().SetFollowPlayer(false);

        }

    }
}

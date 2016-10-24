using UnityEngine;
using System.Collections;

public class RatEnemyPlayerDetection : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<RatEnemy>().SetFollowPlayer(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<RatEnemy>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<RatEnemy>().SetFollowPlayer(false);

        }

    }
}

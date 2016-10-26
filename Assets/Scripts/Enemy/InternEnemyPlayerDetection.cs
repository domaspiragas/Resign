using UnityEngine;
using System.Collections;

public class InternEnemyPlayerDetection : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<InternEnemy>().SetFollowPlayer(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<InternEnemy>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<InternEnemy>().SetFollowPlayer(false);

        }

    }
}

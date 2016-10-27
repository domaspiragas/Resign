using UnityEngine;
using System.Collections;

public class JanitorPlayerDetection : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<JanitorBoss>().SetPlayerPosition(col.transform.position);
            transform.parent.gameObject.GetComponent<JanitorBoss>().SetSeePlayer();
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<JanitorBoss>().SetPlayerPosition(col.transform.position);
        }
    }
}

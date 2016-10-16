using UnityEngine;
using System.Collections;

public class PlayerDetection : MonoBehaviour {


    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<StationaryEnemy>().SetShooting(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<StationaryEnemy>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<StationaryEnemy>().SetShooting(false);

        }

    }
}

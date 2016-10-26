using UnityEngine;
using System.Collections;

public class MailmanMiniBossDetection : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanMiniBoss>().SetFollowPlayer(true);
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanMiniBoss>().SetPlayerPosition(col.transform.position);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.gameObject.GetComponent<MailmanMiniBoss>().SetFollowPlayer(false);

        }

    }
}

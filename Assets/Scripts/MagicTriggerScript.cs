using UnityEngine;
using System.Collections;

public class MagicTriggerScript : MonoBehaviour {

    public GameObject block;
    public GameObject text;

    void OnTriggerEnter2D(Collider2D col)
    {
        block.gameObject.GetComponent<Renderer>().enabled = true;
        text.gameObject.GetComponent<Renderer>().enabled = true;
    }
	// Use this for initialization
	void Start ()
    {
        block.gameObject.GetComponent<Renderer>().enabled = false;
        text.gameObject.GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

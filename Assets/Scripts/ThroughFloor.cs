using UnityEngine;
using System.Collections;

public class ThroughFloor : MonoBehaviour {

    public GameObject player;
    public GameObject platform;

    private PlayerController m_player;


	// Use this for initialization
	void Start ()
    {
        m_player = player.gameObject.GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}

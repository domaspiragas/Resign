using UnityEngine;
using System.Collections;

public class MagicButtonScript : MonoBehaviour {

    public GameObject spawner1, spawner2, wall, gameCamera;
    bool m_buttonOnce = true;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (m_buttonOnce)
        {
            gameCamera.gameObject.GetComponent<Camera>().backgroundColor = Color.red;
            wall.gameObject.GetComponent<Renderer>().enabled = true;
            wall.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            spawner1.gameObject.GetComponent<EndlessSpawner>().SetButtonPressed(true);
            spawner2.gameObject.GetComponent<EndlessSpawner>().SetButtonPressed(true);
            m_buttonOnce = false;
        }
    }
	// Use this for initialization
	void Start ()
    {
        wall.gameObject.GetComponent<Renderer>().enabled = false;
        wall.gameObject.GetComponent<BoxCollider2D>().enabled = false;
	}
	
	// Update is called once per frame
	void Update ()

    {
	
	}
}

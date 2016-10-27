using UnityEngine;
using System.Collections;
using Prime31;

public class JanitorTrap : MonoBehaviour {

    public float damage;
    public float lifeSpan;
    private float m_lifeSpan;
    private CharacterController2D m_controller;

	// Use this for initialization
	void Start ()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
        m_lifeSpan = lifeSpan;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_lifeSpan -= Time.deltaTime;
        if (m_lifeSpan <= 0)
        {
            Destroy(gameObject);
        }
        m_controller.move(new Vector3(0, -50, 0));
	}
}

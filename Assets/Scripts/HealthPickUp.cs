using UnityEngine;
using System.Collections;
using Prime31;

public class HealthPickUp : MonoBehaviour {

    CharacterController2D m_controller;

    public float healthValue = 10f;
    private float m_horizontalLobPower = 0f;
	// Use this for initialization
	void Start ()
    {
        m_controller = gameObject.GetComponent<CharacterController2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // drop the health to the ground. 
        if (!m_controller.isGrounded)
        {
            Vector3 velocity = m_controller.velocity;
            velocity.x += m_horizontalLobPower;
            velocity.y += -50 * Time.deltaTime;
            m_controller.move(velocity * Time.deltaTime);
            
        }
    }

    public void PickUp()
    {
        Destroy(gameObject);
    }
    public void AssignLobDirection(float horzontalPower)
    {
        m_horizontalLobPower = horzontalPower;

    }
}

using UnityEngine;
using System.Collections;

public class PackageProjectile : MonoBehaviour {

    private Vector3 m_startPosition, m_endPosition, m_controlPosition;
    // time the lob lasts
    private float m_time;
    private float m_currentDuration = 0f;
    // if we should be moving
    private bool m_move;

    public float damage;
    
	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PhysicalObject")
        {
            Hit();
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (m_move)
        {
            this.transform.position = CalculateBezierPoint(m_currentDuration/m_time, m_startPosition, m_endPosition, m_controlPosition);
            m_currentDuration += Time.deltaTime;
        }
	
	}
    private Vector3 CalculateBezierPoint(float t, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint)
    {
        float u = 1 - t;
        float uu = u * u;

        Vector3 point = uu * startPosition;
        point += 2 * u * t * controlPoint;
        point += t * t * endPosition;

        return point;
    }

    public void SetBezeierInformation(float time, Vector3 startPosition, Vector3 endPosition, Vector3 controlPosition)
    {
        m_time = time;
        m_startPosition = startPosition;
        m_endPosition = endPosition;
        m_controlPosition = controlPosition;
        m_move = true;
    }

    public void Hit()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class EndlessSpawner : MonoBehaviour {

    public GameObject enemyPrefab;
    private bool m_buttonPressed = false;
	
	// Update is called once per frame
	void Update ()
    {
	    if(m_buttonPressed)
        {
            InvokeRepeating("SpawnEnemy", 1, .5f);
            m_buttonPressed = false;
        }
	}
    private void SpawnEnemy()
    {
        GameObject enemy = (GameObject)Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
    }
    public void SetButtonPressed(bool presseed)
    {
        m_buttonPressed = presseed;
    }
}

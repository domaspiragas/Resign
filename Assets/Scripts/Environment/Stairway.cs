using UnityEngine;
using System.Collections;

public class Stairway : MonoBehaviour {

    public GameObject destination;

    public Vector3 GetDestination()
    {
        return destination.transform.position;
    }
}

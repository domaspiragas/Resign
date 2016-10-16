using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour
{
    public Vector3 GetPosition()
    {
        return new Vector3 (this.transform.position.x, this.transform.position.y, -1f);
    }
}

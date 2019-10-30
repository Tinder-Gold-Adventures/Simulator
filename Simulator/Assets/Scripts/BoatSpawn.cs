using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSpawn : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(8, 8, 8));
    }
}

using UnityEngine;

public class TrainSpawn : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(8, 8, 8));
    }
}

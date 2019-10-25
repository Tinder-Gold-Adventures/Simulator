using UnityEngine;

public class WaypointTrains : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1.5f);
    }
}

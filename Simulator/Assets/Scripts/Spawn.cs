using UnityEngine;

public class Spawn : MonoBehaviour
{
    public TrafficType VehicleType;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        switch (VehicleType)
        {
            case TrafficType.Car:
                Gizmos.color = Color.red;
                break;

            case TrafficType.Train:
                Gizmos.color = Color.yellow;
                break;

            case TrafficType.Boat:
                Gizmos.color = Color.green;
                break;

            case TrafficType.Bicycle:
                Gizmos.color = Color.cyan;
                break;

            case TrafficType.Passenger:
                Gizmos.color = Color.white;
                break;
        }

        Gizmos.DrawCube(transform.position, new Vector3(8,8,8));
    }
}

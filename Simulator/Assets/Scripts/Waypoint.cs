using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public VehicleType VehicleType;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        switch (VehicleType)
        {
            case VehicleType.Car:
                Gizmos.color = Color.red;
                break;

            case VehicleType.Train:
                Gizmos.color = Color.yellow;
                break;

            case VehicleType.Boat:
                Gizmos.color = Color.green;
                break;

            case VehicleType.Bicycle:
                Gizmos.color = Color.cyan;
                break;

            case VehicleType.Passenger:
                Gizmos.color = Color.white;
                break;
        }

        Gizmos.DrawSphere(transform.position, 1.5f);
    }
}

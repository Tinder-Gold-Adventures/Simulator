using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBoatBarrier : MonoBehaviour
{
    //private behavior information
    private Trafficlight_Barrier barrier;
    private Bridge bridge;

    void Start()
    {
        barrier = GetComponentInChildren<Trafficlight_Barrier>();
        if (barrier == null)
        {
            Debug.LogError("ERROR: No trafficlight_barrier found in InvisibleBoatBarrier object");
        }

        bridge = GameObject.FindGameObjectWithTag("Bridge").GetComponent<Bridge>();
    }

    // Update is called once per frame
    void Update()
    {
        //Sets barrier state based on bridge's state: If bridge is Open, activate invisible barrier so boats will stop
        barrier.IsActive = bridge.state == BarrierState.Open ? false : true;
    }

    private void OnDrawGizmos()
    {
        if(barrier != null)
        {
            switch (barrier.IsActive)
            {
                case true:
                    Gizmos.color = Color.red;
                    break;

                case false:
                    Gizmos.color = Color.green;
                    break;
            }

            Gizmos.DrawCube(transform.position + new Vector3(0, 9, 0), new Vector3(2f, 2f, 2f));
        }
    }
}

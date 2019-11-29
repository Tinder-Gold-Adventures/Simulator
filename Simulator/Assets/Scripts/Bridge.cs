using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupId;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.barrier;
    public DeckState state = DeckState.Closed;

    private const float closedRotationX = 0f;
    private const float openRotationX = 90f;

    //public settable fields
    public float RotationSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        UpdateBridgeStatus();
    }

    public void UpdateBridgeStatus()
    {
        //Get current rotation angle and target rotation angle
        Quaternion objRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f + (state == DeckState.Open ? openRotationX : closedRotationX), objRotation.eulerAngles.y, objRotation.eulerAngles.z);

        if (objRotation != targetRotation)
        {
            //If it's not at it's target rotation: move to target rotation
            transform.rotation = Quaternion.RotateTowards(objRotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }
}

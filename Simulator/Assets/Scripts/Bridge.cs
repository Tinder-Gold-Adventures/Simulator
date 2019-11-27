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

    //Private behavoir information
    private float rotationProgress = 0f;
    public bool IsChangingStates = false;

    //public settable fields
    public float RotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (IsChangingStates)
        {
            UpdateBridgeStatus();
        }
    }

    public void UpdateBridgeStatus()
    {
        if (rotationProgress <= 1f)
        {
            rotationProgress += RotationSpeed * Time.deltaTime;
        }

        Quaternion objRotation = transform.rotation;

        Quaternion targetRotation = Quaternion.Euler(0f + (state == DeckState.Open ? closedRotationX : openRotationX), objRotation.eulerAngles.y, objRotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(objRotation, targetRotation, rotationProgress);

        if (rotationProgress >= 0.98f)
        {
            IsChangingStates = false;
            state = state == DeckState.Open ? DeckState.Closed : DeckState.Open;
            rotationProgress = 0f;
        }
    }
}

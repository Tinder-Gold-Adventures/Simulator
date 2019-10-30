using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private const float closedRotationX = 0f;
    private const float openRotationX = 90f;

    //Private behavoir information
    private float rotationProgress = 0f;
    public bool IsChangingStates = false;
    [HideInInspector]
    public BarrierState state = BarrierState.Closed;

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

        Quaternion targetRotation = Quaternion.Euler(0f + (state == BarrierState.Open ? closedRotationX : openRotationX), objRotation.eulerAngles.y, objRotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(objRotation, targetRotation, rotationProgress);

        if (rotationProgress >= 0.98f)
        {
            IsChangingStates = false;
            state = state == BarrierState.Open ? BarrierState.Closed : BarrierState.Open;
            rotationProgress = 0f;
        }
    }
}

﻿using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(8,8,8));
    }
}

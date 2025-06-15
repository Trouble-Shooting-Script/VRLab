using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public const float CELL_SIZE = 0.03125f;
    public static void Snap(Transform target)
    {
        Vector3 position = target.localPosition;
        position.x = Mathf.Round(position.x / CELL_SIZE) * CELL_SIZE;
        position.y = Mathf.Round(position.y / CELL_SIZE) * CELL_SIZE;
        position.z = Mathf.Round(position.z / CELL_SIZE) * CELL_SIZE;
        target.localPosition = position;
        
        Vector3 euler = target.localEulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        target.localEulerAngles = euler;
    }
}

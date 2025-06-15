using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public const float CELL_SIZE = 0.03125f;
    public static void Snap(Transform target)
    {
        Vector3 position = target.position;
        position.x = Mathf.Round(position.x / CELL_SIZE) * CELL_SIZE;
        position.y = Mathf.Round(position.y / CELL_SIZE) * CELL_SIZE;
        position.z = Mathf.Round(position.z / CELL_SIZE) * CELL_SIZE;
        target.position = position;
        
        Vector3 euler = target.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        target.eulerAngles = euler;
    }
}

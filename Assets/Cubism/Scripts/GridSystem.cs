using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public const float CELL_SIZE = 0.03125f;

    public static void Snap(Transform target)
    {
        Vector3 euler = target.localEulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        target.localEulerAngles = euler;
        
        Vector3 position = target.position;
        position.x = Mathf.Round(position.x / CELL_SIZE) * CELL_SIZE;
        position.y = Mathf.Round(position.y / CELL_SIZE) * CELL_SIZE;
        position.z = Mathf.Round(position.z / CELL_SIZE) * CELL_SIZE;
        target.position = position;
    }

    public static void RelativeSnap(Transform target, Transform relativeTo)
    {
        Quaternion prePro = Quaternion.Inverse(relativeTo.rotation) * target.rotation;
        Vector3 euler = prePro.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        Quaternion snappedRot = Quaternion.Euler(euler);
        target.localRotation = relativeTo.rotation * snappedRot;
        
        Vector3 prePos = Quaternion.Inverse(relativeTo.rotation) * (target.position - relativeTo.position);
        Vector3 position = prePos;
        position.x = Mathf.Round(position.x / CELL_SIZE) * CELL_SIZE;
        position.y = Mathf.Round(position.y / CELL_SIZE) * CELL_SIZE;
        position.z = Mathf.Round(position.z / CELL_SIZE) * CELL_SIZE;
        target.position = relativeTo.position + relativeTo.rotation * position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jjam : MonoBehaviour
{
    public void Toggle(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
    
    public void DebugLog(string message)
    {
        Debug.Log(message);
    }
}

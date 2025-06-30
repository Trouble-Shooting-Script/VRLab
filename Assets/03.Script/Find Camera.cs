using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCamera : MonoBehaviour
{
    
    void Start()
    {
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}

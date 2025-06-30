using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoGazeChecker : MonoBehaviour
{
    public float timeThreshold = 2f;
    public float angleThreshold = 30f;
    
    private float timeSinceGazeOff = 0f;
    private Camera cam;

    private void OnEnable()
    {
        timeSinceGazeOff = 0f;
    }

    private void Start()
    {
        cam = Camera.main;
    }
    
    private void Update()
    {
        var gazeToObject = (transform.position - cam.transform.position).normalized;
        var gazeDirection = cam.transform.forward;
        bool showMenu = Vector3.Dot(gazeToObject, gazeDirection) > AngleToDot(angleThreshold);

        if (showMenu)
        {
            timeSinceGazeOff = 0f;
        }
        else
        {
            timeSinceGazeOff += Time.deltaTime;
        }

        if (timeSinceGazeOff >= timeThreshold)
        {
            gameObject.SetActive(false);
        }
    }
    
    static float AngleToDot(float angleDeg)
    {
        return Mathf.Cos(Mathf.Deg2Rad * angleDeg);
    }
}

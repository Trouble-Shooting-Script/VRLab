using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class XRDebugUI : MonoBehaviour
{
    public TMP_Text[] Texts;
    public Transform leftHand;
    public Transform rightHand;
    public Transform e1;
    public Transform e2;

    public float middleEndTime { get; set; }
    public float tolerance = 0.1f; // seconds
    public GameObject testObject;

    private void Update()
    {
        Texts[0].text = e1.position.ToString();
        Texts[1].text = e2.position.ToString();
    }

    public void PrintDebug(int textIndex, string str)
    {
        Texts[textIndex].text = str;
    }
    
    public void UpdateMiddleEndTime()
    {
        middleEndTime = Time.timeSinceLevelLoad;
    }

    public void FingerSnap()
    {
        if(middleEndTime + tolerance < Time.timeSinceLevelLoad)
        {
            PrintDebug(3, "Finger Snap Detected");
            testObject.SetActive(!testObject.activeSelf);
        }
    }
}

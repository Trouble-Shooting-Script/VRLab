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

    private void Update()
    {
        Texts[0].text = e1.position.ToString();
        Texts[1].text = e2.position.ToString();
    }
}

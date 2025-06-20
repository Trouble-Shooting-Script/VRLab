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

    private void Update()
    {
        Texts[0].text = leftHand.rotation.eulerAngles.ToString();
        Texts[1].text = rightHand.rotation.eulerAngles.ToString();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FigureGrab : MonoBehaviour
{
    private XRBaseInteractable mGrabInteractable;
    public Vector3 smallScale;
    public Vector3 largeScale;

    private void OnEnable()
    {
        mGrabInteractable = GetComponent<XRBaseInteractable>();
        mGrabInteractable.selectEntered.AddListener(ScaleSet);
        mGrabInteractable.selectExited.AddListener(ScaleReset);
    }

    private void OnDisable()
    {
        mGrabInteractable.selectEntered.RemoveListener(ScaleSet);
        mGrabInteractable.selectExited.RemoveListener(ScaleReset);
    }

    private void ScaleSet(SelectEnterEventArgs args)
    {
        Debug.Log("ScaleSet 호출됨");
        Debug.Log(args.interactableObject);
        Debug.Log(args.interactorObject);
        args.interactableObject.transform.localScale = smallScale;
    }
    private void ScaleReset(SelectExitEventArgs args)
    {
        Debug.Log("ScaleReset 호출됨");
        Debug.Log(args.interactableObject);
        Debug.Log(args.interactorObject);
        args.interactableObject.transform.localScale = largeScale;
    }
}

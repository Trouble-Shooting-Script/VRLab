using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FigureGrab : MonoBehaviour
{
    private XRGrabInteractable mGrabInteractable;
    public Vector3 smallScale;

    private void OnEnable()
    {
        mGrabInteractable = GetComponent<XRGrabInteractable>();
        mGrabInteractable.trackScale = true;
        mGrabInteractable.selectEntered.AddListener(ScaleSet);
    }

    private void OnDisable()
    {
        mGrabInteractable.selectEntered.RemoveListener(ScaleSet);
    }
    
    private float mScale;

    private void ScaleSet(SelectEnterEventArgs args)
    {
        if (args.interactorObject.handedness == InteractorHandedness.None)
        {
            mGrabInteractable.selectMode = InteractableSelectMode.Single;
            mGrabInteractable.SetTargetLocalScale(smallScale);
        }
        else
        {
            mGrabInteractable.selectMode = InteractableSelectMode.Multiple;
        }
    }
}

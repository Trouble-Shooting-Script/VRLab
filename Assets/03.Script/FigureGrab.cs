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
        mGrabInteractable.selectEntered.AddListener(ScaleSet);
    }

    private void OnDisable()
    {
        mGrabInteractable.selectEntered.RemoveListener(ScaleSet);
    }

    private bool mCanChangeScale = false;
    private float mScale;

    private void ScaleSet(SelectEnterEventArgs args)
    {

        args.interactableObject.transform.localScale = smallScale;
        if (args.interactorObject.handedness == InteractorHandedness.None)
        {
            mGrabInteractable.selectMode = InteractableSelectMode.Single;
        }
        else
        {
            mGrabInteractable.selectMode = InteractableSelectMode.Multiple;
        }
    }
}

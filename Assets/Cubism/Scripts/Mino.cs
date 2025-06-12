using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Mino : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable interactable;
    public List<GameObject> blocks = new List<GameObject>();
    public Vector3 startPos;
    public Quaternion startRot;
    public int[,,] snapShot;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("OnGrab");
        
        startPos = transform.position;
        startRot = transform.rotation;
        snapShot = (int[,,])ToyMaker.instance.Answer.Clone();

        // separate blocks from puzzle
        #region duplicate code
        foreach (var block in blocks)
        {
            Vector3 coord = (block.transform.position - ToyMaker.instance.puzzle.transform.position) * 100f;
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            int z = Mathf.RoundToInt(coord.z);

            try
            {
                switch (snapShot[x, y, z])
                {
                    case 1:
                        snapShot[x, y, z] = 0;
                        break;
                }
            }
            catch (Exception e)
            {
                // out of index
            }
        }
        #endregion
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("OnRelease");
        
        Snap(this.transform);
        if (Transaction() == false)
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
        else
        {
            ToyMaker.instance.Answer = snapShot;
            
            bool isSolved = true;
            foreach (int i in snapShot)
            {
                if (i == 0)
                {
                    isSolved = false;
                    break;
                }
            }

            if (isSolved)
            {
                ToyMaker.instance.Clear();
            }
        }
    }

    public static void Snap(Transform target)
    {
        Vector3 position = target.position;
        position.x = Mathf.Round(position.x * 100) * 0.01f;
        position.y = Mathf.Round(position.y * 100) * 0.01f;
        position.z = Mathf.Round(position.z * 100) * 0.01f;
        target.position = position;
        
        Vector3 euler = target.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        target.eulerAngles = euler;
    }

    private bool Transaction()
    {
        foreach (var block in blocks)
        {
            Vector3 coord = (block.transform.position - ToyMaker.instance.puzzle.transform.position) * 100f;
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            int z = Mathf.RoundToInt(coord.z);

            try
            {
                switch (snapShot[x, y, z])
                {
                    case 1:
                        return false;
                    case 0:
                        snapShot[x, y, z] = 1;
                        break;
                }
            }
            catch (Exception e)
            {
                // out of index
            }
        }

        return true;
    }

    public void UpdateCollider()
    {
        foreach (Transform child in transform)
        {
            var col = child.GetComponent<Collider>();
            interactable.colliders.Add(col);
        }
        
        // Reassign interaction manager to update the colliders
        var manager = FindFirstObjectByType<XRInteractionManager>();
        // manager.RegisterInteractable(interactable as IXRInteractable);
        interactable.interactionManager = null;
        interactable.interactionManager = manager;
    }
}

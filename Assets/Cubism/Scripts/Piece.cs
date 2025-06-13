using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Piece : MonoBehaviour
{
    // interactable variables
    public XRBaseInteractable interactable;
    public Vector3 startPos;
    public Quaternion startRot;
    
    // block variables
    public GameObject blockPrefab;
    public List<GameObject> blocks = new List<GameObject>();
    private int[,,] snapShot;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }
    
    public GameObject MakeModel(int[,,] bluePrint)
    {
        for (int x = 0; x < bluePrint.GetLength(0); x++)
        {
            for (int y = 0; y < bluePrint.GetLength(1); y++)
            {
                for (int z = 0; z < bluePrint.GetLength(2); z++)
                {
                    if (bluePrint[x, y, z] == 1)
                    {
                        var b = Instantiate(blockPrefab, transform);
                        b.transform.localPosition = new Vector3(x, y, z) * 0.01f;
                        blocks.Add(b);
                    }
                }
            }
        }
        UpdateCollider();

        return gameObject;
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
        
        ToyMaker.Snap(this.transform);
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
        XRInteractionManager manager = interactable.interactionManager;
        interactable.interactionManager = null;
        interactable.interactionManager = manager;
    }
}

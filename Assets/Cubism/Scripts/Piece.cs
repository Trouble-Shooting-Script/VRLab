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
    private int[,,] snapshot;

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
                        b.transform.localScale = Vector3.one * GridSystem.CELL_SIZE;
                        b.transform.localPosition = new Vector3(x, y, z) * GridSystem.CELL_SIZE;
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
        startPos = transform.position;
        startRot = transform.rotation;
        snapshot = (int[,,])ToyMaker.instance.Answer.Clone();
        
        // separate blocks from puzzle
        TryUpdateSnapshot(0, v => v == 1);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        GridSystem.Snap(this.transform);
        if (Merge() == false)
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
        else
        {
            ToyMaker.instance.Answer = snapshot;
            
            bool isSolved = true;
            foreach (int i in snapshot)
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

    private bool Merge()
    {
        return TryUpdateSnapshot(1, v => v == 0);
    }

    private bool TryUpdateSnapshot(int valueToSet, Func<int,bool> targetCondition, Func<int, bool> returnCondition = null)
    {
        foreach (var block in blocks)
        {
            Vector3 coord = (block.transform.position - ToyMaker.instance.puzzle.transform.position) / GridSystem.CELL_SIZE;
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            int z = Mathf.RoundToInt(coord.z);

            try
            {
                if(targetCondition(snapshot[x, y, z]))
                {
                    snapshot[x, y, z] = valueToSet;
                }
                else if (returnCondition != null && returnCondition(snapshot[x, y, z]))
                {
                    return false;
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

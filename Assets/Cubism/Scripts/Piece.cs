using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Piece : MonoBehaviour
{
    private static readonly int EMISSION_COLOR = Shader.PropertyToID("_EmissionColor");
    private static readonly int COLOR = Shader.PropertyToID("_AlbedoColor");

    // interactable variables
    public XRBaseInteractable interactable;
    public new Rigidbody rigidbody;
    
    public Transform boardTransform;
    public Outlinable outlinable;
    
    // block variables
    public GameObject blockPrefab;
    public List<GameObject> blocks = new List<GameObject>();
    public Int3DArray snapshot;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        outlinable = GetComponent<Outlinable>();
        interactable = GetComponent<XRBaseInteractable>();
        
        interactable.firstHoverEntered.AddListener((args =>
        {
            outlinable.enabled = true;
        }));
        interactable.lastHoverExited.AddListener((args =>
        {
            if (interactable.isSelected == false)
            {
                outlinable.enabled = false;
            }
        }));
        interactable.selectExited.AddListener((args =>
        {
            outlinable.enabled = false;
        }));
        
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        boardTransform = transform.parent;
        outlinable.AddAllChildRenderersToRenderingList();
        outlinable.OutlineParameters.Color = Color.white;
        outlinable.enabled = false;
    }

    public GameObject MakeModel(int[,,] bluePrint, Color color)
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
                        
                        // set block color
                        Color.RGBToHSV(color, out float h, out float s, out float v);
                        Color emissionColor = Color.HSVToRGB(h, s * 2, v);
                        var r = b.GetComponent<Renderer>();
                        r.material.EnableKeyword("_MK_EMISSION");
                        r.material.SetColor(COLOR, color);
                        r.material.SetColor(EMISSION_COLOR, emissionColor);
                        
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
        rigidbody.isKinematic = false;
        SeparateFromPuzzle();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // make a snapshot
        snapshot = CusismManager.instance.Answer.Clone();
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        
        GridSystem.RelativeSnap(transform, CusismManager.instance.puzzle.transform);

        TryMerge(out bool isUpdateSuccess, out bool isInArea);
        if (isUpdateSuccess)
        {
            if (isInArea)
            {
                rigidbody.isKinematic = true;
                transform.parent = CusismManager.instance.puzzle.transform;
            }
            
            // apply the changes
            CusismManager.instance.Answer = snapshot;
                
            // check puzzle completion
            bool isSolved = true;
            foreach (int i in snapshot.ToArray())
            {
                if (i == 0)
                {
                    isSolved = false;
                    break;
                }
            }
            if (isSolved)
            {
                CusismManager.instance.Clear();
            }
        }
        
        if (isInArea == false || isUpdateSuccess == false)
        {
            // revert snap
            transform.position = pos;
            transform.rotation = rot;
            transform.parent = boardTransform;
            rigidbody.isKinematic = false;
            Debug.Log("Reverted");
        }
    }

    private void SeparateFromPuzzle()
    {
        TryUpdateSnapshot(CusismManager.instance.Answer, 0, v => v == 1);
    }

    private void TryMerge(out bool isUpdateSuccess, out bool isInArea)
    {
        isUpdateSuccess = TryUpdateSnapshot(snapshot, 1, v => v == 0, i => i == 1, out isInArea);
        Debug.Log($"update success: {isUpdateSuccess}, in area: {isInArea}");
    }

    private bool TryUpdateSnapshot(Int3DArray arrayToUpdate, int valueToSet, Func<int, bool> targetCondition)
    {
        return TryUpdateSnapshot(arrayToUpdate, valueToSet, targetCondition, null, out bool isInArea);
    }
    
    private bool TryUpdateSnapshot(Int3DArray arrayToUpdate, int valueToSet, Func<int,bool> targetCondition, Func<int, bool> conflictCheck, out bool isInArea)
    {
        Debug.Log("<color=red>TryUpdateSnapshot</color>");
        isInArea = false;
        foreach (var block in blocks)
        {
            Vector3 coord = CusismManager.instance.puzzle.transform.InverseTransformPoint(block.transform.position) / GridSystem.CELL_SIZE;
            int x = Mathf.RoundToInt(coord.x);
            int y = Mathf.RoundToInt(coord.y);
            int z = Mathf.RoundToInt(coord.z);

            try
            {
                if(targetCondition(arrayToUpdate[x, y, z]))
                {
                    arrayToUpdate[x, y, z] = valueToSet;
                    isInArea = true;
                    
                    Debug.Log($"{x}, {y}, {z} = {valueToSet}");
                }
                else if (conflictCheck != null && conflictCheck(arrayToUpdate[x, y, z]))
                {
                    Debug.Log($"conflict at {x}, {y}, {z} with value {arrayToUpdate[x, y, z]}");
                    return false;
                }
            }
            catch (Exception e)
            {
                // out of index
                Debug.Log($"Out of index at {x}, {y}, {z}.");
            }
        }

        return true;
    }

    private void UpdateCollider()
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ToyMaker : MonoBehaviour
{
    public static ToyMaker instance;
    public List<GameObject> toyBucket = new();
    public Sample sample;
    public Piece piece;
    [HideInInspector] public GameObject puzzle;
    public ShapeSetData bluePrints;
    public int[,,] Answer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeBoard(bluePrints.GetAllShapes());
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (toyBucket != null && toyBucket.Count > 0)
            {
                foreach (var toy in toyBucket)
                {
                    Destroy(toy.gameObject);
                }

                toyBucket.Clear();
            }
        }
    }

    private void MakeBoard(List<int[,,]> bluePrints)
    {
        // create a board
        GameObject absBoard = new GameObject();
        absBoard.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.1f);
        absBoard.name = "Board";
        Snap(absBoard.transform);

        // create a puzzle
        Sample sam = Instantiate(sample, absBoard.transform);
        puzzle = sam.MakeModel(bluePrints[0]);
        puzzle.name = "Puzzle";
        toyBucket.Add(absBoard);

        // create an answer array
        Answer = (int[,,])bluePrints[0].Clone();
        for (int x = 0; x < Answer.GetLength(0); x++)
        {
            for (int y = 0; y < Answer.GetLength(1); y++)
            {
                for (int z = 0; z < Answer.GetLength(2); z++)
                {
                    switch (Answer[x, y, z])
                    {
                        case 1:
                            Answer[x, y, z] = 0;
                            break;
                        case 0:
                            Answer[x, y, z] = -1;
                            break;
                    }
                }
            }
        }


        // create pieces
        for (int i = 1; i < bluePrints.Count; i++)
        {
            Piece piece = Instantiate(this.piece, absBoard.transform);
            piece.transform.localPosition += new Vector3((bluePrints[0].GetLength(0) + 3) * 0.01f, 0.05f * i, 0);
            piece.MakeModel(bluePrints[i]);
            piece.name = "Piece_" + i;
            toyBucket.Add(piece.gameObject);
        }
    }

    public void Clear()
    {
        Debug.Log("아 성공");
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
}
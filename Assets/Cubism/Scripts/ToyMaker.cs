using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ToyMaker : MonoBehaviour
{
    public static ToyMaker instance;
    private static Color[] colors =
    {
        new Color(1.000f, 0.718f, 0.698f, 1.0f),
        new Color(0.698f, 0.922f, 0.949f, 1.0f),
        new Color(0.698f, 0.949f, 0.733f, 1.0f),
        new Color(0.784f, 0.749f, 0.906f, 1.0f),
        new Color(1.000f, 0.875f, 0.729f, 1.0f),
        new Color(1.000f, 0.980f, 0.804f, 1.0f),
        new Color(1.000f, 0.894f, 0.882f, 1.0f)
    };
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
        absBoard.transform.position = Camera.main.transform.position + new Vector3(0, -0.1f, 0.3f);
        absBoard.name = "Board";
        GridSystem.Snap(absBoard.transform);
        toyBucket.Add(absBoard);

        // create a puzzle
        Sample sam = Instantiate(sample, absBoard.transform);
        puzzle = sam.MakeModel(bluePrints[0]);
        puzzle.name = "Puzzle";

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
        Color[] shuffledColor = RandomUtil.GetShuffled(colors);
        for (int i = 1; i < bluePrints.Count; i++)
        {
            Piece piece = Instantiate(this.piece, absBoard.transform);
            piece.transform.localPosition += new Vector3((bluePrints[0].GetLength(0) + 3) * GridSystem.CELL_SIZE,
                0.15f * (i - 1), 0);
            piece.MakeModel(bluePrints[i], shuffledColor[i % shuffledColor.Length]);
            piece.name = "Piece_" + i;
            toyBucket.Add(piece.gameObject);
        }
    }

    public void Clear()
    {
        Debug.Log("아 성공");
    }
}
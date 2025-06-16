using System;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ToyMaker : MonoBehaviour
{
    public static ToyMaker instance;
    private static Color[] colors =
    {
        new Color(1.000f, 0.595f, 0.541f, 1.0f),
        new Color(0.448f, 0.922f, 0.949f, 1.0f),
        new Color(0.448f, 0.949f, 0.629f, 1.0f),
        new Color(0.597f, 0.505f, 0.906f, 1.0f),
        new Color(1.000f, 0.729f, 0.458f, 1.0f),
        new Color(1.000f, 0.882f, 0.610f, 1.0f),
        new Color(1.000f, 0.764f, 0.741f, 1.0f)
    };
    public List<GameObject> toyBucket = new();
    public Sample samplePrefab;
    public Piece piecePrefab;
    [HideInInspector] public GameObject puzzle;
    public ShapeSetData bluePrintsData;
    public int[,,] Answer;


    public ShapeSetData data;

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
            MakeBoard(data, Camera.main.transform.position + new Vector3(0, -0.1f, 0.3f), Quaternion.identity);
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

    public void MakeBoard(ShapeSetData shapeSetData)
    {
        MakeBoard(shapeSetData, Camera.main.transform.position + new Vector3(0, -0.1f, 0.3f), Quaternion.identity);
    }

    private void MakeBoard(ShapeSetData shapeSetData, Vector3 position, Quaternion rotation)
    {
        if(toyBucket != null && toyBucket.Count > 0)
        {
            foreach (var toy in toyBucket)
            {
                Destroy(toy.gameObject);
            }

            toyBucket.Clear();
        }
        
        var bluePrints = shapeSetData.GetAllShapes();
        int[,,] puzzleShape = bluePrints[0];
        bluePrints.Remove(puzzleShape);
        
        // create an answer array
        Answer = (int[,,])puzzleShape.Clone();
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
        
        // create a board
        GameObject absBoard = new GameObject();
        absBoard.transform.position = position;
        absBoard.transform.rotation = rotation;
        absBoard.name = "Board";
        GridSystem.Snap(absBoard.transform);
        toyBucket.Add(absBoard);

        // create a puzzle
        Sample sam = Instantiate(samplePrefab, absBoard.transform);
        puzzle = sam.MakeModel(puzzleShape);
        puzzle.name = "Puzzle";

        // create pieces
        Color[] shuffledColor = RandomUtil.GetShuffled(colors);
        var shuffledBluePrints = RandomUtil.GetShuffled(bluePrints.ToArray());
        
        Vector3 distance = absBoard.transform.right * 0.3f;
        float degree = 360f / shuffledBluePrints.Length;
        
        for (int i = 0; i < shuffledBluePrints.Length; i++)
        {
            Piece piece = Instantiate(piecePrefab, absBoard.transform);
            toyBucket.Add(piece.gameObject);
            
            piece.transform.localPosition = Quaternion.AngleAxis(degree * i, absBoard.transform.forward) * distance;
            piece.MakeModel(shuffledBluePrints[i], shuffledColor[i % shuffledColor.Length]);
            piece.name = $"Piece_{i}";
            piece.rigidbody.AddTorque(
                new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 10f,
                ForceMode.Impulse);
        }
    }

    public void Clear()
    {
        Debug.Log("아 성공");
    }
}
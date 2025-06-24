using System;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CusismManager : MonoBehaviour
{
    public static CusismManager instance;
    
    [Header("Piece Colors")]
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
    public Material clearEffectMaterial;
    private float cutLine = 0.0f;
    private float cutLineMax = 3.0f;
    private bool isClear = false;
    
    
    public List<GameObject> toyBucket = new();
    public Sample samplePrefab;
    public Piece piecePrefab;
    [HideInInspector] public GameObject puzzle;
    public Int3DArray Answer;

    [Header("Whole Game Variables")]
    public static List<ShapeSetData> clearPuzzleData = new List<ShapeSetData>();

    private static readonly int CUT_LINE = Shader.PropertyToID("_Cut_Line");
    public ShapeSetData currentPuzzleData;
    public int prizeCoin = 10;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            clearEffectMaterial.SetFloat(CUT_LINE, cutLine);
        }
    }

    private void Update()
    {
        if (isClear && cutLine < cutLineMax)
        {
            cutLine += Time.deltaTime * 0.15f;
            clearEffectMaterial.SetFloat(CUT_LINE, cutLine);
        }
    }

    private void OnDestroy()
    {
        DestroyAll();
    }

    private void DestroyAll()
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

    public void MakeBoard(ShapeSetData shapeSetData)
    {
        MakeBoard(shapeSetData, Camera.main.transform.position + new Vector3(0, -0.1f, 0.45f), Quaternion.identity);
    }

    private void MakeBoard(ShapeSetData shapeSetData, Vector3 position, Quaternion rotation)
    {
        cutLine = 0f;
        isClear = false;
        
        currentPuzzleData = shapeSetData;
        DestroyAll();
        
        var bluePrints = shapeSetData.GetAllShapes();
        int[,,] puzzleShape = bluePrints[0];
        bluePrints.Remove(puzzleShape);
        
        // create an answer array
        Answer = Int3DArray.FromArray(puzzleShape);
        for (int x = 0; x < Answer.sizeX; x++)
        {
            for (int y = 0; y < Answer.sizeY; y++)
            {
                for (int z = 0; z < Answer.sizeZ; z++)
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
        
        Vector3 distance = absBoard.transform.right * 0.2f;
        float degree = 180f / (shuffledBluePrints.Length - 1);
//        Debug.Log($"{shuffledBluePrints.Length} / {degree}");
        
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
        if (clearPuzzleData.Contains(currentPuzzleData))
        {
            return;
        }
        
        clearPuzzleData.Add(currentPuzzleData);
        GameManager.instance.CurrentCoin += prizeCoin;
        cutLine = puzzle.GetComponent<Collider>().bounds.min.y;
        isClear = true;
        
        Debug.Log("아 성공");
    }
}
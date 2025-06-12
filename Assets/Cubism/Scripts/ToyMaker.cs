using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ToyMaker : MonoBehaviour
{
    public static ToyMaker instance;
    public List<GameObject> toyList = new();
    public BlockSet mino;
    public GameObject normalBlock;
    public Block ghostBlock;
    [HideInInspector] public GameObject puzzle;
    public ShapeSetData BluePrints;
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
            MakeBoard(BluePrints.GetAllShapes());
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (toyList != null && toyList.Count > 0)
            {
                foreach (var toy in toyList)
                {
                    Destroy(toy.gameObject);
                }

                toyList.Clear();
            }
        }
    }

    public GameObject MakePuzzle(int[,,] bluePrint)
    {
        BlockSet toy = Instantiate(mino);
        for (int x = 0; x < bluePrint.GetLength(0); x++)
        {
            for (int y = 0; y < bluePrint.GetLength(1); y++)
            {
                for (int z = 0; z < bluePrint.GetLength(2); z++)
                {
                    if (bluePrint[x, y, z] == 1)
                    {
                        var b = Instantiate(ghostBlock, toy.transform);
                        b.transform.localPosition = new Vector3(x, y, z) * 0.01f;
                        toy.blocks.Add(b.gameObject);

                        if (IsThere(x, y + 1, z))
                        {
                            b.DisableLines(b.up);
                        }
                        if (IsThere(x, y - 1, z))
                        {
                            b.DisableLines(b.down);
                        }
                        if (IsThere(x - 1, y, z))
                        {
                            b.DisableLines(b.left);
                        }
                        if (IsThere(x + 1, y, z))
                        {
                            b.DisableLines(b.right);
                        }
                        if (IsThere(x, y, z - 1))
                        {
                            b.DisableLines(b.back);
                        }
                        if (IsThere(x, y, z + 1))
                        {
                            b.DisableLines(b.forward);
                        }
                    }
                }
            }
        }
        return toy.gameObject;

        bool IsThere(int x, int y , int z)
        {
            try
            {
                return bluePrint[x, y, z] == 1;
            }
            catch (Exception e)
            {
                // out of index
                return false;
            }
        }
    }

    public GameObject MakeToy(int[,,] bluePrint)
    {
        return MakeToy(bluePrint, Vector3.zero, normalBlock);
    }

    public GameObject MakeToy(int[,,] bluePrint, Vector3 position, GameObject prefab)
    {
        BlockSet toy = Instantiate(mino);
        toy.transform.position = position;
        for (int x = 0; x < bluePrint.GetLength(0); x++)
        {
            for (int y = 0; y < bluePrint.GetLength(1); y++)
            {
                for (int z = 0; z < bluePrint.GetLength(2); z++)
                {
                    if (bluePrint[x, y, z] == 1)
                    {
                        var b = Instantiate(prefab, toy.transform);
                        b.transform.localPosition = new Vector3(x, y, z) * 0.01f;
                        toy.blocks.Add(b);
                    }
                }
            }
        }

        return toy.gameObject;
    }

    private void MakeBoard(List<int[,,]> bluePrints)
    {
        GameObject absBoard = new GameObject();
        absBoard.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.1f);
        absBoard.name = "Board";
        Mino.Snap(absBoard.transform);

        Answer = (int[,,])bluePrints[0].Clone();
        puzzle = MakePuzzle(bluePrints[0]);
        puzzle.transform.SetParent(absBoard.transform, false);
        puzzle.name = "Puzzle";
        bluePrints.RemoveAt(0);

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

        toyList.Add(absBoard);

        for (int i = 0; i < bluePrints.Count; i++)
        {
            int[,,] bluePrint = bluePrints[i];
            var toy = MakeToy(bluePrint, absBoard.transform.position + new Vector3(0.05f, 0.05f * i, 0), normalBlock);
            toy.GetComponent<Mino>().UpdateCollider();
            toyList.Add(toy);
        }
    }

    public void Clear()
    {
        Debug.Log("아 성공");
    }
}
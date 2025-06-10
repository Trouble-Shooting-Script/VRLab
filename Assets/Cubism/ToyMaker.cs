using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ToyMaker : MonoBehaviour
{
    public static ToyMaker instance;
    public List<GameObject> toyList = new List<GameObject>();
    public Mino mino;
    public GameObject block;
    public Material ghostMat;
    public GameObject puzzle;
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
            List<int[,,]> bluePrints = new List<int[,,]>();

            bluePrints = new List<int[,,]>();
            bluePrints.Add(new int[,,]{{{1,1},{1,1}},{{1,1},{1,1}}});
            bluePrints.Add(new int[,,]{{{1,1},{1,0}},{{1,0},{0,0}}});
            bluePrints.Add(new int[,,]{{{1,1},{1,0}},{{1,0},{0,0}}});
            
            MakeBoard(bluePrints);
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

    private int[,,] GetRandomBluePrint()
    {
        int[,,] bp = new int[4, 4, 4];
        for (int x = 0; x < bp.GetLength(0); x++)
        {
            for (int y = 0; y < bp.GetLength(1); y++)
            {
                for (int z = 0; z < bp.GetLength(2); z++)
                {
                    bp[x,y,z] = Random.Range(0,2);
                }
            }
        }

        return bp;
    }
    
    public GameObject MakeToy()
    {
        return MakeToy(GetRandomBluePrint());
    }

    public GameObject MakeToy(int[,,] bluePrint)
    {
        return MakeToy(bluePrint, Vector3.zero);
    }
    
    public GameObject MakeToy(int[,,] bluePrint, Vector3 position)
    {
        Mino toy = Instantiate(mino);
        toy.transform.position = position;
        for (int x = 0; x < bluePrint.GetLength(0); x++)
        {
            for (int y = 0; y < bluePrint.GetLength(1); y++)
            {
                for (int z = 0; z < bluePrint.GetLength(2); z++)
                {
                    if (bluePrint[x, y, z] == 1)
                    {
                        var b = Instantiate(block, toy.transform);
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
        puzzle = MakeToy(bluePrints[0]);
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
        
        var rdrs = puzzle.GetComponentsInChildren<Renderer>();
        foreach (Renderer rdr in rdrs)
        {
            rdr.material = ghostMat;
        }
        
        toyList.Add(absBoard);

        for (int i = 0; i < bluePrints.Count; i++)
        {
            int[,,] bluePrint = bluePrints[i];
            var toy = MakeToy(bluePrint, absBoard.transform.position + new Vector3(0.05f, 0.05f * i, 0));
            toy.GetComponent<Mino>().UpdateCollider();
            toyList.Add(toy);
        }
    }

    public void Clear()
    {
        Debug.Log("아 성공");
    }
}

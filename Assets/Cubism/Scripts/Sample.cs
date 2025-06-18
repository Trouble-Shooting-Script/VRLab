using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sample : MonoBehaviour
{
    public Transform center;
    public SampleBlock[,,] Blueprint;
    public SampleBlock blockPrefab;
    
    public GameObject MakeModel(int[,,] bluePrint)
    {
        int xSize = bluePrint.GetLength(0);
        int ySize = bluePrint.GetLength(1);
        int zSize = bluePrint.GetLength(2);
        Blueprint = new SampleBlock[xSize, ySize, zSize];
        
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if (bluePrint[x, y, z] == 1)
                    {
                        var b = Instantiate(blockPrefab, transform);
                        b.transform.localScale = Vector3.one * (GridSystem.CELL_SIZE * 0.9921875f);
                        b.transform.localPosition = new Vector3(x, y, z) * GridSystem.CELL_SIZE;
                        Blueprint[x, y, z] = b;
                    }
                }
            }
        }
        CleanOutline();
        UpdateBound(xSize, ySize, zSize);
        AdjustCenterPosition(xSize, ySize, zSize);

        return gameObject;
    }

    private void AdjustCenterPosition(int x, int y, int z)
    {
        Vector3 newCenter = (new Vector3(x, y, z) - Vector3.one) * (GridSystem.CELL_SIZE * 0.5f);
        center.localPosition = newCenter;
    }

    private void UpdateBound(int x, int y, int z)
    {
        var col = GetComponent<BoxCollider>();
        col.size = new Vector3(x, y, z) * GridSystem.CELL_SIZE;
        col.center = col.size * 0.5f - Vector3.one * (GridSystem.CELL_SIZE * 0.5f);
    }
    
    private void CleanOutline()
    {
        for (int x = 0; x < Blueprint.GetLength(0); x++)
        {
            for (int y = 0; y < Blueprint.GetLength(1); y++)
            {
                for (int z = 0; z < Blueprint.GetLength(2); z++)
                {
                    if (Blueprint[x, y, z] is null)
                    {
                        continue;
                    }

                    SampleBlock b = Blueprint[x, y, z];

                    b.Flag.L = IsThere(x - 1, y, z);
                    b.Flag.R = IsThere(x + 1, y, z);
                    b.Flag.U = IsThere(x, y + 1, z);
                    b.Flag.D = IsThere(x, y - 1, z);
                    b.Flag.F = IsThere(x, y, z + 1);
                    b.Flag.B = IsThere(x, y, z - 1);
                    
                    b.Flag.LU = IsThere(x - 1, y + 1, z);
                    b.Flag.LD = IsThere(x - 1, y - 1, z);
                    b.Flag.RU = IsThere(x + 1, y + 1, z);
                    b.Flag.RD = IsThere(x + 1, y - 1, z);
                    b.Flag.LF = IsThere(x - 1, y, z + 1);
                    b.Flag.RF = IsThere(x + 1, y, z + 1);
                    b.Flag.LB = IsThere(x - 1, y, z - 1);
                    b.Flag.RB = IsThere(x + 1, y, z - 1);
                    b.Flag.FU = IsThere(x, y + 1, z + 1);
                    b.Flag.FD = IsThere(x, y - 1, z + 1);
                    b.Flag.BU = IsThere(x, y + 1, z - 1);
                    b.Flag.BD = IsThere(x, y - 1, z - 1);
                    
                    b.CleanLines();
                }
            }
        }

        bool IsThere(int x, int y , int z)
        {
            try
            {
                return Blueprint[x, y, z] is not null;
            }
            catch (Exception e)
            {
                // out of index
                return false;
            }
        }
    }
}


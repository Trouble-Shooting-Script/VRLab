using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Int3DArray
{
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public int[] data;

    public Int3DArray(int x, int y, int z)
    {
        Initialize(x, y, z);
    }
    
    public void Initialize(int x, int y, int z)
    {
        sizeX = x;
        sizeY = y;
        sizeZ = z;
        data = new int[x * y * z];
    }
    
    public int this[int x, int y, int z]
    {
        get
        {
            if(x < 0 || x >= sizeX ||
               y < 0 || y >= sizeY ||
               z < 0 || z >= sizeZ)
            {
                throw new System.IndexOutOfRangeException("Index out of bounds for Int3DArray.");
            }
            return data[x + sizeX * (y + sizeY * z)];
        }
        set
        {
            if(x < 0 || x >= sizeX ||
               y < 0 || y >= sizeY ||
               z < 0 || z >= sizeZ)
            {
                throw new System.IndexOutOfRangeException("Index out of bounds for Int3DArray.");
            }
            data[x + sizeX * (y + sizeY * z)] = value;
        }
    }

    public static Int3DArray FromArray(int[,,] source)
    {
        int sizeX = source.GetLength(0);
        int sizeY = source.GetLength(1);
        int sizeZ = source.GetLength(2);

        Int3DArray result = new Int3DArray(sizeX, sizeY, sizeZ);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    result[x, y, z] = source[x, y, z];
                }
            }
        }
        return result;
    }

    // 변환: Int3DArray -> int[,,]
    public int[,,] ToArray()
    {
        int[,,] array = new int[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    array[x, y, z] = this[x, y, z];
                }
            }
        }
        return array;
    }

    public Int3DArray Clone()
    {
        Int3DArray clone = new Int3DArray(sizeX, sizeY, sizeZ);
        clone.data = (int[])data.Clone();
        return clone;
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "ShapeSetData", menuName = "Shape/Shape Set Data")]
public class ShapeSetData : ScriptableObject
{
    [TextArea(3, 30)]
    public List<string> shapeTexts = new List<string>();

    public List<int[,,]> GetAllShapes()
    {
        var allShapes = new List<int[,,]>();

        foreach (var shapeText in shapeTexts)
        {
            var shape = ParseShape(shapeText.Trim());
            allShapes.Add(shape);
        }

        return allShapes;
    }

    private int[,,] ParseShape(string data)
    {
        var blocks = Regex.Split(data.Trim(), @"(?:\r?\n){2,}");
        int xSize = blocks.Length;

        int ySize = 0;
        int zSize = 0;

        // 블록 크기 측정 (모든 블록이 동일한 크기라고 가정)
        if (xSize > 0)
        {
            var lines = blocks[0].Split('\n');
            ySize = lines.Length;
            if (ySize > 0)
                zSize = lines[0].Trim().Length;
        }

        int[,,] result = new int[zSize, ySize, xSize];

        for (int z = 0; z < xSize; z++)
        {
            var lines = blocks[z].Split('\n');

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y].Trim();
                for (int x = 0; x < line.Length; x++)
                {
                    result[x, y, z] = (line[x] == '1') ? 1 : 0;
                }
            }
        }
        
        return result;
    }
}
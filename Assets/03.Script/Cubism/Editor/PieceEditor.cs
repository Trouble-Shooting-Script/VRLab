using UnityEditor;

[CustomEditor(typeof(Piece))]
public class PieceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Piece script = (Piece)target;

        if (script.snapshot != null && script.snapshot.data != null)
        {
            for (int x = 0; x < script.snapshot.sizeX; x++)
            {
                EditorGUILayout.LabelField($"Slice X={x}");
                for (int y = 0; y < script.snapshot.sizeY; y++)
                {
                    string row = "";
                    for (int z = 0; z < script.snapshot.sizeZ; z++)
                    {
                        row += script.snapshot[x, y, z] + " ";
                    }

                    EditorGUILayout.LabelField(row);
                }
            }
        }
    }
}
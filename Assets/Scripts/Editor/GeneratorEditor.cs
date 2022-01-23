using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var generator = (Generator)target;

        if (GUILayout.Button("Generate"))
        {
            generator.Setup();
            generator.Generate();

            if (generator.Rooms.Count > 0)
            {
                generator.MapGrid.ToList().ForEach(mapCols =>
                {
                    mapCols.Where(cell => cell != null).ToList().ForEach(cell => EditorUtility.SetDirty(cell.Chip));
                });
            }
        }

        if (GUILayout.Button("Clear"))
            generator.ClearMap();
    }
}
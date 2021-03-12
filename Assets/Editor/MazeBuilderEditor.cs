using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MazeBuilder))]
public class MazeBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MazeBuilder script = (MazeBuilder)target;
        if (GUILayout.Button("Build Maze")) {
            if (script.MapNumber < 0)
                script.ClearParent();
            else
                script.Build();
        }
    }
}
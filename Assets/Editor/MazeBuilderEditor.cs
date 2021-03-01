using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MazeBuilder))]
public class MazeBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MazeBuilder script = (MazeBuilder)target;
        if (GUILayout.Button("Build Maze"))
            script.Build();
    }
}
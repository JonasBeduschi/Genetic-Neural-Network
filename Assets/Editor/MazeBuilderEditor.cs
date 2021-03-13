using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[CustomEditor(typeof(MazeBuilder))]
public class MazeBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MazeBuilder script = (MazeBuilder)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Maze"))
            script.Build();
        GUILayout.Space(5);
        if (GUILayout.Button("Clear")) {
            script.ClearParent();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        GUILayout.EndHorizontal();
    }
}
using UnityEditor;
using UnityEngine;
using FlappyPlane;

[CustomEditor(typeof(FlappyAgent))]
public class FlappyAgentEditor : Editor
{
    private FlappyAgent script;
    private GUIContent content;

    private SerializedProperty lasers;
    private SerializedProperty layer;
    private SerializedProperty numberOfLasers;
    private SerializedProperty force;

    private int previousLasers;

    private void OnEnable()
    {
        script = (FlappyAgent)target;
        previousLasers = script.numberOfLasers;
    }

    public override void OnInspectorGUI()
    {
        // Debug.Log($"Initializing a new update with {script.lasers.Length}");
        serializedObject.Update();
        //Debug.Log($"Continuing Initialization with {script.lasers.Length}");

        lasers = serializedObject.FindProperty(nameof(script.lasers));
        layer = serializedObject.FindProperty(nameof(script.objectsLayer));
        numberOfLasers = serializedObject.FindProperty(nameof(script.numberOfLasers));
        force = serializedObject.FindProperty(nameof(script.force));

        for (int i = 0; i < script.lasers.Length; i++) {
            if (!script.lasers[i]) {
                //Debug.Log("Couldn't find a laser! Setting up everything again with length " + script.numberOfLasers);
                SetLasers(0);
                SetLasers(script.numberOfLasers);
            }
        }
        //Debug.Log($"Initializing drawing editor with length {script.lasers.Length}");
        DrawEditor();
        //Debug.Log($"Finished drawing editor with length {script.lasers.Length}");
        if (serializedObject.hasModifiedProperties) {
            //Debug.Log($"Will apply modified properties with {script.lasers.Length}");
            serializedObject.ApplyModifiedProperties();
            //Debug.Log($"Applied modified properties with {script.lasers.Length}");
        }
        else {
            //Debug.Log($"Did not apply modified properties at {script.lasers.Length}");
        }
    }

    private void DrawEditor()
    {
        if (Application.isPlaying)
            EditorGUI.BeginDisabledGroup(true);

        // Force
        content = new GUIContent("Force", "The \"Jump\" force of the plane.");
        content = new GUIContent(content);
        EditorGUILayout.PropertyField(force, content, true);

        // Fitness
        content = new GUIContent("Fitness", "The fitness of this Creature, calculated only when dead.");
        EditorGUILayout.LabelField(content, new GUIContent(script.Fitness.ToString()));

        // Layer
        content = new GUIContent("Maze Layer", "The Layer in which walls and obstacles are located.");
        EditorGUILayout.PropertyField(layer, content, true);

        // Number of lasers
        content = new GUIContent("Number of Lasers", "How many detection lasers should the Creature have? One single input is added per laser on the NN.");
        content = new GUIContent(content);
        EditorGUILayout.IntSlider(numberOfLasers, 0, 33, content);

        // Lasers
        if (!Application.isPlaying) EditorGUI.BeginDisabledGroup(true);
        content = new GUIContent("Lasers", "The list of lasers from the Creature, not to be updated manually.");
        EditorGUILayout.PropertyField(lasers, content, true);
        if (!Application.isPlaying) EditorGUI.EndDisabledGroup();

        // In-game data
        if (Application.isPlaying) {
            content = new GUIContent("Output");
            //if (!script.Dead) EditorGUILayout.FloatField(content, script.GetOutput()[0]);
            /*if (script.GetInput().Length > startIndex + 2) {
                EditorGUILayout.LabelField("Up Distance", (script.GetInput()[startIndex + 1] * MazeRunner.EyeDistanceCap).ToString());
                EditorGUILayout.LabelField("Frontal Distance", (script.GetInput()[startIndex] * MazeRunner.EyeDistanceCap).ToString());
                EditorGUILayout.LabelField("Down Distance", (script.GetInput()[startIndex + 2] * MazeRunner.EyeDistanceCap).ToString());
            }
            else if (script.GetInput().Length > startIndex) {
                EditorGUILayout.LabelField("Frontal Distance", (script.GetInput()[startIndex] * MazeRunner.EyeDistanceCap).ToString());
            }*/
            EditorGUI.EndDisabledGroup();
        }
        else {
            // Set lasers
            if (previousLasers != numberOfLasers.intValue) {
                //Debug.Log($"Previous lasers are different: {previousLasers}. Setting to: {numberOfLasers.intValue}");
                SetLasers(numberOfLasers.intValue);
                previousLasers = numberOfLasers.intValue;
            }
        }
    }

    private void SetLasers(int number)
    {
        if (number <= 0) {
            for (int i = 0; i < script.lasers.Length; i++) {
                if (script.lasers[i])
                    DestroyImmediate(script.lasers[i].gameObject);
            }
            script.lasers = new Transform[0];
            return;
        }
        int previous = script.lasers.Length;
        // Destroy excess ones
        for (int i = number; i < previous; i++) {
            DestroyImmediate(script.lasers[i].gameObject);
        }
        // Crop array
        lasers.ClearArray();
        Transform[] array = script.lasers.Copy(number);
        for (int i = 0; i < previous && i < number; i++) {
            lasers.InsertArrayElementAtIndex(i);
            SerializedProperty property = lasers.GetArrayElementAtIndex(i);
            property.objectReferenceValue = array[i];
        }

        for (int i = previous; i < number; i++) {
            GameObject go = new GameObject("Laser " + i);
            lasers.InsertArrayElementAtIndex(i);
            go.transform.parent = script.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Extensions.QuaternionFromDegrees(DegreesForIndex(i) - 90);
            SerializedProperty property = lasers.GetArrayElementAtIndex(i);
            property.objectReferenceValue = go.transform;
        }
        //Debug.Log($"Finished setting lasers at length {script.lasers.Length}!");
    }

    private float DegreesForIndex(int index)
    {
        // Make is so 0 is up and 180 is down
        if (index == 0)
            return 90;
        else if (index == 1)
            return 0;
        else if (index == 2)
            return 180;

        if (index < 5)
            return 45 + (index - 3) * 90;
        else if (index < 9)
            return 22.5f + (index - 5) * 45;
        else if (index < 17)
            return 11.25f + (index - 9) * 22.5f;
        else if (index < 33)
            return 5.625f + (index - 17) * 11.25f;
        else
            return 0;
    }
}
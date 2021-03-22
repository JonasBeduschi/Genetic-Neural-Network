using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeRunner))]
public class MazeRunnerEditor : Editor
{
    private MazeRunner script;
    private GUIContent content;

    private SerializedProperty lasers;
    private SerializedProperty layer;
    private SerializedProperty numberOfLasers;
    private SerializedProperty memoryLength;
    private SerializedProperty memoriesToConsider;

    private int previousLasers;

    private void OnEnable()
    {
        script = (MazeRunner)target;
        previousLasers = script.numberOfLasers;
    }

    public override void OnInspectorGUI()
    {
        // Debug.Log($"Initializing a new update with {script.lasers.Length}");
        serializedObject.Update();
        //Debug.Log($"Continuing Initialization with {script.lasers.Length}");

        lasers = serializedObject.FindProperty(nameof(script.lasers));
        layer = serializedObject.FindProperty(nameof(script.mazeLayer));
        numberOfLasers = serializedObject.FindProperty(nameof(script.numberOfLasers));
        memoryLength = serializedObject.FindProperty(nameof(script.memoryLength));
        memoriesToConsider = serializedObject.FindProperty(nameof(script.memoriesToConsider));

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

        // Fitness
        content = new GUIContent("Fitness", "The fitness of this Creature, calculated only when dead");
        EditorGUILayout.LabelField(content, new GUIContent(script.Fitness.ToString()));

        // Body
        content = new GUIContent("Body", "The actual moving part of the Creature, usually named \"Body\"");
        script.bodyTransform = (Transform)EditorGUILayout.ObjectField(content, script.bodyTransform, typeof(Transform), true);

        // Layer
        content = new GUIContent("Maze Layer", "The Layer in which walls and obstacles are located");
        EditorGUILayout.PropertyField(layer, content, true);

        // Number of lasers
        content = new GUIContent("Number of Lasers", "How many detection lasers should the Creature have? One single input is added per laser on the NN");
        content = new GUIContent(content);
        EditorGUILayout.IntSlider(numberOfLasers, 0, 32, content);

        // Lasers
        if (!Application.isPlaying) EditorGUI.BeginDisabledGroup(true);
        content = new GUIContent("Lasers", "The list of lasers from the Creature, not to be updated manually");
        EditorGUILayout.PropertyField(lasers, content, true);
        if (!Application.isPlaying) EditorGUI.EndDisabledGroup();

        // Memory length
        content = new GUIContent("Memory Length", "How many previous positions should be saved, one per FixedUpdate");
        content = new GUIContent(content);
        EditorGUILayout.IntSlider(memoryLength, 0, 200, content);

        // Memories to consider
        content = new GUIContent("Memories to Consider", "How many previous positions should be considered by the NN. Two inputs are added per position (YZ)");
        EditorGUILayout.IntSlider(memoriesToConsider, 1, 10, content);

        // In-game data
        if (Application.isPlaying) {
            content = new GUIContent("Instant Velocity");
            if (script.GetInput().Length > 0) EditorGUILayout.Vector3Field(content, new Vector3(script.GetInput()[1], script.GetInput()[2], 0));
            int startIndex = 3 + script.memoriesToConsider * 2;
            if (script.GetInput().Length > startIndex + 2) {
                EditorGUILayout.LabelField("Up Distance", (script.GetInput()[startIndex + 1] * MazeRunner.EyeDistanceCap).ToString());
                EditorGUILayout.LabelField("Frontal Distance", (script.GetInput()[startIndex] * MazeRunner.EyeDistanceCap).ToString());
                EditorGUILayout.LabelField("Down Distance", (script.GetInput()[startIndex + 2] * MazeRunner.EyeDistanceCap).ToString());
            }
            else if (script.GetInput().Length > startIndex) {
                EditorGUILayout.LabelField("Frontal Distance", (script.GetInput()[startIndex] * MazeRunner.EyeDistanceCap).ToString());
            }
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
            go.transform.parent = script.bodyTransform.GetChild(0);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Extensions.QuaternionFromDegrees(DegreesForIndex(i));
            SerializedProperty property = lasers.GetArrayElementAtIndex(i);
            property.objectReferenceValue = go.transform;
        }
        //Debug.Log($"Finished setting lasers at length {script.lasers.Length}!");
    }

    private float DegreesForIndex(int index)
    {
        int direction;
        if (index == 0)
            return 0;
        else if (index == 1)
            return -90;
        else if (index == 2)
            return 90;
        else if (index == 3)
            return 180;

        if (index % 2 == 0)
            direction = -1;
        else
            direction = 1;

        if (index < 8)
            return (45 + (index / 2 - 2) * 90) * direction;
        else if (index < 16)
            return (22.5f + (index / 2 - 4) * 45) * direction;
        else if (index < 32)
            return (11.25f + (index / 2 - 8) * 22.5f) * direction;
        else
            return 0;
    }
}
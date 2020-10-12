using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Creature))]
public class CreatureEditor : Editor
{
    private Creature script;
    private GUIContent content;

    private SerializedProperty lasers;
    private SerializedProperty layer;
    private SerializedProperty numberOfLasers;
    private SerializedProperty memoryLength;
    private SerializedProperty memoriesToConsider;

    private int previousLasers;

    private void OnEnable()
    {
        script = (Creature)target;
        previousLasers = script.numberOfLasers;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        lasers = serializedObject.FindProperty(nameof(script.lasers));
        layer = serializedObject.FindProperty(nameof(script.layer));
        numberOfLasers = serializedObject.FindProperty(nameof(script.numberOfLasers));
        memoryLength = serializedObject.FindProperty(nameof(script.memoryLength));
        memoriesToConsider = serializedObject.FindProperty(nameof(script.memoriesToConsider));

        for (int i = 0; i < script.lasers.Length; i++) {
            if (!script.lasers[i]) {
                SetLasers(0);
                SetLasers(script.numberOfLasers);
            }
        }

        DrawEditor();

        serializedObject.ApplyModifiedProperties();
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
        content = new GUIContent("Floor Layer", "The Layer in which walls and obstacles are located");
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
            if (script.inputs.Length > 0) EditorGUILayout.Vector3Field(content, new Vector3(0, script.inputs[0], script.inputs[1]));
            int startIndex = 2 + script.memoriesToConsider * 2;
            if (script.inputs.Length > startIndex + 2) {
                EditorGUILayout.LabelField("Up Distance", script.inputs[startIndex + 1].ToString());
                EditorGUILayout.LabelField("Frontal Distance", script.inputs[startIndex].ToString());
                EditorGUILayout.LabelField("Down Distance", script.inputs[startIndex + 2].ToString());
            }
            else if (script.inputs.Length > startIndex) {
                EditorGUILayout.LabelField("Frontal Distance", script.inputs[startIndex].ToString());
            }
            EditorGUI.EndDisabledGroup();
        }
        else {
            // Set lasers
            if (previousLasers != numberOfLasers.intValue) {
                previousLasers = numberOfLasers.intValue;
                SetLasers(previousLasers);
            }
        }
    }

    private void SetLasers(int number)
    {
        int previous = script.lasers.Length;
        for (int i = number; i < previous; i++) {
            if (script.lasers[i])
                DestroyImmediate(script.lasers[i].gameObject);
        }
        script.lasers.CopyInto(ref script.lasers, number);

        for (int i = previous; i < number; i++) {
            script.lasers[i] = new GameObject("Laser " + i).transform;
            script.lasers[i].parent = script.bodyTransform.GetChild(0);
            script.lasers[i].localPosition = Vector3.zero;
            script.lasers[i].localRotation = Extensions.QuaternionFromDegrees(DegreesForIndex(i));
        }
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
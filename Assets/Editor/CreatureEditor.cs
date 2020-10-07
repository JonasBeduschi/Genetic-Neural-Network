using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Creature))]
public class CreatureEditor : Editor
{
    Creature script;
    GUIContent content;

    SerializedProperty lasers;
    SerializedProperty layer;
    SerializedProperty numberOfLasers;
    SerializedProperty memoryLength;
    SerializedProperty memoriesToConsider;

    int previousLasers;

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

        // Fitness
        EditorGUILayout.LabelField("Fitness", script.Fitness.ToString());
        // Head Transform
        if (Application.isPlaying) {
            EditorGUI.BeginDisabledGroup(true);
            script.bodyTransform = (Transform)EditorGUILayout.ObjectField("body", script.bodyTransform, typeof(Transform), true);
            EditorGUI.EndDisabledGroup();
        }
        else
            script.bodyTransform = (Transform)EditorGUILayout.ObjectField("body", script.bodyTransform, typeof(Transform), true);
        // Layer
        if (Application.isPlaying)
            EditorGUILayout.LabelField("Layer", script.layer.ToString());
        else {
            content = new GUIContent("Layer");
            EditorGUILayout.PropertyField(layer, content, true);
        }
        // Number of Lasers
        if (Application.isPlaying)
            EditorGUILayout.LabelField("Number of Lasers", script.numberOfLasers.ToString());
        else {
            content = new GUIContent("Number of Lasers");
            EditorGUILayout.IntSlider(numberOfLasers, 0, 32, content);
        }
        // Lasers
        EditorGUI.BeginDisabledGroup(true);
        content = new GUIContent("Lasers");
        EditorGUILayout.PropertyField(lasers, content, true);
        EditorGUI.EndDisabledGroup();
        // Memory length
        if (Application.isPlaying)
            EditorGUILayout.LabelField("Memory Length", script.memoryLength.ToString());
        else {
            content = new GUIContent("Memory Length");
            EditorGUILayout.IntSlider(memoryLength, 0, 200, content);
        }
        // Skip how many memories
        if (Application.isPlaying)
            EditorGUILayout.LabelField("Memories to Consider", script.memoriesToConsider.ToString());
        else {
            content = new GUIContent("Memories to Consider");
            EditorGUILayout.IntSlider(memoriesToConsider, 1, 10, content);
        }

        if (Application.isPlaying) {
            EditorGUI.BeginDisabledGroup(true);
            content = new GUIContent("Instant Velocity");
            EditorGUILayout.Vector3Field(content, new Vector3(0, script.inputs[0], script.inputs[1]));
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

        serializedObject.ApplyModifiedProperties();

        if (previousLasers != numberOfLasers.intValue) {
            previousLasers = numberOfLasers.intValue;
            SetLasers(previousLasers);
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
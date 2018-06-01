using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompanionObjective))]
[CanEditMultipleObjects]
public class ObjectiveEditor : Editor {

    SerializedProperty objectiveTypeProp;
    SerializedProperty objectiveTaskProp;
    SerializedProperty instructionClipProp;
    SerializedProperty reinforcementClipProp;
    SerializedProperty reinforcementIntervalProp;

    SerializedProperty trashAmountProp;

    private CompanionObjective _objective;

    public void OnEnable() {
        objectiveTypeProp = serializedObject.FindProperty("objectiveType");
        objectiveTaskProp = serializedObject.FindProperty("objectiveTask");
        instructionClipProp = serializedObject.FindProperty("instructionClip");
        reinforcementClipProp = serializedObject.FindProperty("reinforcementClip");
        reinforcementIntervalProp = serializedObject.FindProperty("reinforcementInterval");

        trashAmountProp = serializedObject.FindProperty("trashAmount");

        _objective = (CompanionObjective)target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(objectiveTypeProp, new GUIContent("Objective Type"));

        ObjectiveType type = (ObjectiveType)objectiveTypeProp.enumValueIndex;
        ObjectiveTask task = (ObjectiveTask)objectiveTaskProp.enumValueIndex;

        switch(type) {
            case ObjectiveType.Main:
                EditorGUILayout.PropertyField(objectiveTaskProp, new GUIContent("Objective Task"));

                switch (task) {
                    case ObjectiveTask.Cleanup:
                        EditorGUILayout.PropertyField(trashAmountProp, new GUIContent("Trash Amount"));
                        break;

                    case ObjectiveTask.Place:
                        break;

                    case ObjectiveTask.PowerOn:
                        break;

                    default:
                        break;
                }
                
                break;

            case ObjectiveType.Side:
                _objective.objectiveTask = ObjectiveTask.SideTask;
                break;

            default:
                break;
        }

        EditorGUILayout.PropertyField(instructionClipProp, new GUIContent("Instruction Clip"));
        EditorGUILayout.PropertyField(reinforcementClipProp, new GUIContent("Reinforcement Clip"));
        EditorGUILayout.PropertyField(reinforcementIntervalProp, new GUIContent("Reinforcement Interval"));

        //apply changes
        serializedObject.ApplyModifiedProperties();

    }
}

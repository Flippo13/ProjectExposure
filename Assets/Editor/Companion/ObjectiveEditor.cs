using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompanionObjective))]
[CanEditMultipleObjects]
public class ObjectiveEditor : Editor {

    //default props
    SerializedProperty objectiveTypeProp;
    SerializedProperty objectiveTaskProp;
    SerializedProperty animationTriggerProp;
    SerializedProperty instructionClipProp;
    SerializedProperty reinforcementClipProp;
    SerializedProperty reinforcementIntervalProp;

    //task dependant props
    SerializedProperty trashAmountProp;
    SerializedProperty sceneTransitionProp;
    SerializedProperty dropdownZonesProp;
    SerializedProperty turbineProp;
    SerializedProperty powerGridProp;
    SerializedProperty turbineButtonProp;

    private CompanionObjective _objective;

    public void OnEnable() {
        objectiveTypeProp = serializedObject.FindProperty("objectiveType");
        objectiveTaskProp = serializedObject.FindProperty("objectiveTask");
        animationTriggerProp = serializedObject.FindProperty("animationTrigger");
        instructionClipProp = serializedObject.FindProperty("instructionClip");
        reinforcementClipProp = serializedObject.FindProperty("reinforcementClip");
        reinforcementIntervalProp = serializedObject.FindProperty("reinforcementInterval");

        trashAmountProp = serializedObject.FindProperty("trashAmount");
        sceneTransitionProp = serializedObject.FindProperty("sceneTransition");
        dropdownZonesProp = serializedObject.FindProperty("dropdownZones");
        turbineProp = serializedObject.FindProperty("turbine");
        powerGridProp = serializedObject.FindProperty("powerGrid");
        turbineButtonProp = serializedObject.FindProperty("turbineButton");

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

                    case ObjectiveTask.Choose:
                        //array
                        EditorGUILayout.PropertyField(dropdownZonesProp, new GUIContent("Dropdown Zones"));

                        for (int i = 0; i < dropdownZonesProp.arraySize; i++) {
                            EditorGUILayout.PropertyField(dropdownZonesProp.GetArrayElementAtIndex(i), new GUIContent("Zone " + (i + 1)));
                        }

                        break;

                    case ObjectiveTask.Place:
                        EditorGUILayout.PropertyField(turbineProp, new GUIContent("Dropping Turbine"));

                        break;

                    case ObjectiveTask.PlugIn:
                        EditorGUILayout.PropertyField(powerGridProp, new GUIContent("Power Grid"));
                        
                        break;

                    case ObjectiveTask.PowerOn:
                        EditorGUILayout.PropertyField(turbineButtonProp, new GUIContent("Turbine Button"));

                        break;

                    case ObjectiveTask.Assemble:

                        break;

                    case ObjectiveTask.NextLevel:
                        EditorGUILayout.PropertyField(sceneTransitionProp, new GUIContent("Scene Transition"));

                        break;

                    default:
                        break;
                }
                
                break;

            case ObjectiveType.Side:
                _objective.objectiveTask = ObjectiveTask.Event;
                break;

            default:
                break;
        }

        EditorGUILayout.PropertyField(animationTriggerProp, new GUIContent("Animation Trigger"));

        if(task != ObjectiveTask.Place) {
            //default properties
            EditorGUILayout.PropertyField(instructionClipProp, new GUIContent("Instruction Clip"));
            EditorGUILayout.PropertyField(reinforcementClipProp, new GUIContent("Reinforcement Clip"));
            EditorGUILayout.PropertyField(reinforcementIntervalProp, new GUIContent("Reinforcement Interval"));
        }

        //apply changes
        serializedObject.ApplyModifiedProperties();

    }
}

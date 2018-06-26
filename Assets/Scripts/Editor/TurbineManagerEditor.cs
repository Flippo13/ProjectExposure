using System.Collections;
using System.Collections.Generic;
using UnityEditor; 
using UnityEngine;

[CustomEditor(typeof(TurbineManager))]
[CanEditMultipleObjects]
public class TurbineManagerEditor : Editor {

    SerializedProperty turbine;
    
    void OnEnable()
    {
        turbine = serializedObject.FindProperty("turbine"); 
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(turbine, new GUIContent("Set Up Turbines"), true); 
        serializedObject.ApplyModifiedProperties();

    }

}

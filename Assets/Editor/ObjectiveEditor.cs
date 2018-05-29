using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompanionObjective))]
[CanEditMultipleObjects]
public class ObjectiveEditor : Editor {

    SerializedProperty objectiveTypeProp;
    SerializedProperty objectiveTaskProp;
	
}

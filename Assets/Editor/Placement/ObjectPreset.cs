using UnityEngine;

public class ObjectPreset : ScriptableObject
{
    public GameObject gameObject;
    public string presetName;
    [HideInInspector]
    public string tag = "Untagged";
    [HideInInspector]
    public int layer;
    public bool isStatic = true;
    [HideInInspector]
    public float opacity;

    public float offset;

    //Uniform scaling
    public bool uniformScaling;
    [HideInInspector]
    public float uniformScaleMin;
    [HideInInspector]
    public float uniformScaleMax;

    // 0 = x, 1 = y, 2 = z
    public Vector3 minRotations;
    public Vector3 maxRotations;
    public Vector3 minScaling = new Vector3(1, 1, 1);
    public Vector3 maxScaling = new Vector3(1, 1, 1);

    // 0 = x, 1 = y, 2 = z
    [HideInInspector]
    public bool[] rotationBooleans = new bool[] { false, false, false };
    [HideInInspector]
    public bool[] scalingBooleans = new bool[] { false, false, false };
}
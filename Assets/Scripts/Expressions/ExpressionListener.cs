using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionListener : MonoBehaviour
{
    [SerializeField]
    private Material _faceMaterial;

    public void ChangeExpression(string name)
    {
        // Check if string isn't empty and whether it exist in the Expression enum
        if (name != "" && Enum.IsDefined(typeof(Expressions), name))
        {
            _faceMaterial.SetFloat("_Expression", (int)Enum.Parse(typeof(Expressions), name));
        }
        else { Debug.Log("Expression string is empty or doesn't exist "); }
    }

    public void ChangeColor(string color)
    {
        if (color != "")
            _faceMaterial.SetColor("_Color", (Color)typeof(Color).GetProperty(color.ToLowerInvariant()).GetValue(null, null));
        else { Debug.Log("Color string is empty or doesn't exist (perhaps not a default Unity color)"); }
    }
}

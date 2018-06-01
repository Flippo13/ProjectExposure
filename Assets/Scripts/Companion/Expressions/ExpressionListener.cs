using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionListener : MonoBehaviour
{
    [SerializeField]
    private Material _faceMaterial;
    [SerializeField]
    public List<Expression> _expressions;

    [Serializable]
    public class Expression
    {
        public string _expressionName;
        public Color _color;
        public Expressions _expression;
        public float _emissionStrength;
    }

    public void ChangeExpression(string expressionName)
    {
        Expression newExpression = GetExpressionData(expressionName);
        if (newExpression != null)
        {
            _faceMaterial.SetFloat("_Expression", (int)newExpression._expression);
            _faceMaterial.SetFloat("_EmissionStrenght", newExpression._emissionStrength);
            _faceMaterial.SetColor("_Color", newExpression._color);
        }
        else
            Debug.Log(expressionName + " doesn't seem to exist in the Expression Listener");
    }

    private Expression GetExpressionData(string expressionName)
    {
        for (int i = 0; i < _expressions.Count; i++)
        {
            if (expressionName == _expressions[i]._expressionName)
                return _expressions[i];
        }
        return null;
    }

}

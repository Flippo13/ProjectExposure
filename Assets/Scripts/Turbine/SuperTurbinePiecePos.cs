using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTurbinePiecePos : MonoBehaviour {


    public TurbinePiecePosition[] turbinePiecesPosition; 

    [SerializeField]
    private Color notConnectedColor;
    [SerializeField]
    private Color connectedColor;

    void Start()
    {
        SetColor(); 
    }


    private void SetColor()
    {
        for (int i = 0; i < turbinePiecesPosition.Length; i++)
        {
            turbinePiecesPosition[i].notConnectedColor = notConnectedColor;
            turbinePiecesPosition[i].connectedColor = connectedColor;
        }
    }
}

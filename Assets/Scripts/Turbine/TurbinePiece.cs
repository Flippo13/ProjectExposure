using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiece : MonoBehaviour {

    public enum Piece {TrubineBlade, TurbineFoundation};
    public Piece piece;

    public GameObject turbinePiecePosition; 

    public bool connected;

    private Renderer rend;
    private bool _grabbed; 

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>(); 

		if(this.tag == "TurbinePiecePosition")
        {
            rend.material.color = new Color(0.3f, 0.8f,0.4f, 0.31f);
            connected = false; 
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        rend.material.color = new Color(0.3f, 0.8f, 0.4f, 1.0f);
        connected = true; 
    }

    public void Kill()
    {
        Destroy(this); 
    }

}

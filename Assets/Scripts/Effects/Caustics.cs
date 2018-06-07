using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caustics : MonoBehaviour
{

    [SerializeField]
    private float _interval = 0.045f;
    [SerializeField]
    private float _minimumLight = 2;

    [SerializeField]
    private List<Texture2D> _causticTextures;


    private Light _caustics;
    private int currentTexture;

    private void Awake()
    {
        _caustics = GetComponent<Light>();
        InvokeRepeating("ChangeSprite", 0, _interval);
    }

    private void FixedUpdate()
    {
        _caustics.intensity = Mathf.Sin(Time.time) + _minimumLight;
    }


    void ChangeSprite()
    {
        _caustics.cookie = _causticTextures[currentTexture];
        //Reset cycle if reached the end of the spritesheet
        currentTexture++;
        if (currentTexture == _causticTextures.Count)
            currentTexture = 0;
        
        Debug.Log(currentTexture);
    }
}

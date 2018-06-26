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
    private float _sinMultiplier = 2;

    [SerializeField]
    private List<Texture2D> _causticTextures;


    private Light _caustics;
    private int currentTexture;
    private bool invert = false;

    private void Awake()
    {
        _caustics = GetComponent<Light>();
        InvokeRepeating("ChangeSprite", 0, _interval);
    }

    private void FixedUpdate()
    {
        _caustics.intensity = Mathf.Sin(Time.time * (1 + _sinMultiplier)) + _minimumLight;
    }


    void ChangeSprite()
    {
        //_caustics.cookie = _causticTextures[currentTexture];
        //currentTexture++;
        ////Reset cycle if reached the end of the spritesheet
        //if (currentTexture == _causticTextures.Count)
        //    currentTexture = 0;

        if (!invert)
        {
            _caustics.cookie = _causticTextures[currentTexture];
            currentTexture++;
            if (currentTexture == _causticTextures.Count)
            {
                currentTexture = _causticTextures.Count - 1;
                invert = true;
            }
        }
        else
        {
            _caustics.cookie = _causticTextures[currentTexture];
            currentTexture--;
            if (currentTexture < 0)
            {
                currentTexture = 0;
                invert = false;
            }
        }

    }
}

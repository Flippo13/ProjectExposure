using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedScreen : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> _sprites = new List<Sprite>();
    [SerializeField]
    private float _spriteSpeed;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _spriteRenderer.sprite = _sprites[(int)(Time.time * _spriteSpeed) % _sprites.Count];
    }
}

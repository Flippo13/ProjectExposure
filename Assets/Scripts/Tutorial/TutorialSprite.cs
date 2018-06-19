using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TutorialSprite : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _sprites = new List<Sprite>();

    [SerializeField]
    private float _spriteSpeed;

    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;

    private GameObject _parent;

    //float _pingPongFloat;
    //private Color _buttonColor;
    //private Material _buttonMaterial;
  
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _parent = transform.parent.gameObject;

        _lineRenderer.SetPosition(0, _parent.transform.position);
        _lineRenderer.SetPosition(1, _parent.transform.parent.position);
        //_buttonMaterial = transform.parent.GetComponent<Renderer>().material;
    }
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        _lineRenderer.SetPosition(0, _parent.transform.position);
        _lineRenderer.SetPosition(1, _parent.transform.parent.position);

        // Animate sprite
        _spriteRenderer.sprite = _sprites[(int)(Time.time * _spriteSpeed) % _sprites.Count];
    }

    //private void LateUpdate()
    //{
    //    // Ping pong the button so it pops out
    //    _pingPongFloat = Mathf.PingPong(Time.time, 1);
    //    _buttonColor = Color.Lerp(Color.white, Color.blue, _pingPongFloat);
    //    _buttonMaterial.color = _buttonColor;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TutorialSprite : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;

    float _pingPongFloat;
    private Color _buttonColor;
    private Material _buttonMaterial;

    private bool activated;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, new Vector3(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.min.y, _spriteRenderer.bounds.min.z));
        _lineRenderer.SetPosition(1, transform.parent.position);

        _buttonMaterial = transform.parent.GetComponent<Renderer>().material;
    }
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.parent.position);
    }

    private void LateUpdate()
    {
        // Ping pong the button so it pops out
        _pingPongFloat = Mathf.PingPong(Time.time, 1);
        _buttonColor = Color.Lerp(Color.white, Color.blue, _pingPongFloat);
        _buttonMaterial.color = _buttonColor;
    }
}

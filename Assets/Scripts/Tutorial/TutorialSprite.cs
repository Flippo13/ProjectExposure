using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TutorialSprite : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer.SetPosition(0, new Vector3(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.min.y, _spriteRenderer.bounds.min.z));
        _lineRenderer.SetPosition(1, transform.parent.position);
    }
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        _lineRenderer.SetPosition(0, new Vector3(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.min.y, _spriteRenderer.bounds.min.z));
        _lineRenderer.SetPosition(1, transform.parent.position);
    }
}

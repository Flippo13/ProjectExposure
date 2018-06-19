using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TutorialSprite : MonoBehaviour
{
    public bool useParent;
    public GameObject _Button;
    private Color _baseColor;
    private Color _highlightedColor;


    [SerializeField]
    private List<Sprite> _sprites = new List<Sprite>();

    [SerializeField]
    private float _spriteSpeed;

    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;

    private GameObject _parent;

    float _pingPongFloat;
    private Color _buttonColor;
    private Material _buttonMaterial;

    private void Awake()
    {
        _baseColor = new Color(0.86f, 0.57f, 0, 1);
        _highlightedColor = new Color(0.86f, 0.57f, 0, 0.8f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _parent = transform.parent.gameObject;

        _lineRenderer.SetPosition(0, _parent.transform.position);
        _lineRenderer.SetPosition(1, _parent.transform.parent.position);
        _buttonMaterial = _Button.GetComponent<Renderer>().material;
    }
    void Update()
    {
        if (useParent)
        {
            Vector3 deltaVec = Camera.main.transform.position - transform.parent.position;
            transform.parent.rotation = Quaternion.LookRotation(deltaVec);
        }
        else
        {
            Vector3 deltaVec = Camera.main.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(deltaVec);
        }

        _lineRenderer.SetPosition(0, _parent.transform.position);
        _lineRenderer.SetPosition(1, _parent.transform.parent.position);

        // Animate sprite
        _spriteRenderer.sprite = _sprites[(int)(Time.time * _spriteSpeed) % _sprites.Count];
    }

    private void LateUpdate()
    {
        // Ping pong the button so it pops out
        _pingPongFloat = Mathf.PingPong(Time.time * 3, 1);
        _buttonColor = Color.Lerp(_baseColor, _highlightedColor, _pingPongFloat);
        _buttonMaterial.color = _buttonColor;
    }
}

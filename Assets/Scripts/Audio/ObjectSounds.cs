using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSounds : MonoBehaviour
{

    [SerializeField]
    private SurfaceTypes _surface;

    [SerializeField]
    private SoundStrength _strength;
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            SoundHandler.PlayOneShot(_strength, _surface, this.gameObject, _rigidbody.velocity.magnitude, _boxCollider.bounds.size, true);
        else
            SoundHandler.PlayOneShot(_strength, _surface, this.gameObject, _rigidbody.velocity.magnitude, _boxCollider.bounds.size, false);

    }
}

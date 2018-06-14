using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSounds : MonoBehaviour
{

    [SerializeField]
    private SurfaceTypes _surface;

    [SerializeField]
    private SoundStrength _strength;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            SoundHandler.PlayOneShot(_strength, _surface, this.gameObject, true);
        else
            SoundHandler.PlayOneShot(_strength, _surface, this.gameObject, false);

    }
}

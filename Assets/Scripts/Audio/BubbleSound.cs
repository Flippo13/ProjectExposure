using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSound : MonoBehaviour {

    [SerializeField]
    [FMODUnity.EventRef]
    private string _BubbleStart;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _BubblePop;


    private ParticleSystem.Particle[] _particles;
    private ParticleSystem _particlesSystem;


    private void Awake()
    {
        _particlesSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particlesSystem.main.maxParticles];

        FMODUnity.RuntimeManager.PlayOneShot(_BubbleStart, transform.position);
    }


    void FixedUpdate()
    {
        int aliveParticles = _particlesSystem.GetParticles(_particles);

        for (int i = 0; i < aliveParticles; i++)
        {
            if (_particles[i].remainingLifetime < 0.1f)
                FMODUnity.RuntimeManager.PlayOneShot(_BubblePop, _particles[i].position);
        }
    }
}

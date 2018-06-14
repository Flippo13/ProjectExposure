using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSound : MonoBehaviour
{

    [SerializeField]
    [FMODUnity.EventRef]
    private string _BubbleStart;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _BubblePop;


    private ParticleSystem.Particle[] _particles;
    private ParticleSystem _particlesSystem;

    private List<ParticleSystem.Particle> insideList;

    private bool inside = true;

    private void Awake()
    {
        insideList = new List<ParticleSystem.Particle>();

        _particlesSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particlesSystem.main.maxParticles];

        FMODUnity.RuntimeManager.PlayOneShot(_BubbleStart, transform.position);
    }

    private void Update()
    {
        var trigger = _particlesSystem.trigger;
        trigger.inside = inside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
    }
    private void OnParticleTrigger()
    {
        if (inside)
        {
            insideList.Clear();
            int numInside = _particlesSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);

            for (int i = 0; i < numInside; i++)
            {
                if (insideList[i].remainingLifetime < 0.05f)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_BubblePop, _particles[i].position);
                }
            }
            _particlesSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);
        }
    }
}

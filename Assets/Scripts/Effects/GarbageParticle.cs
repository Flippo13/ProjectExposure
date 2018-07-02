using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageParticle : MonoBehaviour
{

    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _particle;

    private int minimumRange = 12;
    private int _particleAmount;
    private int _trashAmount;

    private ParticleSystem _particleSystem;
    private bool _enabled = true;

    // Use this for initialization
    void Start()
    {

        _particleSystem = _particle.GetComponent<ParticleSystem>();
        _particleAmount = _particleSystem.main.maxParticles;
        _trashAmount = transform.childCount - 4;

        InvokeRepeating("ChangeTrash", 0, 0.2f);
    }

    public void ChangeTrash()
    {
        if (_enabled)
        {
            // Check if player is in range
            if (Vector3.Distance(transform.position, _player.transform.position) < minimumRange)
            {
                var main = _particleSystem.main;
                main.maxParticles = Mathf.RoundToInt((((float)transform.childCount - 4) / (float)_trashAmount) * _particleAmount);
                if (main.maxParticles <= 0)
                {
                    _particle.SetActive(false);
                    _enabled = false;
                }
            }
        }
    }
}

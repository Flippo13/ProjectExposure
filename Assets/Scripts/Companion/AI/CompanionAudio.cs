using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAudio : MonoBehaviour {

    public AudioClip motorSound;

    private AudioSource[] _audioSources;

    public void Awake() {
        _audioSources = GetComponents<AudioSource>();

        if (_audioSources.Length != 2) Debug.LogWarning("Warning: Companion needs 2 audio sources");

        SetClip(motorSound, AudioSourceType.Effects);
        //PlayAudioSource(AudioSourceType.Effects, true);
    }

    public void SetClip(AudioClip clip, AudioSourceType sourceType) {
        _audioSources[(int)sourceType].clip = clip;
    }

    public void PlayAudioSource(AudioSourceType sourceType, bool loop = false) {
        _audioSources[(int)sourceType].Play();
    }

    public void StopAudioSource(AudioSourceType sourceType) {
        _audioSources[(int)sourceType].Stop();
    }

    public bool IsPlaying(AudioSourceType sourceType) {
        return _audioSources[(int)sourceType].isPlaying;
    }

}

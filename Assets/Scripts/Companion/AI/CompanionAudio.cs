using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CompanionAudio : MonoBehaviour {

    [EventRef]
    public string motorSound;

    private EventInstance[] _audioTracks;

    public void Awake() {
        _audioTracks = new EventInstance[2];

        SetClip(motorSound, AudioSourceType.Effects);
        //PlayAudioSource(AudioSourceType.Effects);
    }

    public void SetClip(string eventPath, AudioSourceType sourceType) {
        _audioTracks[(int)sourceType] = RuntimeManager.CreateInstance(eventPath);
        _audioTracks[(int)sourceType].set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
    }

    public void PlayAudioSource(AudioSourceType sourceType) {
        _audioTracks[(int)sourceType].start();
    }

    public void StopAudioSource(AudioSourceType sourceType) {
        _audioTracks[(int)sourceType].stop(STOP_MODE.IMMEDIATE);
    }

    public PLAYBACK_STATE IsPlaying(AudioSourceType sourceType) {
        PLAYBACK_STATE result;
        _audioTracks[(int)sourceType].getPlaybackState(out result);

        return result;
    }

}

using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField]
    private float _minVelocity = 0.1f;
    [SerializeField]
    private float _velocityDivider = 8;

    private static SoundHandler _soundHandler = null;

    [Header("Surface events")]
    public Surfaces[] _surfaceTypes = new Surfaces[9];

    [FMODUnity.EventRef]
    public string _softSandImpact;

    [FMODUnity.EventRef]
    public string _hardSandImpact;
    private static bool _debug;

    [System.Serializable]
    public class Surfaces
    {
        public SurfaceTypes surface;
        [FMODUnity.EventRef]
        public string hardImpact;
        [FMODUnity.EventRef]
        public string softImpact;
    }

    private void Awake()
    {
        if (_soundHandler != null && _soundHandler != this)
        {
            Destroy(this.gameObject);
        }
        _soundHandler = this;
    }


    public static void PlayOneShot(SoundStrength strenght, SurfaceTypes surface, GameObject gameObject, float magnitude, Vector3 size, bool terrain)
    {
        float volume;

        // Depending the volume on the magnitude
        volume = magnitude / _soundHandler._velocityDivider;
        volume = volume * (size.normalized.x + size.normalized.y);

        // Clamp to make sure we're not making it too loud
        volume = Mathf.Clamp01(volume);

        // If volume is barely audable, return.
        if (volume < _soundHandler._minVelocity)
            return;

        // Debugging
        if (_debug)
            Debug.Log("Playing " + _soundHandler.getEvent(strenght, surface) + " with volume:" + volume);

        // Create the FMOD instance
        FMOD.Studio.EventInstance soundInstance = RuntimeManager.CreateInstance(_soundHandler.getEvent(strenght, surface));
        soundInstance.setVolume(volume);
        soundInstance.set3DAttributes((RuntimeUtils.To3DAttributes(gameObject.transform.position)));

        // Start sound
        soundInstance.start();
        soundInstance.release();

        //if(terrain)

    }

    private string getEvent(SoundStrength strenght, SurfaceTypes surface)
    {
        string eventString = "";
        for (int i = 0; i < _surfaceTypes.Length; i++)
        {
            if (_surfaceTypes[i].surface == surface)
            {
                if (strenght == SoundStrength.Hard)
                    return _surfaceTypes[i].hardImpact;
                else
                    return _surfaceTypes[i].softImpact;
            }
        }
        return eventString;
    }
}

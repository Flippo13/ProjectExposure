using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{

    private static SoundHandler _soundHandler = null;

    public Surfaces[] _surfaceTypes = new Surfaces[9];

    [FMODUnity.EventRef]
    public string _softSandImpact;

    [FMODUnity.EventRef]
    public string _hardSandImpact;

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


    public static void PlayOneShot(SoundStrength strenght, SurfaceTypes surface, GameObject gameObject, bool terrain)
    {
        // Play initial impact
        FMODUnity.RuntimeManager.PlayOneShotAttached(_soundHandler.getEvent(strenght, surface), gameObject);

        // If we hit the terrain, we should play some extra 'sand' impacts.
        if (terrain)
        {
            if (strenght == SoundStrength.Hard)
                FMODUnity.RuntimeManager.PlayOneShot(_soundHandler._softSandImpact, gameObject.transform.position);
            else
                // Currently just use softSand because I couldn't find hardsand sample.
                FMODUnity.RuntimeManager.PlayOneShot(_soundHandler._softSandImpact, gameObject.transform.position);
        }
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

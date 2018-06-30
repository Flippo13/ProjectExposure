using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VacuumScript : MonoBehaviour {

    public Transform vacuumBackAnchor;
    public Transform vacuumHandAnchor;
    public VacuumArea vacuumArea;
    public Text trashCounter; 

    [SerializeField]
    [EventRef]
    private string _vacuumSound;

    [SerializeField]
    [EventRef]
    private string _addScoreSound;

    public float suckSpeed;
    public ObjectGrabber vacuumGrabber;
    public ObjectGrabber leftHandGrabber;
    public float deformationStep;
    public float scaleFactor;

    public GameObject _particleParent;

    private int _trashCount;

    private List<Transform> _destroyedObjects;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private VacuumState _state = VacuumState.CompanionBack;
    private Renderer _renderer;

    private FMOD.Studio.EventInstance _vacuumInstance;

    private bool _soundPlayed;

    // Use this for initialization
    void Awake() {
        _destroyedObjects = new List<Transform>();
        _trashCount = 0;
        trashCounter.text = "" + _trashCount;

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _rigidbody.useGravity = true;

        _vacuumInstance = RuntimeManager.CreateInstance(_vacuumSound);

        _renderer = GetComponent<Renderer>();
    }

    public void OnTriggerEnter(Collider other) {
        if (_state != VacuumState.Player) return; //only execute in player state

        if (other.gameObject.layer == Layers.Suckable && !_destroyedObjects.Contains(other.transform)) {
            _trashCount++;

            //apply to score tracker
            ScoreTracker.Score = _trashCount;
            // Play sound after each 
            RuntimeManager.PlayOneShot(_addScoreSound, transform.position);
            trashCounter.text = "" + _trashCount;
            _destroyedObjects.Add(other.transform);
        }
    }

    public void Update() {
        if(_state == VacuumState.Player && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5) {
            //player holds the vacuum and pressed the right index finger trigger button
            byte[] samples = { 200 }; //1 haptic sample with the strength of 200 (range: 0-255)
            OVRHaptics.RightChannel.Preempt(new OVRHapticsClip(samples, 1)); //play the vibration clip

            MoveTrash();
        } else if(_state == VacuumState.Player && !vacuumGrabber.IsHoldingObject()) {
            //player released the vacuum 
            SetVacuumState(VacuumState.Free);
        } else if(_state != VacuumState.Player && vacuumGrabber.InVacuumMode() && vacuumGrabber.IsHoldingObject()) {
            //player grabs vacuum whenever he wants
            SetVacuumState(VacuumState.Player);
        }

        if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.5 && _soundPlayed) {
            _vacuumInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _soundPlayed = false;

            // Disable Particles
            for (int i = 0; i < _particleParent.transform.childCount; i++)
            {
                _particleParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        } 
    }

    public void SetVacuumState(VacuumState state) {
        _state = state;

        Debug.Log("Entering Vaccum state: " + state);

        switch (state) {

            case VacuumState.CompanionBack:
                //attach to companion back and reset
                transform.parent = vacuumBackAnchor;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;

                _renderer.material.SetFloat("_Outline", 3f);

                break;

            case VacuumState.CompanionHand:
                //attach to companion hand an reset
                transform.parent = vacuumHandAnchor;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;

                _renderer.material.SetFloat("_Outline", 3f);

                break;

            case VacuumState.Player:
                //release
                transform.parent = null;

                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;

                _renderer.material.SetFloat("_Outline", 0f);

                break;

            case VacuumState.Free:
                //release
                transform.parent = null;

                _collider.isTrigger = false;
                _rigidbody.isKinematic = false;

                _renderer.material.SetFloat("_Outline", 3f);

                break;

            default:
                break;
        }
    }

    public void MoveTrash() {
        if(!_soundPlayed) {
            _vacuumInstance.start();
            _soundPlayed = true;

            // Enable Particles
            for (int i = 0; i < _particleParent.transform.childCount; i++)
            {
                _particleParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        //update sound pos
        _vacuumInstance.set3DAttributes((RuntimeUtils.To3DAttributes(transform)));

        if (vacuumArea.suckableObjectsList.Count == 0) return;

        //expensive loop (better: make a suckable object script and cache components that need to be accessed)
        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++) {
            Vector3 suckDir = (transform.position - vacuumArea.suckableObjectsList[i].position).normalized;
            Transform currentTransform = vacuumArea.suckableObjectsList[i];
            Renderer currentRenderer = currentTransform.GetComponent<Renderer>();
            Rigidbody currentRigidbody = currentTransform.GetComponent<Rigidbody>();

            currentRigidbody.isKinematic = true;

            //translate to vacuum gun collider and scale down
            currentTransform.position += suckDir * suckSpeed;
            currentTransform.localScale = currentTransform.localScale * scaleFactor;

            //apply deformation
            float newDeform = Mathf.Clamp01(currentRenderer.material.GetFloat("_Deform") + deformationStep);
            currentRenderer.material.SetFloat("_Deform", newDeform);

            currentRigidbody.isKinematic = false;
        }

        //clean up the grabber and vacuum area lists, then destroy the object
        for (int i = 0; i < _destroyedObjects.Count; i++) {
            vacuumArea.RemoveTransfromFromList(_destroyedObjects[i]);
            vacuumGrabber.RemoveGrabCandidate(_destroyedObjects[i]);
            leftHandGrabber.RemoveGrabCandidate(_destroyedObjects[i]);
            Destroy(_destroyedObjects[i].gameObject);
        }

        _destroyedObjects.Clear();
    }

    public int GetTrashCount() {
        return _trashCount;
    }

    public VacuumState GetVacuumState() {
        return _state;
    }
}

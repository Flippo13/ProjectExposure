using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class WaterConditionManager : MonoBehaviour {

    [Header("Water Phyiscs")]
    public float gravityMultiplier;

    [Header("Condition Tracking")]
    public Transform trashHolder;

    [Header("Post Processing")]
    public PostProcessingProfile dirtyWaterPP;
    public PostProcessingProfile cleanWaterPP;
    public PostProcessingProfile currentWaterPP;

    [Header("Fog")]
    public Color dirtyWaterFogColor;
    public Color cleanWaterFogColor;

    [Header("Skybox")]
    public Color dirtyWaterSkyboxTint;
    public Color cleanWaterSkyboxTint;

    private ColorGradingModel.Settings _dirtyColorGrading;
    private ColorGradingModel.Settings _cleanColorGrading;
    private Color _currentFogColor;
    private Color _currentSkyboxTint;

    private int _totalTrashCount;
    private float _prevIncrementor;
    private float _incrementor;

    public void Awake() {
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = currentWaterPP; //apply PP to the camera

        Physics.gravity = Physics.gravity * gravityMultiplier; //adjust game physics

        _dirtyColorGrading = dirtyWaterPP.colorGrading.settings;
        _cleanColorGrading = cleanWaterPP.colorGrading.settings;

        _totalTrashCount = trashHolder.GetComponentsInChildren<Collider>().Length; //count trash

        _prevIncrementor = 0f;
        _incrementor = 0f;

        ApplyWaterCondition();
    }

    public void Update() {
        //update incrementor when sucking trash
        if (SceneManager.GetActiveScene().buildIndex == 0) _incrementor = (float)ScoreTracker.ScoreLevel1 / (float)_totalTrashCount; //cast to float to avoid integer division
        else _incrementor = (float)ScoreTracker.ScoreLevel2 / (float)_totalTrashCount;

        _incrementor = Mathf.Clamp01(_incrementor); //clamp

        //avoid updating it everything, just whenever it needs to be changed (might still be quite performance heavy)
        if(_incrementor > _prevIncrementor) {
            ApplyWaterCondition();
            _prevIncrementor = _incrementor;
        }
        
    }

    private void ApplyWaterCondition() {
        InterpolatePP();
        InterpolateFogColor();
        InterpolateSkyboyTint();
    }

    private void InterpolatePP() {
        ColorGradingModel.Settings currentColorGrading = currentWaterPP.colorGrading.settings;

        //Tonemapping
        currentColorGrading.tonemapping.neutralWhiteLevel = Mathf.Lerp(_dirtyColorGrading.tonemapping.neutralWhiteLevel, _cleanColorGrading.tonemapping.neutralWhiteLevel, _incrementor);

        //Basic
        currentColorGrading.basic.postExposure = Mathf.Lerp(_dirtyColorGrading.basic.postExposure, _cleanColorGrading.basic.postExposure, _incrementor);
        currentColorGrading.basic.temperature = Mathf.Lerp(_dirtyColorGrading.basic.temperature, _cleanColorGrading.basic.temperature, _incrementor);
        currentColorGrading.basic.tint = Mathf.Lerp(_dirtyColorGrading.basic.tint, _cleanColorGrading.basic.tint, _incrementor);
        currentColorGrading.basic.saturation = Mathf.Lerp(_dirtyColorGrading.basic.saturation, _cleanColorGrading.basic.saturation, _incrementor);
        currentColorGrading.basic.contrast = Mathf.Lerp(_dirtyColorGrading.basic.contrast, _cleanColorGrading.basic.contrast, _incrementor);

        //Color Wheels (linear)
        currentColorGrading.colorWheels.linear.lift = Color.Lerp(_dirtyColorGrading.colorWheels.linear.lift, _cleanColorGrading.colorWheels.linear.lift, _incrementor);
        currentColorGrading.colorWheels.linear.gamma = Color.Lerp(_dirtyColorGrading.colorWheels.linear.gamma, _cleanColorGrading.colorWheels.linear.gamma, _incrementor);
        currentColorGrading.colorWheels.linear.gain = Color.Lerp(_dirtyColorGrading.colorWheels.linear.gain, _cleanColorGrading.colorWheels.linear.gain, _incrementor);

        //Color Wheels (log)
        currentColorGrading.colorWheels.log.slope = Color.Lerp(_dirtyColorGrading.colorWheels.log.slope, _cleanColorGrading.colorWheels.log.slope, _incrementor);
        currentColorGrading.colorWheels.log.power = Color.Lerp(_dirtyColorGrading.colorWheels.log.power, _cleanColorGrading.colorWheels.log.power, _incrementor);
        currentColorGrading.colorWheels.log.offset = Color.Lerp(_dirtyColorGrading.colorWheels.log.offset, _cleanColorGrading.colorWheels.log.offset, _incrementor);

        currentWaterPP.colorGrading.settings = currentColorGrading; //set new color grading settings to the PP on the camera
    }

    private void InterpolateFogColor() {
        _currentFogColor = Color.Lerp(dirtyWaterFogColor, cleanWaterFogColor, _incrementor); //interpolate color based on collected trash

        RenderSettings.fogColor = _currentFogColor; //set fog color
    }

    private void InterpolateSkyboyTint() {
        _currentSkyboxTint = Color.Lerp(dirtyWaterSkyboxTint, cleanWaterSkyboxTint, _incrementor); //interpolate color based on collected trash

        RenderSettings.skybox.SetColor("_Tint", _currentSkyboxTint); //set the tint for the skybox
        DynamicGI.UpdateEnvironment(); //apply settings to the skybox
    }
}

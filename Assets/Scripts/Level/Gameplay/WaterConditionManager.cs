using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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

    [Header("Kelp and Plants")]
    public Color dirtyKelpColor;
    public Color dirtyKelp2Color;
    public Color dirtyGroundFoliageColor;

    public Transform kelpHolder;
    public Transform plantHolder;

    [Header("Dust Particles")]
    public ParticleSystem[] dustParticles;

    [Header("Disabled Fish Tankes")]
    public FlockingGlobal[] fishTankes;

    private const string KELPMAT = "Kelp_Material (Instance)";
    private const string KELP2MAT = "Kelp2_Material (Instance)";
    private const string GROUNDFOLIAGEMAT = "GroundFoliage_Material (Instance)";

    private ColorGradingModel.Settings _dirtyColorGrading;
    private ColorGradingModel.Settings _cleanColorGrading;
    private Color _cleanFoliageColor;

    private List<Renderer> _kelpRenderers;
    private List<Renderer> _kelp2Renderers;
    private List<Renderer> _groundFoliageRenderers;

    private int _totalTrashCount;
    private float _prevIncrementor;
    private float _incrementor;

    public void Awake() {
        Camera.main.GetComponent<PostProcessingBehaviour>().profile = currentWaterPP; //apply PP to the camera

        Physics.gravity = Physics.gravity * gravityMultiplier; //adjust game physics

        _dirtyColorGrading = dirtyWaterPP.colorGrading.settings;
        _cleanColorGrading = cleanWaterPP.colorGrading.settings;

        _cleanFoliageColor = new Color(1, 1, 1, 1); //white

        InitRendererLists();

        _totalTrashCount = trashHolder.GetComponentsInChildren<BoxCollider>().Length; //count trash might be slightly inaccurate

        _prevIncrementor = 0f;
        _incrementor = 0f;

        ApplyWaterCondition();
    }

    public void Update() {
        //update incrementor when sucking trash
        _incrementor = (float)ScoreTracker.Score / (float)_totalTrashCount; //cast to float to avoid integer division
        _incrementor = Mathf.Clamp01(_incrementor); //clamp

        //avoid updating it every frame, just whenever it needs to be changed (might still be quite performance heavy)
        if (_incrementor > _prevIncrementor) {
            ApplyWaterCondition();
            _prevIncrementor = _incrementor;
        }
    }

    private void InitRendererLists() {
        //create and fill lists with correct renderers
        _kelpRenderers = new List<Renderer>();
        _kelp2Renderers = new List<Renderer>();
        _groundFoliageRenderers = new List<Renderer>();

        Renderer[] kelps = kelpHolder.GetComponentsInChildren<Renderer>();
        Renderer[] groundFoliage = plantHolder.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < kelps.Length; i++) {
            if (kelps[i].material.name == KELPMAT) _kelpRenderers.Add(kelps[i]);
            else if (kelps[i].material.name == KELP2MAT) _kelp2Renderers.Add(kelps[i]);
        }

        for (int i = 0; i < groundFoliage.Length; i++) {
            if (groundFoliage[i].material.name == GROUNDFOLIAGEMAT) _groundFoliageRenderers.Add(groundFoliage[i]);
        }
    }

    private void ApplyWaterCondition() {
        InterpolatePP();
        InterpolateFogColor();
        InterpolateSkyboyTint();
        InterpolateFoliage();
        InterpolateDustParticles();
        InterpolateFish();
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
        Color currentFogColor = Color.Lerp(dirtyWaterFogColor, cleanWaterFogColor, _incrementor); //interpolate color based on collected trash

        RenderSettings.fogColor = currentFogColor; //set fog color
    }

    private void InterpolateSkyboyTint() {
        Color currentSkyboxTint = Color.Lerp(dirtyWaterSkyboxTint, cleanWaterSkyboxTint, _incrementor); //interpolate color based on collected trash

        RenderSettings.skybox.SetColor("_Tint", currentSkyboxTint); //set the tint for the skybox
        DynamicGI.UpdateEnvironment(); //apply settings to the skybox and global illumination
    }

    private void InterpolateFoliage() {
        //interpolate the material colors of kelps and plants
        for (int i = 0; i < _kelpRenderers.Count; i++) {
            _kelpRenderers[i].material.SetColor("_Color", Color.Lerp(dirtyKelpColor, _cleanFoliageColor, _incrementor));
        }

        for (int i = 0; i < _kelp2Renderers.Count; i++) {
            _kelp2Renderers[i].material.SetColor("_Color", Color.Lerp(dirtyKelp2Color, _cleanFoliageColor, _incrementor));
        }

        for (int i = 0; i < _groundFoliageRenderers.Count; i++) {
            _groundFoliageRenderers[i].material.SetColor("_Color", Color.Lerp(dirtyGroundFoliageColor, _cleanFoliageColor, _incrementor));
        }
    }

    private void InterpolateDustParticles() {
        for (int i = 0; i < dustParticles.Length; i++) {
            var main = dustParticles[i].main;
            main.maxParticles = (int)Mathf.Lerp(dustParticles[i].main.maxParticles, 0, _incrementor); //needs to return as an interger value
        }
    }

    private void InterpolateFish() {
        //These are the strangest for loops I have ever written
        if (_incrementor >= 0.25f) {
            for (int i = 0; i < Mathf.RoundToInt(fishTankes.Length * 0.25f); i++) {
                fishTankes[i].enabled = true;
            }
        }

        if (_incrementor >= 0.5f) {
            for (int i = Mathf.RoundToInt(fishTankes.Length * 0.25f); i < fishTankes.Length * 0.5f; i++) {
                fishTankes[i].enabled = true;
            }
        }

        if (_incrementor >= 0.75f) {
            for (int i = Mathf.RoundToInt(fishTankes.Length * 0.5f); i < fishTankes.Length * 0.75f; i++) {
                fishTankes[i].enabled = true;
            }
        }

        if (_incrementor >= 1) {
            for (int i = Mathf.RoundToInt(fishTankes.Length * 0.75f); i < fishTankes.Length; i++) {
                fishTankes[i].enabled = true;
            }
        }
    }
}

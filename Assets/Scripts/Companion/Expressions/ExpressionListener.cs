using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class ExpressionListener : MonoBehaviour {
    [SerializeField]
    private Material _faceMaterial;
    [SerializeField]
    public List<Expression> _expressions;

    [SerializeField]
    private int _bufferSize = 1024;
    [SerializeField]
    private float _strength;

    public float minSpeed;
    public float maxSpeed;

    private CompanionAudio _companionAudio;
    private float _timer;

    [Serializable]
    public class Expression {
        public string _expressionName;
        public Expressions _expression;
    }

    private void Awake() {
        _companionAudio = transform.parent.GetComponent<CompanionAudio>();
    }

    public void ChangeExpression(string expressionName) {
        Expression newExpression = GetExpressionData(expressionName);
        if (newExpression != null)
            _faceMaterial.SetFloat("_Expression", (int)newExpression._expression);
        else
            Debug.Log(expressionName + " doesn't seem to exist in the Expression Listener");
    }

    private Expression GetExpressionData(string expressionName) {
        for (int i = 0; i < _expressions.Count; i++) {
            if (expressionName == _expressions[i]._expressionName)
                return _expressions[i];
        }
        return null;
    }

    private float GetSinWave(float speed) {
        _timer += Time.deltaTime * speed;
        return Mathf.Clamp(Mathf.Sin(_timer), 0.1f, 1f);
    }

    private void SetEmissionStrength() {
        if (_companionAudio.GetStartedPlaying() && _companionAudio.GetPlaybackState(AudioSourceType.Voice) != FMOD.Studio.PLAYBACK_STATE.STOPPED) {
            float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            _faceMaterial.SetFloat("_EmissionStrenght", GetSinWave(speed) * _strength);
        } else {
            _faceMaterial.SetFloat("_EmissionStrenght", 0.5f * _strength);
        }
    }

    void Update() {
        //Probably should set this to a different channel, currently using the master channel group
        SetEmissionStrength();
    }

    float lin2dB(float linear) {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }
}

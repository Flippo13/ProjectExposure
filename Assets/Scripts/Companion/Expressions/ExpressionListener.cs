using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class ExpressionListener : MonoBehaviour
{
    [SerializeField]
    private Material _faceMaterial;
    [SerializeField]
    public List<Expression> _expressions;

    [SerializeField]
    private int _bufferSize = 1024;
    [SerializeField]
    private int _strenght;

    FMOD.DSP fft;

    [Serializable]
    public class Expression
    {
        public string _expressionName;
        public Color _color;
        public Expressions _expression;
    }

    private void Awake()
    {
        // Create a DSP listener so we can detect peaks
        FMODUnity.RuntimeManager.LowlevelSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, _bufferSize);

        // Probably should set this to a different channel, currently using the master channel group
        FMOD.ChannelGroup channelGroup;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);
    }

    public void ChangeExpression(string expressionName)
    {
        Expression newExpression = GetExpressionData(expressionName);
        if (newExpression != null)
        {
            _faceMaterial.SetFloat("_Expression", (int)newExpression._expression);        
            _faceMaterial.SetColor("_Color", newExpression._color);
        }
        else
            Debug.Log(expressionName + " doesn't seem to exist in the Expression Listener");
    }

    private Expression GetExpressionData(string expressionName)
    {
        for (int i = 0; i < _expressions.Count; i++)
        {
            if (expressionName == _expressions[i]._expressionName)
                return _expressions[i];
        }
        return null;
    }

    void Update()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        var spectrum = fftData.spectrum;

        if (fftData.numchannels > 0)
        {
            for (int i = 0; i < 1; ++i)
            {
                _faceMaterial.SetFloat("_EmissionStrenght", _strenght / Mathf.Abs(lin2dB(spectrum[0][i])));
            }
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }
}

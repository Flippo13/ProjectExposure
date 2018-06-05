using UnityEngine;
public class Fog : MonoBehaviour
{
    [SerializeField]
    private Gradient _fogGradient;
    [SerializeField]
    private Material _fogMaterial;

    [SerializeField]
    [Range(0, 5)]
    private float _FogIntensity = 2;

    [SerializeField]
    [Range(0, 5)]
    private float _FogAmount = 2;

    [SerializeField]
    [Range(32, 256)]
    private int _textureSize = 128;

    void Awake()
    {
        GenerateTexture();
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    [ContextMenu("Update")]
    private void GenerateTexture()
    {
        //Create new texture with
        Texture2D texture = new Texture2D(_textureSize, 1);

        //Some texture settings to make it ideal for the shader
        texture.alphaIsTransparency = true;
        texture.anisoLevel = 16;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Trilinear;

        //Add gradient colors to the texture
        for (int i = 0; i < _textureSize; i++)
        {
            texture.SetPixel(i, 1, _fogGradient.Evaluate((float)i / _textureSize));
        }
        texture.Apply();

        //Apply generated texture and paremeters to shader
        _fogMaterial.SetTexture("_ColorRamp", texture);
        _fogMaterial.SetFloat("_FogIntensity", _FogIntensity);
        _fogMaterial.SetFloat("_FogAmount", _FogAmount);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _fogMaterial);
    }
}
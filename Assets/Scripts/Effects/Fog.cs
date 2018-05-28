using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Fog : MonoBehaviour
{

    public Material fogMaterial;
    public Camera main;

    void Start()
    {
        main.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, fogMaterial);
    }
}
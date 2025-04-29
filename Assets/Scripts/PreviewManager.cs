using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RawImage rawImage;
    
    private CustomRenderTexture renderTexture;
    
    private void Start()
    {
        renderTexture = new CustomRenderTexture(512, 512);
        cam.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
    }
}

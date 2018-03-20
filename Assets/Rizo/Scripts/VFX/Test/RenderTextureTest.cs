using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureTest : MonoBehaviour
{
    public VFXImageEffect _effect;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetTexture("_MainTex", _effect._outputImage);
    }
}

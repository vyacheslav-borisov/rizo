using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VFXImageEffect : MonoBehaviour
{
    [HideInInspector]
    public RenderTexture _outputImage;

    public Material _processingShader;
    public string _textureParamName;
    public string _alphaParamName;
    [Range(0, 1)]
    public float  _shaderAlpha = 1.0f;

    public enum Scope
    {
        INIT,
        UPDATE,
        RENDER
    }

    public    virtual void _SetShaderParams(Scope scope) { }

    protected virtual void _OnShaderError()
    {
        gameObject.SetActive(false);
    }

    protected virtual RenderTexture CreateOutputBuffer()
    {
        var output = new RenderTexture(Screen.width, Screen.height,
            0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        return output;
    }  

    private void Awake()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            Debug.LogError("image effects is not supported!");
            _OnShaderError();
            return;
        }

        if (_processingShader && !_processingShader.shader.isSupported)
        {
            Debug.LogError("shader effect " + _processingShader.shader.isSupported + "  is not supported!");
            _OnShaderError();
            return;
        }

        _outputImage = CreateOutputBuffer();

        var camera = GetComponent<Camera>();
        camera.targetTexture = _outputImage;

        _SetShaderParams(Scope.INIT);
    }    
}

using UnityEngine;

public class VFXOverlay : VFXImageEffect
{
    public VFXImageEffect[] _effects;
    private int[] _hashTexures;
    private int[] _hashAlphas;

    public override void _SetShaderParams(Scope scope)
    {
        if(scope == Scope.INIT)
        {
            _hashTexures = new int[_effects.Length];
            _hashAlphas = new int[_effects.Length];

            for(int i = 0; i < _effects.Length; i++)
            {
                _hashTexures[i] = Shader.PropertyToID(_effects[i]._textureParamName);
                _hashAlphas[i] = Shader.PropertyToID(_effects[i]._alphaParamName);
            }
        }
        
        if(scope == Scope.UPDATE)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _processingShader.SetFloat(_hashAlphas[i], _effects[i]._shaderAlpha);                
            }
        }
        
        if(scope == Scope.RENDER)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _processingShader.SetTexture(_hashTexures[i], _effects[i]._outputImage);
            }
        }       
    }

    protected override void _OnShaderError()
    {
        enabled = false;
    }

    protected override RenderTexture CreateOutputBuffer()
    {
        return null;
    }
}

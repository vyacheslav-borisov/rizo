using UnityEngine;

public class VFXColorKeyMask : VFXImageEffect
{
    public Texture _textureMask;
    [Range(0.1f, 100.0f)]
    public float    _scaleV = 1.0f;
    [Range(1.0f, 10.0f)]
    public float _factor = 1.0f;

    public override void _SetShaderParams(Scope scope)
    {
        if (scope == Scope.INIT)
        {
            _processingShader.SetTexture("_MaskTex", _textureMask);
        }

        if (scope == Scope.UPDATE)
        {
            _processingShader.SetFloat("_scaleV", _scaleV);
            _processingShader.SetFloat("_factor", _factor);
        }
    }
}

using UnityEngine;

public class VFXGlowEffect : VFXImageEffect
{
    [Range(0.01f, 1.0f)]
    public float _fraction = 0.2f;
    [Range(0.8f, 10.5f)]
    public float _factor = 10.0f;

    private int _paramID_MainTexture;
    private int _paramID_TexelStepU;
    private int _paramID_TexelStepV;
    private int _paramID_Factor;

    protected override RenderTexture CreateOutputBuffer()
    {
        int width = (int)(Screen.width * _fraction);
        int height = (int)(Screen.height * _fraction);

        var output = new RenderTexture(width, height, 0, 
            RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        
        return output;
    }

    public override void _SetShaderParams(Scope scope)
    {
        if(scope == Scope.INIT)
        {
            _paramID_MainTexture = Shader.PropertyToID("_MainTex");
            _paramID_TexelStepU = Shader.PropertyToID("_TexelUStep");
            _paramID_TexelStepV = Shader.PropertyToID("_TexelVStep");
            _paramID_Factor = Shader.PropertyToID("_Factor");

            int width = (int)(Screen.width * _fraction);
            int height = (int)(Screen.height * _fraction);
            float texelStepU = (1.0f / (1.0f * width));
            float texelStepV = (1.0f / (1.0f * height));

            _processingShader.SetFloat(_paramID_TexelStepU, texelStepU);
            _processingShader.SetFloat(_paramID_TexelStepV, texelStepV);
        }

        if(scope == Scope.UPDATE)
        {
            _processingShader.SetFloat(_paramID_Factor, _factor);
        }
    }
}

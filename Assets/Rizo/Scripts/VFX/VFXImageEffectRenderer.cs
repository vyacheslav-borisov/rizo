using UnityEngine;

[RequireComponent(typeof(VFXImageEffect))]
public class VFXImageEffectRenderer : MonoBehaviour
{
    public VFXImageEffect _effect;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _effect._SetShaderParams(VFXImageEffect.Scope.RENDER);
        Graphics.Blit(source, destination, _effect._processingShader);
    }
}

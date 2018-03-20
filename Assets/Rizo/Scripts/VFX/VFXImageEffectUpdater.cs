using UnityEngine;

[RequireComponent(typeof(VFXImageEffect))]
public class VFXImageEffectUpdater : MonoBehaviour
{
    public VFXImageEffect _effect;
		
	void Update ()
    {
        _effect._SetShaderParams(VFXImageEffect.Scope.UPDATE);    	
	}
}

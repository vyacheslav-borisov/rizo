using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSEffectScaller: MonoBehaviour
{
    [Range(0, 1)]
    public float Scale = 1.0f;
    public bool ParentScale = true;
    public bool ChildsScale = true;
    public bool ParticleSizes = true;
    public bool ParticleSpeeds = true;   

    private Transform[] _components;

    private ParticleSystem[] particles;
    private float[] originalSizes;
    private float[] originalSpeeds;
    private float[] originalRates;
    private float[] originalRadiuses;



    private void UpdateEffect()
    {
        if (ChildsScale)
        {
            foreach (var component in _components)
            {
                transform.localScale = Vector3.one * Scale;
            }
        }

        for (int i = 0; i < particles.Length; i++)
        {
            var main = particles[i].main;
            var emission = particles[i].emission;
            var shape = particles[i].shape;

            if (ParticleSizes)
            {
                main.startSizeMultiplier = originalSizes[i] * Scale;
            }

            if (ParticleSpeeds)
            {
                main.startSpeedMultiplier = originalSpeeds[i] * Scale;
            }
        }
    }

    void Start ()
    {
        _components = GetComponentsInChildren<Transform>();

        particles = GetComponentsInChildren<ParticleSystem>();
        originalSizes = new float[particles.Length];
        originalSpeeds = new float[particles.Length];
       
        for (int i = 0; i < particles.Length; i++)
        {
            originalSizes[i] = particles[i].main.startSizeMultiplier;
            originalSpeeds[i] = particles[i].main.startSpeedMultiplier;            
        }

        UpdateEffect();		
	}
	
	void Update ()
    {
        UpdateEffect();	
	}
}

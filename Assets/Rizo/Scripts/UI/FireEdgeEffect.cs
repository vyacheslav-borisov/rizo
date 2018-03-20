using UnityEngine;
using System.Collections;

public class FireEdgeEffect : MonoBehaviour
{
    [Range(0, 1)]
    public float Radius = 1.0f;
    [Range(0, 1)]
    public float Scale = 0.2f;
    public float LifeTime = 3.0f;
    
    private ParticleSystem[] particles;
    private float[] originalSizes;
    private float[] originalSpeeds;
    private float[] originalRates;
    private float[] originalRadiuses;

    void Start ()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        originalSizes = new float[particles.Length];
        originalSpeeds = new float[particles.Length];
        originalRates = new float[particles.Length];
        originalRadiuses = new float[particles.Length];

        for (int i =0; i < particles.Length; i++)
        {
            originalSizes[i] = particles[i].main.startSizeMultiplier;
            originalSpeeds[i] = particles[i].main.startSpeedMultiplier;
            originalRates[i] = particles[i].emission.rateOverTimeMultiplier;
            originalRadiuses[i] = particles[i].shape.radius;
        }

        UpdateEffect();

        StartCoroutine(Coroutine_Attenuate());        		
    }

    IEnumerator Coroutine_Attenuate()
    {
        yield return new WaitForSeconds(LifeTime);

        float time = 0;
        float initialScale = Scale;

        while (time < 1.0f)
        {
            time += Time.deltaTime;
            Scale = Mathf.Lerp(initialScale, 0.0f, time);

            UpdateEffect();

            yield return null;
        }

        Destroy(gameObject);
    }

    private void UpdateEffect()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            var main = particles[i].main;
            var emission = particles[i].emission;
            var shape = particles[i].shape;

            main.startSizeMultiplier = originalSizes[i] * Scale;
            main.startSpeedMultiplier = originalSpeeds[i] * Scale;

            shape.radius = originalRadiuses[i] * Radius;
            emission.rateOverTimeMultiplier = originalRates[i] * Radius;
        }
    }

    /*private void Update()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            var main = particles[i].main;
            var emission = particles[i].emission;
            var shape = particles[i].shape;

            main.startSizeMultiplier = originalSizes[i] * Scale;
            main.startSpeedMultiplier = originalSpeeds[i] * Scale;

            shape.radius = originalRadiuses[i] * Radius;
            emission.rateOverTimeMultiplier = originalRates[i] * Radius;
        }
    }*/
}

using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MovementNoise : MonoBehaviour
{
    [Serializable]
    public struct Noise
    {
        public Noise(bool sod, bool fip, bool fit, float weight, float scale, float cutoff)
        {
            scaleOnDelta = sod;
            factorInPosition = fip;
            factorInTime = fit;
            this.weight = weight;
            this.scale = scale;
            this.cutoff = cutoff;
        }
        public bool scaleOnDelta;
        public bool factorInPosition;
        public bool factorInTime;
        public float weight;
        public float scale;
        public float cutoff;
    }


    // Settings
    public Noise noise = new(
        false, false, true,
        1f, 0.05f, 0.2f
    );
    public float noiseAmount = 0f;
    public Color noiseColor = Color.black;


    // Apply Noise
    public void ApplyNoise(ref Vector3 vector, float perlinOffset=0)
    {
        Vector3 perlinWeight = ((noise.factorInTime ? Time.time : 1) + perlinOffset) * noise.scale * (noise.factorInPosition ? transform.position : Vector3.one);
        Assert.IsFalse(float.IsNaN(perlinWeight.x) || float.IsNaN(perlinWeight.y) || float.IsNaN(perlinWeight.z));
        float rotationNoise = Mathf.PerlinNoise(perlinWeight.x, perlinWeight.z);
        float speedNoise = Mathf.PerlinNoise1D(rotationNoise);
        Vector3 vectorNoise = Quaternion.AngleAxis(rotationNoise * 360, Vector3.up) * Vector3.forward * speedNoise;
        if (Mathf.Abs(vectorNoise.magnitude) > noise.cutoff)
        {
            Debug.DrawLine(transform.position, transform.position + vectorNoise* noise.weight, noiseColor, Time.fixedDeltaTime);
            noiseAmount = vectorNoise.magnitude;
            vector += (noise.scaleOnDelta ? Time.fixedDeltaTime : 1) * noise.weight * vectorNoise;
            vector /= 1 + noise.weight;
        }
    }
}

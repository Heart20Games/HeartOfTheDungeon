using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystems : BaseMonoBehaviour
{
    [SerializeField] private bool loadChildren = true;
    [ConditionalField("loadChildren", inverse: true)]
    [SerializeField] private List<ParticleSystem> particles = new();
    private readonly List<ParticleSystem> children = new();
    private bool prevLoadChildren = true;

    private void Awake()
    {
        UpdateLoadChildren();
    }

    private void OnValidate()
    {
        UpdateLoadChildren();
    }

    private void UpdateLoadChildren()
    {
        if (loadChildren != prevLoadChildren)
        {
            if (!loadChildren)
                ClearChildren();
            else
                LoadChildren();

            prevLoadChildren = loadChildren;
        }
    }

    [ButtonMethod]
    private void LoadChildren()
    {
        ClearChildren();
        children.AddRange(gameObject.GetComponentsInChildren<ParticleSystem>());
        particles.AddRange(children);
    }

    [ButtonMethod]
    private void ClearChildren()
    {
        particles.RemoveAll((system) => { return children.Contains(system); });
        children.Clear();
    }

    [ButtonMethod]
    public void Play()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }

    [ButtonMethod]
    public void Stop()
    {
        foreach (var particle in particles)
        {
            particle.Stop();
        }
    }
}

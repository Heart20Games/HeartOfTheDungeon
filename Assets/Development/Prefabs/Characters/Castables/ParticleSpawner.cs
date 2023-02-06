using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSpawner : MonoBehaviour, IPositionable
{
    public ParticleSystem system;
    public Transform source;
    public Transform target;

    private readonly List<ParticleSystem> instances = new List<ParticleSystem>();
    public UnityEvent onFinished;
    
    private void Start()
    {
        if (source == null)
        {
            source = this.transform;
        }
    }

    private void Update()
    {
        int running = instances.Count;
        for (int i = 0; i < instances.Count; i++)
        {
            ParticleSystem instance = instances[i];
            if (instance != null && !instance.IsAlive())
            {
                Destroy(instance.gameObject);
                onFinished.Invoke();
            }
            else if (instance == null)
            {
                running -= 1;
            }
        }
        if (running == 0)
        {
            instances.Clear();
        }
    }

    public void SetOrigin(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
    }

    public void Spawn()
    {
        ParticleSystem instance = Instantiate(system, source);
        instance.transform.position = target.position + system.transform.position;
        instance.Play();
        instances.Add(instance);
    }
}

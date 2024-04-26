using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class EffectSpawner : Positionable
{
    [SerializeField] private StudioEventEmitter deathPoof;
    [Serializable]
    public struct Effect
    {
        public Effect(float lifeTime, Transform prefab)
        {
            this.lifeTime = lifeTime;
            this.prefab = prefab;
        }
        public float lifeTime;
        public Transform prefab;
    }

    public struct Instance
    {
        public Instance(Effect effect, Transform transform)
        {
            this.effect = effect;
            this.transform = transform;
        }
        public Effect effect;
        public Transform transform;
    }

    public List<Effect> effects;

    private void Start()
    {
        if (source == null)
        {
            source = transform;
        }
    }

    public void Spawn()
    {
        foreach (Effect effect in effects)
        {
            deathPoof.Play();
            Transform tform = Instantiate(effect.prefab, source);
            tform.position = target.position + effect.prefab.transform.position;
            StartCoroutine(CleanupInstance(new Instance(effect, tform)));
        }
    }

    public IEnumerator CleanupInstance(Instance instance)
    {
        yield return new WaitForSeconds(instance.effect.lifeTime);
        Destroy(instance.transform.gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;
using MyBox;

public class EffectSpawner : Positionable
{
    [FormerlySerializedAs("deathPoof")]
    [SerializeField] private StudioEventEmitter soundEmitter;
    [SerializeField] private bool autoSpawn = false;
    [ConditionalField("autoSpawn")]
    [SerializeField] private float spawnRate;
    [ConditionalField("autoSpawn")]
    public List<Target> spawnTargets;
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

    [Serializable]
    public struct Target
    {
        public Target(Transform parent, Vector3 offset)
        {
            this.parent = parent;
            this.offset = offset;
        }
        public Transform parent;
        public Vector3 offset;
    }

    public List<Effect> effects;

    private void Start()
    {
        if (source == null)
        {
            source = transform;
        }
    }

    private void Update()
    {
        if (autoSpawn)
        {
            autoSpawnRoutine ??= StartCoroutine(AutoSpawn());
        }
        else if (autoSpawnRoutine != null)
        {
            StopCoroutine(autoSpawnRoutine);
            autoSpawnRoutine = null;
        }
    }

    private void OnDestroy()
    {
        if (autoSpawnRoutine != null)
        {
            StopCoroutine(autoSpawnRoutine);
            autoSpawnRoutine = null;
        }
    }

    private Coroutine autoSpawnRoutine = null;
    private int lastSpawnTime = 0;
    private IEnumerator AutoSpawn()
    {
        while (true)
        {
            Print($"Auto Spawning {spawnTargets.Count} times.", true, this);
            foreach (var target in spawnTargets)
            {
                Spawn(target.offset, target.parent);
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void Spawn(Vector3 position, Transform parent=null)
    {
        parent = (parent != null ? parent : (source == null ? transform : source));
        foreach (Effect effect in effects)
        {
            if (soundEmitter != null)
                soundEmitter.Play();
            Transform tform = Instantiate(effect.prefab, parent);
            tform.position = position + effect.prefab.transform.position;
            StartCoroutine(CleanupInstance(new Instance(effect, tform)));
        }
    }

    public void Spawn()
    {
        Spawn(target.position);
    }

    public IEnumerator CleanupInstance(Instance instance)
    {
        yield return new WaitForSeconds(instance.effect.lifeTime);
        Destroy(instance.transform.gameObject);
    }
}

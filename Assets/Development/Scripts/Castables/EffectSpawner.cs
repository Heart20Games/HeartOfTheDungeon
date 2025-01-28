using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;
using MyBox;
using UnityEngine.PlayerLoop;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class EffectSpawner : Positionable
{
    [FormerlySerializedAs("deathPoof")]
    [SerializeField] private StudioEventEmitter soundEmitter;
    [SerializeField] private bool autoSpawn = false;
    [ConditionalField("autoSpawn")]
    [SerializeField] private float spawnRate;
    [ConditionalField("autoSpawn")]
    public List<Target> spawnTargets = new();
    public List<Effect> effects = new();
    private List<Instance> instances = new();

    // Structs
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
        public Target(Transform parent, List<Vector3> offsets, bool useWorldSpace=false)
        {
            this.parent = parent;
            this.useWorldSpace = useWorldSpace;
            this.mode = Mode.Raw;
            this.offset = new();
            this.radius = 1f;
            this.density = 8f;
            this.offsets = offsets;
        }
        public Target(Transform parent, Vector3 offset, bool useWorldSpace=false)
        {
            this.parent = parent;
            this.useWorldSpace = useWorldSpace;
            this.mode = Mode.Single;
            this.offset = offset;
            this.radius = 1f;
            this.density = 8f;
            this.offsets = new();
            CalculateOffsets();
        }
        public Target(Transform parent, Vector3 offset, float radius, float density, bool useWorldSpace=false)
        {
            this.parent = parent;
            this.useWorldSpace = useWorldSpace;
            this.mode = Mode.Ring;
            this.offset = offset;
            this.radius = radius;
            this.density = density;
            this.offsets = new();
            CalculateOffsets();
        }

        public enum Mode { Single, Raw, Ring }
        public Transform parent;
        public bool useWorldSpace;
        public Mode mode;
        public Vector3 offset;
        [ConditionalField(true, "ShowRadius")]
        public float radius;
        [ConditionalField(true, "ShowDensity")]
        public float density;
        public List<Vector3> offsets;

        private bool ShowRadius() { return mode == Mode.Ring; }
        private bool ShowDensity() { return mode == Mode.Ring; }

        public readonly void CalculateOffsets()
        {
            switch (mode)
            {
                case Mode.Raw: break;
                default: offsets.Clear(); break;
            }
            offsets.Clear();

            switch (mode)
            {
                case Mode.Raw: break;
                case Mode.Single: Offset(offset, offsets);  break;
                case Mode.Ring: Ring(offset, radius, density, offsets); break;
                default: Offset(offset, offsets); break;
            }

            void Offset(Vector3 offset, List<Vector3> offsets)
            {
                offsets.Add(offset);
            }

            void Ring(Vector3 offset, float radius, float density, List<Vector3> offsets)
            {
                for (int i = 1; i <= density; i++)
                {
                    float arcPos = Mathf.Lerp(0, 2 * Mathf.PI, i / density);
                    Vector3 point = new(Mathf.Cos(arcPos), Mathf.Sin(arcPos), 0);
                    offsets.Add((point * radius) + offset);
                }
            }
        }
    }

    // Stuff

    private void Start()
    {
        if (source == null)
        {
            source = transform;
        }
    }

    private void OnDisable()
    {
        ClearInstances();
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

    private void OnValidate()
    {
        foreach (var target in spawnTargets)
        {
            target.CalculateOffsets();
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
                foreach (var offset in target.offsets)
                {
                    Spawn(offset, target.parent, target.useWorldSpace);
                }
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void Spawn(Vector3 position, Transform parent=null, bool useWorldSpace=false)
    {
        parent = (parent != null ? parent : (source == null ? transform : source));
        foreach (Effect effect in effects)
        {
            if (soundEmitter != null)
                soundEmitter.Play();
            Transform tform = Instantiate(effect.prefab, parent);
            Vector3 final = position + effect.prefab.transform.position;
            if (useWorldSpace)
                tform.position = final;
            else
                tform.localPosition = final;
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

    public void ClearInstances()
    {
        foreach (var instance in instances)
        {
            Destroy(instance.transform.gameObject);
        }
        instances.Clear();
    }
}

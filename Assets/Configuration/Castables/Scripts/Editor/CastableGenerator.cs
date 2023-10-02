using Attributes;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Assertions;

namespace HotD.Castables
{
    using static Body.Behavior.ContextSteering.CSContext;
    using static HotD.Castables.CastableToLocation;
    using static Loadout;
    using Range = Body.Behavior.ContextSteering.CSContext.Range;

    [CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/Castable Generator", order = 1)]
    public class CastableGenerator : ScriptableObject
    {
        public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
        public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

        [Header("Parameters")]
        public string outputName = "New Castable";
        public CastableSettings settings = new();
        [Space]
        public CastableStats stats;
        public List<Effect> effects;

        [Header("Targeting")]
        public TargetingMethod targetingMethod;

        // EXECUTION
        public List<Execution> executions;

        [Header("Results")]
        public string castablesDirectory = "Assets/Configuration/Castables/";
        public bool createSubFolder = true;
        public bool overwrite = true;
        public bool replace = true;
        public List<Outputs> outputs;
        [ReadOnly] public string fullDirectory = "";
        [ReadOnly] public GameObject prefab;
        [ReadOnly] public List<CastableItem> items = new();
        [ReadOnly] public float timeOfLastGeneration;

        [Serializable]
        public struct Outputs
        {
            public string alias;
            public List<Target> targets;
        }
        [Serializable]
        public struct Target
        {
            public Slot slot;
            public Loadout loadout;
        }

        // Generate All Castables
        static public List<CastableGenerator> generators = new();
        [MenuItem("Tools / Generate Castables")]
        static public void GenerateAllCastables()
        {
            Debug.Log($"Generating {generators.Count} Castables");
            foreach (var generator in generators)
            {
                generator.GenerateCastable();
            }
        }

        // Generate Castable
        public void GenerateCastable()
        {
            if (!Application.isEditor)
            {
                Debug.LogWarning("Cannot generate castable outside the Editor");
            }
            else
            {
                if (!generators.Contains(this))
                    generators.Add(this);
                string oldDirectory = fullDirectory;
                PrepareResultDirectory();
                bool sameDirectory = oldDirectory.Equals(fullDirectory);

                if (prefab == null || overwrite || !sameDirectory)
                {
                    if (replace) // && !sameDirectory)
                    {
                        if (prefab != null)
                        {
                            AssetDatabase.DeleteAsset($"{oldDirectory}/{prefab.name}.prefab");
                            prefab = null;
                        }
                        if (items.Count > 0)
                        {
                            foreach (var oldItem in items)
                            {
                                AssetDatabase.DeleteAsset($"{oldDirectory}/{oldItem.name}.asset");
                            }
                            items.Clear();
                        }
                    }

                    // Trim the name so it doesn't look like a series of subdirectories
                    outputName = outputName.Trim('/');

                    // Objects
                    GameObject gameObject = new(outputName);
                    Pivot pivot = GeneratePivot(gameObject);

                    // Components
                    Castable castable = GenerateCastableBase(gameObject, pivot);
                    Damager damager = GenerateDamager(castable, gameObject);
                    Charger charger = GenerateCharger(castable, gameObject);
                    Timer coolDownTimer = GenerateCooldownTimer(castable, gameObject);

                    // Effects
                    foreach (var effect in effects)
                    {
                        effect.GenerateEffect(castable, stats);
                    }

                    // Fields
                    Context context = new(stats.targetIdentity, Range.InAttackRange, new(), new(), new(0, stats.Range), 50);
                    castable.castStatuses = stats.castStatuses;
                    castable.hitStatuses = stats.hitStatuses;

                    // Targeting Methods
                    switch (targetingMethod)
                    {
                        case TargetingMethod.TargetBased: break;
                        case TargetingMethod.LocationBased: break;
                        case TargetingMethod.DirectionBased: break;
                    }

                    // Execution Methods
                    foreach (Execution execution in executions)
                    {
                        execution.PrepareExecutionMethod(castable, stats, pivot, gameObject, damager);
                    }

                    // Save to Prefab
                    prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, $"{fullDirectory}/{outputName}.prefab");
                    DestroyImmediate(gameObject);
                    timeOfLastGeneration = Time.time;

                    // Set up Item
                    CastableItem item = GenerateItem(context);

                    // Set up Aliases
                    foreach (var output in outputs)
                    {
                        CastableItem copy = item;
                        if (!output.alias.Equals(outputName))
                        {
                            copy = Instantiate(item);
                            AssetDatabase.CreateAsset(copy, $"{fullDirectory}/{output.alias}.asset");
                            EditorUtility.SetDirty(copy);
                            items.Add(copy);
                        }
                        foreach (var target in output.targets)
                        {
                            if (target.slot != Slot.None)
                            {
                                target.loadout.SetSlot(target.slot, copy);
                                EditorUtility.SetDirty(target.loadout);
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(this);
            }
        }


        // Helpers

        private Pivot GeneratePivot(GameObject gameObject)
        {
            GameObject pivotObject = new("Pivot");
            Pivot pivot = pivotObject.AddComponent<Pivot>();
            pivot.transform.SetParent(gameObject.transform, false);
            return pivot;
        }

        private Castable GenerateCastableBase(GameObject gameObject, Pivot pivot)
        {
            Castable castable = gameObject.AddComponent<Castable>();
            castable.onSetPowerLevel ??= new();
            castable.onSetIdentity ??= new();
            castable.onCast ??= new();
            castable.onTrigger ??= new();
            castable.onRelease ??= new();
            castable.onUnCast ??= new();
            castable.onCasted ??= new();
            settings.ApplyToCastable(castable);
            castable.pivot = pivot.transform;
            return castable;
        }

        private Charger GenerateCharger(Castable castable, GameObject gameObject)
        {
            Assert.IsNotNull(castable);
            if (stats.useChargeUp)
            {
                Charger charger = gameObject.AddComponent<Charger>();
                charger.onBegin = new();
                charger.onCharge = new();
                charger.onCharged = new();
                charger.onInterrupt = new();
                charger.length = stats.ChargeRate;
                UnityEventTools.AddPersistentListener(castable.onTrigger, charger.Begin);
                UnityEventTools.AddPersistentListener(castable.onRelease, charger.Interrupt);
                UnityEventTools.AddPersistentListener(charger.onCharge, castable.SetPowerLevel);
                if (settings.castOnChargeUp)
                    UnityEventTools.AddPersistentListener(charger.onCharged, castable.Cast);
                return charger;
            }
            else return null;
        }

        private Timer GenerateCooldownTimer(Castable castable, GameObject gameObject)
        {
            Assert.IsNotNull(castable);
            if (stats.useCooldown)
            {
                Timer coolDownTimer = gameObject.AddComponent<Timer>();
                coolDownTimer.onComplete = new();
                coolDownTimer.length = stats.Cooldown; // TODO: Account for bonuses
                UnityEventTools.AddVoidPersistentListener(castable.onCast, coolDownTimer.Play);
                UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, castable.UnCast);
                return coolDownTimer;
            }
            else return null;
        }

        private Damager GenerateDamager(Castable castable, GameObject gameObject)
        {
            Assert.IsNotNull(castable);
            if (stats.dealDamage)
            {
                Damager damager = gameObject.AddComponent<Damager>();
                damager.damage = stats.Damage; // TODO: Account for bonuses
                UnityEventTools.AddPersistentListener(castable.onSetIdentity, damager.SetIdentity);
                return damager;
            }
            else return null;
        }

        private CastableItem GenerateItem(Context context)
        {
            CastableItem item = (CastableItem)CreateInstance(typeof(CastableItem));
            AssetDatabase.CreateAsset(item, $"{fullDirectory}/{outputName}.asset");
            EditorUtility.SetDirty(item);
            item.prefab = prefab.GetComponent<Castable>();
            item.stats = stats;
            item.context = context;
            items.Add(item);
            return item;
        }

        private static Casted AddCastedComponent(GameObject castedObject, Castable castable, CastableStats stats)
        {
            Casted casted = castedObject.AddComponent<Casted>();
            ConnectCastedComponent(casted, castable, stats);
            return casted;
        }

        private static void ConnectCastedComponent(Casted casted, Castable castable, CastableStats stats)
        {
            //casted.gameObject.SetActive(false);
            //casted.enabled = false;

            casted.stats = stats;

            casted.onSetPowerLimit ??= new();
            //UnityEventTools.AddPersistentListener(stats.chargeLimit.updatedFinal, casted.SetPowerLimit);
            
            casted.onTrigger ??= new();
            casted.onRelease ??= new();
            casted.onCast ??= new();
            casted.onUnCast ??= new();
            casted.onSetPowerLevel ??= new();
            UnityEventTools.AddPersistentListener(castable.onTrigger, casted.OnTrigger);
            UnityEventTools.AddPersistentListener(castable.onRelease, casted.OnRelease);
            UnityEventTools.AddPersistentListener(castable.onCast, casted.OnCast);
            UnityEventTools.AddPersistentListener(castable.onUnCast, casted.OnUnCast);
            UnityEventTools.AddPersistentListener(castable.onSetPowerLevel, casted.SetPowerLevel);
        }


        // Structs

        [Serializable]
        public struct CastableSettings
        {
            public CastableSettings(bool followBody = true, bool castOnTrigger = true, bool castOnRelease = false, bool unCastOnRelease = false, bool castOnChargeUp = false)
            {
                this.followBody = followBody;
                this.castOnTrigger = castOnTrigger;
                this.castOnRelease = castOnRelease;
                this.unCastOnRelease = unCastOnRelease;
                this.castOnChargeUp = castOnChargeUp;
            }
            public bool followBody;
            public bool castOnTrigger;
            public bool castOnRelease;
            public bool unCastOnRelease;
            public bool castOnChargeUp;
            public readonly void ApplyToCastable(Castable castable)
            {
                castable.onTrigger ??= new();
                castable.onRelease ??= new();
                castable.onCast ??= new();
                castable.onUnCast ??= new();

                castable.onSetPowerLevel ??= new();
                castable.onSetMaxPowerLevel ??= new();

                castable.onSetIdentity ??= new();

                castable.followBody = followBody;
                castable.castOnTrigger = castOnTrigger;
                castable.castOnRelease = castOnRelease;
                castable.unCastOnRelease = unCastOnRelease;
            }
        }

        [Serializable]
        public struct Effect
        {
            public Effect(Location source = Location.Character, Location target = Location.FiringPoint, Vector2 chargeLevels = new(), Vector2 comboSteps = new())
            {
                this.name = $"{source} -> {target}";
                casted = null;
                this.source = source;
                this.target = target;

                this.chargeLevels = chargeLevels;
                this.comboSteps = comboSteps;
            }

            public string name;
            public Casted casted;
            public Location source;
            public Location target;

            public Vector2 chargeLevels;
            public Vector2 comboSteps;

            public readonly void GenerateEffect(Castable castable, CastableStats stats)
            {
                if (casted != null)
                {
                    Casted body = Instantiate(casted, castable.transform);
                    ConnectCastedComponent(body, castable, stats);

                    body.powerRange = chargeLevels;
                    body.comboRange = comboSteps;
                    body.applyOnSet = true;

                    castable.toLocations.Add(new(body, source, target));
                    castable.castingMethods.Add(body.gameObject);
                }
            }
        }

        [Serializable]
        public struct Execution
        {
            public Execution(ExecutionMethod method=ExecutionMethod.ColliderBased, Location source=Location.Character, Location target=Location.FiringPoint, Vector2 chargeLevels=new(), Vector2 comboSteps=new())
            {
                this.name = method.ToString();
                this.method = method;
                this.source = source;
                this.target = target;

                this.chargeLevels = chargeLevels;
                this.comboSteps = comboSteps;
                this.colliderPrefab = null;
                this.projectileLifeSpan = 1;
                this.projectilePrefab = null;
            }

            public string name;
            public ExecutionMethod method;
            public Location source;
            public Location target;

            public Vector2 chargeLevels;
            public Vector2 comboSteps;

            // Execution: Collider
            [ConditionalField("method", false, ExecutionMethod.ColliderBased)]
            public CastedCollider colliderPrefab;

            // Execution: Projectile
            [ConditionalField("method", false, ExecutionMethod.ProjectileBased)]
            public float projectileLifeSpan;
            [ConditionalField("method", false, ExecutionMethod.ProjectileBased)]
            public Projectile projectilePrefab;

            public readonly void PrepareExecutionMethod(Castable castable, CastableStats stats, Pivot pivot, GameObject gameObject, Damager damager = null)
            {
                switch (method)
                {
                    case ExecutionMethod.ColliderBased: PrepareCollisionMethod(castable, stats, pivot, damager); break;
                    case ExecutionMethod.ProjectileBased: PrepareProjectileMethod(castable, stats, pivot, gameObject, damager); break;
                    case ExecutionMethod.SelectionBased: break;
                }
            }

            public readonly void PrepareCollisionMethod(Castable castable, CastableStats stats, Pivot pivot, Damager damager = null)
            {
                pivot.enabled = false;
                if (colliderPrefab != null)
                {
                    CastedCollider collider = Instantiate(colliderPrefab, pivot.transform);
                    ConnectCastedComponent(collider, castable, stats);
                    collider.powerRange = chargeLevels;
                    collider.comboRange = comboSteps;

                    castable.toLocations.Add(new(collider, source, target));
                    castable.castingMethods.Add(collider.gameObject);
                    
                    if (damager != null)
                    {
                        collider.hitDamageable = new();
                        collider.leftDamageable = new();
                        UnityEventTools.AddPersistentListener(collider.hitDamageable, damager.HitDamageable);
                        UnityEventTools.AddPersistentListener(collider.leftDamageable, damager.LeftDamageable);
                    }
                }
            }

            public readonly void PrepareProjectileMethod(Castable castable, CastableStats stats, Pivot pivot, GameObject gameObject, Damager damager = null)
            {
                GameObject castedObject = new(name);
                castedObject.transform.parent = pivot.transform;
                castable.castingMethods.Add(castedObject);
                
                GameObject pivotObject = new($"{name} Pivot");
                pivotObject.transform.parent = castedObject.transform;
                Pivot castedPivot = pivotObject.AddComponent<Pivot>();

                Casted casted = AddCastedComponent(castedObject, castable, stats);
                casted.powerRange = chargeLevels;
                casted.comboRange = comboSteps;

                ProjectileSpawner spawner = castedObject.AddComponent<ProjectileSpawner>();
                UnityEventTools.AddPersistentListener(casted.onCast, spawner.Spawn);
                spawner.pivot = castedPivot.transform;
                spawner.lifeSpan = projectileLifeSpan;
                spawner.applyOnSet = false;
                castedPivot.enabled = false;
                castable.toLocations.Add(new(spawner, source, target));

                if (projectilePrefab != null)
                {
                    Projectile projectile = Instantiate(projectilePrefab, castedPivot.transform);
                    spawner.projectile = projectile;
                    castedPivot.body = projectile.transform;
                    projectile.transform.localPosition = new();
                    if (damager != null)
                    {
                        projectile.hitDamageable = new();
                        projectile.leftDamageable = new();
                        UnityEventTools.AddPersistentListener(projectile.hitDamageable, damager.HitDamageable);
                        UnityEventTools.AddPersistentListener(projectile.leftDamageable, damager.LeftDamageable);
                    }
                }
            }
        }


        // Results

        private void PrepareResultDirectory()
        {
            fullDirectory = castablesDirectory;

            // Adjust for adding a sub folder using the output name.
            if (createSubFolder)
            {
                // Fix the Directory components to make sure they're valid.
                if (!fullDirectory.EndsWith('/'))
                {
                    fullDirectory += "/";
                }
                fullDirectory += outputName;
            }

            // Loop through directories, creating them as necessary.
            string[] steps = (fullDirectory).Split('/', System.StringSplitOptions.RemoveEmptyEntries);
            string lastPath = steps[0];
            for (int i = 1; i < steps.Length; i++)
            {
                if (steps[i].Trim() != "")
                {
                    string path = string.Join('/', steps, 0, i + 1);
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        AssetDatabase.CreateFolder(lastPath, steps[i]);
                    }
                    lastPath = path;
                }
            }
        }
    }
}
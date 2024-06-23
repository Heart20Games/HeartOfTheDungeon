using MyBox;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Assertions;
using static global::Body.Behavior.ContextSteering.CSContext;
using static HotD.Castables.CastableToLocation;
using static HotD.Castables.DelegatedExecutor;
using static HotD.Castables.Loadout;
using Range = global::Body.Behavior.ContextSteering.CSContext.Range;
using static CastCoordinator;

namespace HotD.Generators
{
    using Castables;

    [CreateAssetMenu(fileName = "NewStateCastGenerator", menuName = "Loadouts/State Cast Generator", order = 1)]
    public class StateCastGenerator : Generator
    {
        public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

        [Header("Parameters")]
        public CastableSettings settings = new();
        [Space]
        public CastableStats stats;
        public float[] chargeTimes = new float[3];
        public List<Effect> effects;

        [Header("Targeting")]
        public TargetingMethod targetingMethod;
        public AimingMethod aimingMethod;

        // EXECUTION
        public List<Execution> executions;

        [Header("Results")]
        public List<Outputs> outputs;
        [ReadOnly] public GameObject prefab;
        [ReadOnly] public List<CastableItem> items = new();

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
        //static public List<StateCastGenerator> generators = new();
        //[MenuItem("Tools / Generate Castables")]
        //static public void GenerateAllCastables()
        //{
        //    Debug.Log($"Generating {generators.Count} Castables");
        //    foreach (var generator in generators)
        //    {
        //        generator.GenerateCastable();
        //    }
        //}

        // Generate Castable
        [ButtonMethod]
        public void GenerateCastable()
        {
            if (!Application.isEditor)
            {
                Debug.LogWarning("Cannot generate castable outside the Editor");
            }
            else
            {
                //if (!generators.Contains(this))
                //    generators.Add(this);
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

                    // Base
                    StateCastable castable = GenerateCastableBase(gameObject, pivot);
                    Damager damager = GenerateDamager(castable, gameObject);
                    castable.AddBaseTransitions();

                    // Activation
                    DelegatedExecutor activationExecutor = GenerateExecutor(castable, CastState.Activating, "Activation");
                    if (stats.useChargeUp)
                    {
                        castable.AddChargeTransitions();
                        Charger charger = GenerateCharger(activationExecutor);
                    }

                    // Execution
                    DelegatedExecutor executionExecutor = GenerateExecutor(castable, CastState.Executing, "Execution");
                    if (executions.Count > 0)
                    {
                        Comboer comboer = GenerateComboer(executionExecutor, stats, pivot, gameObject, damager);
                    }

                    // Cooldown
                    if (stats.useCooldown)
                    {
                        DelegatedExecutor cooldownExecutor = GenerateExecutor(castable, CastState.Cooldown, "Cooldown");
                        castable.AddCooldownTransitions();
                        Timer coolDownTimer = GenerateCooldownTimer(cooldownExecutor);
                    }

                    // Effects
                    foreach (var effect in effects)
                    {
                        effect.GenerateEffect(castable, stats);
                    }

                    // Fields
                    Context context = new(stats.targetIdentity, Range.InAttackRange, new(), new(), new(0, stats.Range), 50);
                    castable.fields.castStatuses = stats.castStatuses;
                    castable.fields.hitStatuses = stats.hitStatuses;

                    // Targeting Methods
                    switch (targetingMethod)
                    {
                        case TargetingMethod.TargetBased: break;
                        case TargetingMethod.LocationBased: break;
                        case TargetingMethod.DirectionBased: break;
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

        private StateCastable GenerateCastableBase(GameObject gameObject, Pivot pivot)
        {
            StateCastable castable = gameObject.AddComponent<StateCastable>();
            castable.onSetPowerLevel ??= new();
            castable.onSetIdentity ??= new();
            castable.onCast ??= new();
            castable.onTrigger ??= new();
            castable.onRelease ??= new();
            castable.onUnCast ??= new();
            castable.onCasted ??= new();
            settings.ApplyToCastable(castable);
            castable.fields.pivot = pivot.transform;
            return castable;
        }

        private DelegatedExecutor GenerateExecutor(StateCastable castable, CastState executorState, string executorName)
        {
            Assert.IsNotNull(castable);

            GameObject gameObject = new(executorName);
            DelegatedExecutor executor = gameObject.AddComponent<DelegatedExecutor>();
            executor.State = executorState;

            // Needs to be a child of the StateCastable
            gameObject.transform.SetParent(castable.gameObject.transform);

            return executor;
        }

        private Charger GenerateCharger(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);
            
            Charger charger = executor.gameObject.AddComponent<Charger>();
            charger.onBegin = new();
            charger.onCharge = new();
            charger.onChargeInt = new();
            charger.onCharged = new();
            charger.onInterrupt = new();
            charger.chargeTimes = chargeTimes;
            charger.chargeLimit = stats.chargeLimit;

            // Start Transition
            ActionEvent startTransition = new("Start", CastAction.Start, Triggers.StartAction, false);
            UnityEventTools.AddPersistentListener(startTransition.startAction, charger.Begin);
            executor.supportedActions.Add(startTransition);

            // Release Transition
            ActionEvent releaseTransition = new("Release", CastAction.Start, Triggers.None, true);
            executor.supportedActions.Add(releaseTransition);
            //UnityEventTools.AddPersistentListener(releaseTransition.startAction, charger.Interrupt);

            // Keep Power Level Updated
            UnityEventTools.AddPersistentListener(charger.onChargeInt, executor.SetPowerLevel);
                
            // Executor On Full Charge?
            if (settings.castOnChargeUp)
                UnityEventTools.AddPersistentListener(charger.onCharged, executor.End);
                
            return charger;
        }

        public Comboer GenerateComboer(DelegatedExecutor executor, CastableStats stats, Pivot pivot, GameObject gameObject, Damager damager = null)
        {
            Assert.IsNotNull(executor);

            Comboer comboer = executor.gameObject.AddComponent<Comboer>();

            GameObject methods = new("Methods");
            methods.transform.SetParent(comboer.transform);
            Pivot methodPivot = methods.AddComponent<Pivot>();
            methodPivot.body = pivot.body;

            for (int i = 0; i < executions.Count; i++)
            {
                Execution execution = executions[i];
                Casted casted = execution.PrepareExecutionMethod(executor, stats, methodPivot, gameObject, damager);
                comboer.AddStep(i + 1, casted.gameObject);
            }

            return comboer;
        }

        private Timer GenerateCooldownTimer(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            Timer coolDownTimer = executor.gameObject.AddComponent<Timer>();
            coolDownTimer.onComplete = new();
            coolDownTimer.length = stats.Cooldown; // TODO: Account for bonuses

            // Start Transition
            ActionEvent startTransition = new("Start", CastAction.Start, Triggers.None, false);
            UnityEventTools.AddVoidPersistentListener(startTransition.startAction, coolDownTimer.Play);
            executor.supportedActions.Add(startTransition);

            // Finish on Timer Complete
            UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, executor.End);

            return coolDownTimer;
        }

        private Damager GenerateDamager(CastableProperties castable, GameObject gameObject)
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
            item.prefab = prefab;
            item.targetingMethod = targetingMethod;
            item.aimingMethod = aimingMethod;
            item.stats = stats;
            item.context = context;
            items.Add(item);
            return item;
        }

        private static Casted AddCastedComponent(GameObject castedObject, CastableProperties castable, CastableStats stats)
        {
            Casted casted = castedObject.AddComponent<Casted>();
            ConnectCastedComponent(casted, castable, stats);
            return casted;
        }

        private static void ConnectCastedComponent(Casted casted, CastableProperties castable, CastableStats stats)
        {
            //casted.gameObject.SetActive(false);
            //casted.enabled = false;

            casted.stats = stats;
            casted.powerLimit = stats.chargeLimit;

            casted.onSetPowerLimit ??= new();
            //UnityEventTools.AddPersistentListener(stats.chargeLimit.updatedFinal, casted.OnSetPowerLimit);

            casted.onTrigger ??= new();
            casted.onRelease ??= new();
            casted.onCast ??= new();
            casted.onEndCast ??= new();
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
            public readonly void ApplyToCastable(CastableProperties castable)
            {
                castable.onTrigger ??= new();
                castable.onRelease ??= new();
                castable.onCast ??= new();
                castable.onUnCast ??= new();

                castable.onSetPowerLevel ??= new();
                castable.onSetMaxPowerLevel ??= new();

                castable.onSetIdentity ??= new();

                castable.fields.followBody = followBody;
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
            public CastedVFX casted;
            public Location source;
            public Location target;

            public Vector2 chargeLevels;
            public Vector2 comboSteps;

            public readonly void GenerateEffect(CastableProperties castable, CastableStats stats)
            {
                if (casted != null)
                {
                    CastedVFX body = (PrefabUtility.InstantiatePrefab(casted.gameObject) as GameObject).GetComponent<CastedVFX>(); // castable.transform);
                    body.transform.SetParent(castable.transform);
                    ConnectCastedComponent(body, castable, stats);

                    body.powerRange = chargeLevels;
                    body.comboRange = comboSteps;
                    body.applyOnSet = true;

                    castable.fields.toLocations.Add(new(body, source, target));
                    castable.fields.castingMethods.Add(body.gameObject);
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

            public readonly Casted PrepareExecutionMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot, GameObject gameObject, Damager damager = null)
            {
                return method switch
                {
                    ExecutionMethod.ColliderBased => PrepareCollisionMethod(executor, stats, pivot, damager),
                    ExecutionMethod.ProjectileBased => PrepareProjectileMethod(executor, stats, pivot, damager),
                    ExecutionMethod.SelectionBased => null,
                    _ => null,
                };
            }

            public readonly Casted PrepareCollisionMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot, Damager damager = null)
            {
                pivot.enabled = false;
                if (colliderPrefab != null)
                {
                    CastedCollider collider = (PrefabUtility.InstantiatePrefab(colliderPrefab.gameObject) as GameObject).GetComponent<CastedCollider>(); // , pivot.transform);
                    collider.transform.SetParent(pivot.transform);
                    ConnectCastedComponent(collider, executor, stats);
                    collider.powerRange = chargeLevels;
                    collider.comboRange = comboSteps;

                    executor.fields.toLocations.Add(new(collider, source, target));
                    executor.fields.castingMethods.Add(collider.gameObject);
                    
                    if (damager != null)
                    {
                        collider.hitDamageable = new();
                        collider.leftDamageable = new();
                        UnityEventTools.AddPersistentListener(collider.hitDamageable, damager.HitDamageable);
                        UnityEventTools.AddPersistentListener(collider.leftDamageable, damager.LeftDamageable);
                    }

                    return collider;
                }
                return null;
            }

            public readonly Casted PrepareProjectileMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot, Damager damager = null)
            {
                GameObject castedObject = new(name);
                castedObject.transform.parent = pivot.transform;
                executor.fields.castingMethods.Add(castedObject);
                
                GameObject pivotObject = new($"{name} Pivot");
                pivotObject.transform.parent = castedObject.transform;
                Pivot castedPivot = pivotObject.AddComponent<Pivot>();

                Casted casted = AddCastedComponent(castedObject, executor, stats);
                casted.powerRange = chargeLevels;
                casted.comboRange = comboSteps;

                ProjectileSpawner spawner = castedObject.AddComponent<ProjectileSpawner>();
                UnityEventTools.AddPersistentListener(casted.onCast, spawner.Spawn);
                spawner.pivot = castedPivot.transform;
                spawner.lifeSpan = projectileLifeSpan;
                spawner.applyOnSet = false;
                castedPivot.enabled = false;
                executor.fields.toLocations.Add(new(spawner, source, target));

                if (projectilePrefab != null)
                {
                    Projectile projectile = (PrefabUtility.InstantiatePrefab(projectilePrefab.gameObject) as GameObject).GetComponent<Projectile>(); //, castedPivot.transform);
                    projectile.transform.SetParent(castedPivot.transform);
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

                return casted;
            }
        }


        //// Results

        //private void PrepareResultDirectory()
        //{
        //    fullDirectory = baseDirectory;

        //    // Adjust for adding a sub folder using the output name.
        //    if (createSubFolder)
        //    {
        //        // Fix the Directory components to make sure they're valid.
        //        if (!fullDirectory.EndsWith('/'))
        //        {
        //            fullDirectory += "/";
        //        }
        //        fullDirectory += outputName;
        //    }

        //    // Loop through directories, creating them as necessary.
        //    string[] steps = (fullDirectory).Split('/', System.StringSplitOptions.RemoveEmptyEntries);
        //    string lastPath = steps[0];
        //    for (int i = 1; i < steps.Length; i++)
        //    {
        //        if (steps[i].Trim() != "")
        //        {
        //            string path = string.Join('/', steps, 0, i + 1);
        //            if (!AssetDatabase.IsValidFolder(path))
        //            {
        //                AssetDatabase.CreateFolder(lastPath, steps[i]);
        //            }
        //            lastPath = path;
        //        }
        //    }
        //}
    }
}
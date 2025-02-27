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

namespace HotD.Generators
{
    using Castables;
    using static Castables.Coordination;
    using static HotD.Generators.StateCastGenerator.CastableSettings;
    using static Castables.ExecutionMethod;
    using static HotD.Castables.CastableFields;
    using static CastableStats;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using UnityEngine.Events;
    using System.CodeDom.Compiler;

    [CreateAssetMenu(fileName = "NewStateCastGenerator", menuName = "Loadouts/State Cast Generator", order = 1)]
    public class StateCastGenerator : Generator
    {
        public enum ExecutionType { Passive, ColliderBased, ProjectileBased, SelectionBased}

        [Header("Parameters")]
        public CastableSettings settings = new();
        [Space]
        public CastableStats stats;
        public float[] chargeTimes = new float[3];
        public List<Effect> effects;

        [Header("Targeting")]
        [Tooltip("Used to describe the shape of the animations and weapon art that should be used.")]
        public ActionType actionType;
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
                                if (oldItem != null)
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
                    castable.AddBaseTransitions();

                    // Executors
                    List<CastStateExecutor> executors = new();

                    // Equipped
                    if (stats.useComboCooldown)
                    {
                        DelegatedExecutor equippedExecutor = GenerateExecutor(castable, CastState.Equipped, "Equipped");
                        executors.Add(equippedExecutor);
                        if (settings.useComboSteps)
                        {
                            Timer coolDownTimer = GenerateComboIncrementation(equippedExecutor);
                        }
                        if (stats.useChargeUp)
                        {
                            AddChargeReset(equippedExecutor);
                        }
                    }

                    // Activation
                    if (actionType == ActionType.Passive && settings.castOn.HasFlag(CastOn.Trigger)) // The default, bare-bones activation transitions.
                    {
                        castable.AddInstantCastOnTriggerTransitions();
                    }
                    else // Should only do fancy stuff if we have anim coordination to wait on, or we're not casting on trigger.
                    {
                        DelegatedExecutor activationExecutor = GenerateExecutor(castable, CastState.Activating, "Activation");
                        executors.Add(activationExecutor);
                        if (stats.useChargeUp)
                        {
                            castable.AddChargeTransitions();
                            Charger charger = GenerateCharger(activationExecutor);
                        }
                        else
                        {
                            castable.AddComboTransitions();
                            GenerateWindUp(activationExecutor);
                        }
                    }

                    // Action Buffering
                    if (settings.useTriggerBuffer)
                    {
                        StateBuffer buffer = castable.gameObject.AddComponent<StateBuffer>();
                        buffer.AddTriggerBuffer(settings.triggerBufferTime);
                    }

                    // Execution
                    DelegatedExecutor executionExecutor = GenerateExecutor(castable, CastState.Executing, "Execution");
                    executors.Add(executionExecutor);
                    if (executions.Count > 0)
                    {
                        Comboer comboer = GenerateComboer(executionExecutor, stats, pivot, gameObject);
                    }
                    else
                    {
                        GenerateWindDown(executionExecutor);
                    }

                    // Cooldown
                    if (stats.useCooldown)
                    {
                        DelegatedExecutor cooldownExecutor = GenerateExecutor(castable, CastState.Cooldown, "Cooldown");
                        executors.Add(cooldownExecutor);
                        castable.AddCooldownTransitions();
                        if (stats.useChargeUp)
                        {
                            Charger discharger = GenerateDischarger(cooldownExecutor);
                        }
                        else
                        {
                            Timer coolDownTimer = GenerateCooldownTimer(cooldownExecutor);
                        }
                    }

                    // Effects
                    if (effects.Count > 0)
                    {
                        CastListenerDistributor effectManager = GenerateCastListenerDistributor(castable, "Effects");
                        foreach (var effect in effects)
                        {
                            ICastListener listener = effect.Generate(effectManager, stats);
                            effectManager.AddListener(listener);
                        }
                        effectManager.ChargeTimesSet(chargeTimes);
                        foreach (var executor in executors)
                        {
                            executor.ToTriggerListeners = effectManager.SetTriggers;
                            executor.onTriggers ??= new();
                            UnityEventTools.AddPersistentListener(executor.onTriggers, effectManager.SetTriggers);
                            Assert.IsNotNull(executor.ToTriggerListeners);
                        }
                    }

                    // Fields
                    Context context = new(stats.targetIdentity, Range.InAttackRange, new(), new(), new(0, stats.Range), 50);
                    castable.fields.activationStatusClass = stats.GetStatusClass(StatusType.Activation);
                    castable.fields.executionStatusClass = stats.GetStatusClass(StatusType.Execution);
                    castable.fields.hitStatusClass = stats.GetStatusClass(StatusType.Hit);

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
            settings.ApplyToCastable(castable);
            castable.fields.pivot = pivot.transform;
            castable.fields.Stats = stats;
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

        private void GenerateWindUp(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            // Wait For Wind Up Transition
            TransitionEvent windUpTransition = actionType switch
            {
                ActionType.Passive =>   new("Wind Up (Instantaneous)", CastAction.Start, Triggers.StartCast, Triggers.StartAction),
                _ =>                    new("Wind Up", CastAction.Start, Triggers.StartCast, Triggers.StartAction, CastAction.End),
            };
            executor.supportedTransitions.Add(windUpTransition);
        }

        private Charger GenerateCharger(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            executor.connectToFieldEvents = true;
            
            Charger charger = executor.gameObject.AddComponent<Charger>();
            charger.InitializeEvents();
            charger.chargeTimes = chargeTimes;

            // Start Transition
            TransitionEvent startTransition = new("Start", CastAction.Start, Triggers.StartAction, Triggers.StartAction);
            UnityEventTools.AddPersistentListener(startTransition.startAction, charger.Begin);
            executor.supportedTransitions.Add(startTransition);

            // Release Transition
            CastAction releaseTriggerAction = settings.castOn.HasFlag(CastOn.Release) ? CastAction.Release | CastAction.End : CastAction.End;
            TransitionEvent releaseTransition = actionType switch
            {
                ActionType.Passive =>   new("Release", releaseTriggerAction, Triggers.StartCast),
                _ =>                    new("Release", releaseTriggerAction, Triggers.StartCast, Triggers.None, CastAction.Continue, true),
            };
            executor.supportedTransitions.Add(releaseTransition);
            UnityEventTools.AddPersistentListener(releaseTransition.startAction, charger.Interrupt);
            UnityEventTools.AddFloatPersistentListener(releaseTransition.startAction, charger.SkipToLevel, 1);

            // Keep Power Level Updated
            UnityEventTools.AddPersistentListener(charger.onCharge, executor.SetPowerLevel);
            UnityEventTools.AddPersistentListener(executor.fieldEvents.onSetMaxPowerLevel, charger.SetMaxLevel);
            
            // Executor On Full Charge?
            if (settings.castOn.HasFlag(CastOn.ChargeUp))
                UnityEventTools.AddPersistentListener(charger.onCharged, executor.End);
                
            return charger;
        }

        public void GenerateWindDown(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            // Start Transition
            TransitionEvent startTransition = actionType switch
            {
                ActionType.Passive =>   new("Start", CastAction.Start, Triggers.None, Triggers.None, CastAction.None, false),
                _ =>                    new("Start", CastAction.Start, Triggers.None, Triggers.None, CastAction.End, false),
            };
            executor.supportedTransitions.Add(startTransition);
        }

        public Comboer GenerateComboer(DelegatedExecutor executor, CastableStats stats, Pivot pivot, GameObject gameObject)
        {
            Assert.IsNotNull(executor);

            executor.connectToFieldEvents = true;

            // Start Transition
            TransitionEvent startTransition = actionType switch
            {
                ActionType.Passive =>   new("Start", CastAction.Start, Triggers.None, Triggers.StartCast, CastAction.None, false),
                _ =>                    new("Start", CastAction.Start, Triggers.None, Triggers.StartCast, CastAction.End, true)
            };
            executor.supportedTransitions.Add(startTransition);

            // End Transition
            string endName = settings.endOn.HasFlag(EndOn.Release) ? "Release / End" : "End";
            CastAction endTriggerAction = settings.endOn.HasFlag(EndOn.Release) ? CastAction.Release | CastAction.End : CastAction.End;
            TransitionEvent endTransition = new(endName, endTriggerAction, Triggers.None, Triggers.EndCast | Triggers.EndAction);
            executor.supportedTransitions.Add(endTransition);

            Comboer comboer = executor.gameObject.AddComponent<Comboer>();
            comboer.ignoreStep = !settings.useComboSteps;

            GameObject methods = new("Methods");
            methods.transform.SetParent(comboer.transform);
            Pivot methodPivot = methods.AddComponent<Pivot>();
            methodPivot.body = pivot.body;

            if (settings.useComboSteps)
            {
                // Keep Combo Step Updated
                UnityEventTools.AddPersistentListener(executor.fieldEvents.onSetComboStep, comboer.SetStep);
            }

            for (int i = 0; i < executions.Count; i++)
            {
                Execution execution = executions[i];
                Castables.ExecutionMethod method = execution.PrepareExecutionMethod(executor, stats, methodPivot, gameObject);
                if (method != null)
                {
                    comboer.AddStep(i + 1, method.gameObject);
                }
            }

            return comboer;
        }

        private void AddChargeReset(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            // Start Transition
            TransitionEvent startTransition = new("Start", CastAction.Start, Triggers.None, Triggers.None);
            if (!TransitionEvent.TryFindActionEvent(CastAction.Start, executor.supportedTransitions, out startTransition, startTransition))
            {
                executor.supportedTransitions.Add(startTransition);
            }
            UnityEventTools.AddVoidPersistentListener(startTransition.startAction, executor.ResetPowerLevel);
        }

        private Timer GenerateComboIncrementation(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            if (!settings.usePowerLevelAsComboStep)
            {
                // Trigger Transition
                TransitionEvent triggerTransition = new("Trigger", CastAction.Trigger, Triggers.None, Triggers.None);
                UnityEventTools.AddVoidPersistentListener(triggerTransition.startAction, executor.IncrementComboStep);
                executor.supportedTransitions.Add(triggerTransition);
            }

            if (stats.useComboCooldownTime)
            {
                Timer coolDownTimer = executor.gameObject.AddComponent<Timer>();
                coolDownTimer.onComplete = new();
                coolDownTimer.length = stats.ComboCooldown; // TODO: Account for bonuses

                // Start Transition
                TransitionEvent startTransition = new("Start", CastAction.Start);
                UnityEventTools.AddVoidPersistentListener(startTransition.startAction, coolDownTimer.Play);
                executor.supportedTransitions.Add(startTransition);

                // Reset on Timer Complete
                UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, executor.ResetComboStep);

                // End Transition
                TransitionEvent endTransition = new("End", CastAction.End);
                UnityEventTools.AddVoidPersistentListener(endTransition.startAction, coolDownTimer.Interrupt);
                executor.supportedTransitions.Add(endTransition);
                
                return coolDownTimer;
            }
            else
            {
                // Start Transition
                TransitionEvent startTransition = new("Start", CastAction.Start);
                UnityEventTools.AddVoidPersistentListener(startTransition.startAction, executor.ResetComboStep);
                executor.supportedTransitions.Add(startTransition);

                return null;
            }
        }

        private Timer GenerateCooldownTimer(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);

            executor.connectToFieldEvents = true;

            Timer coolDownTimer = executor.gameObject.AddComponent<Timer>();
            coolDownTimer.onComplete = new();
            coolDownTimer.length = stats.Cooldown; // TODO: Account for bonuses

            Debug.Log($"Cooldown time: {coolDownTimer.length}; {stats.Cooldown}");

            // Start Transition
            TransitionEvent startTransition = new("Start", CastAction.Start);
            UnityEventTools.AddVoidPersistentListener(startTransition.startAction, coolDownTimer.Play);
            executor.supportedTransitions.Add(startTransition);

            // Finish on Timer Complete
            UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, executor.End);

            // Keep Cooldown Length Updated
            UnityEventTools.AddPersistentListener(executor.fieldEvents.onSetCooldown, coolDownTimer.SetLength);

            return coolDownTimer;
        }

        public Charger GenerateDischarger(DelegatedExecutor executor)
        {
            Assert.IsNotNull(executor);
            executor.connectToFieldEvents = true;

            Charger discharger = executor.gameObject.AddComponent<Charger>();
            discharger.onCharge = new();
            discharger.onCharged = new();
            discharger.discharge = true;
            discharger.distributeChargeTimes = true;
            discharger.chargeTimes = new float[1] { stats.Cooldown };

            Debug.Log($"Discharge time: {chargeTimes[0]}; {stats.Cooldown}");

            // Keep Charger Settings Updated
            UnityEventTools.AddPersistentListener(executor.fieldEvents.onSetPowerLevel, discharger.SetMaxLevel);
            UnityEventTools.AddPersistentListener(executor.fieldEvents.onSetCooldown, discharger.SetChargeTimeScale);

            // Start Transition
            TransitionEvent startTransition = new("Start", CastAction.Start);
            UnityEventTools.AddVoidPersistentListener(startTransition.startAction, discharger.Begin);
            executor.supportedTransitions.Add(startTransition);

            // Finish on Charged
            UnityEventTools.AddVoidPersistentListener(discharger.onCharged, executor.End);

            // Keep Charge Level Updated
            UnityEventTools.AddPersistentListener(discharger.onCharge, executor.SetPowerLevel);

            return discharger;
        }

        static private Damager AddDamager(CastProperties castable, GameObject gameObject, CastableStats stats, string label, int damageBase = 0, float tickRate = 0.25f, bool doTick = false, bool doTickOnProc = true)
        {
            Assert.IsNotNull(castable);
            if (stats.dealDamage)
            {
                Damager damager = gameObject.AddComponent<Damager>();
                damager.Name = label;
                damager.damage = damageBase; // + stats.Damage; // TODO: Account for bonuses
                damager.tickRate = tickRate;
                damager.shouldTick = doTick;
                damager.shouldTickOnProc = doTickOnProc;
                UnityEventTools.AddPersistentListener(castable.fieldEvents.onSetIdentity, damager.SetIdentity);
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
            item.actionType = actionType;
            item.targetingMethod = targetingMethod;
            item.aimingMethod = aimingMethod;
            item.stats = stats;
            item.context = context;
            items.Add(item);
            return item;
        }

        private static Castables.ExecutionMethod AddExecutionMethod(GameObject castedObject, CastProperties castable)
        {
            Castables.ExecutionMethod executionMethod = castedObject.AddComponent<Castables.ExecutionMethod>();
            executionMethod.aimAtCrosshair = true;
            executionMethod.initializeOffOf = castable;
            return executionMethod;
        }

        private static CastListenerDistributor GenerateCastListenerDistributor(CastProperties castable, string name)
        {
            GameObject child = new(name);
            child.transform.SetParent(castable.transform);
            
            DependentCastProperties properties = child.AddComponent<DependentCastProperties>();
            properties.initializeOffOf = castable;
            
            CastListenerDistributor distributor = child.AddComponent<CastListenerDistributor>();
            distributor.properties = properties;

            properties.connectToFieldEvents = true;
            UnityEventTools.AddPersistentListener(properties.fieldEvents.onSetOwner, distributor.SetOwner);
            UnityEventTools.AddPersistentListener(properties.fieldEvents.onSetPowerLevelInt, distributor.LevelSet);

            return distributor;
        }

        private static Casted AddCastedComponent(GameObject castedObject, CastProperties castable, CastableStats stats)
        {
            Casted casted = castedObject.AddComponent<Casted>();
            ConnectCastedComponent(casted, castable, stats);
            return casted;
        }

        private static void ConnectCastedComponent(Casted casted, CastProperties castable, CastableStats stats)
        {
            //casted.gameObject.SetActive(false);
            //casted.enabled = false;

            casted.Stats = stats;
            casted.PowerLimit = stats.chargeLimit;

            casted.InitializeUnityEvents();
            //UnityEventTools.AddPersistentListener(stats.chargeLimit.updatedFinal, casted.OnSetPowerLimit);

            //UnityEventTools.AddPersistentListener(castable.castEvents.onTrigger, casted.OnTrigger);
            //UnityEventTools.AddPersistentListener(castable.castEvents.onRelease, casted.OnRelease);
            //UnityEventTools.AddPersistentListener(castable.castEvents.onStartCast, casted.OnStartCast);
            //UnityEventTools.AddPersistentListener(castable.castEvents.onEndCast, casted.OnEndCast);
            UnityEventTools.AddPersistentListener(castable.fieldEvents.onSetPowerLevel, casted.SetPowerLevel);
        }


        // Structs

        [Serializable]
        public struct CastableSettings
        {
            [Flags]
            public enum CastOn
            {
                None = 0,
                Trigger = 1 << 0,
                Release = 1 << 1,
                ChargeUp = 1 << 2,
            }

            [Flags]
            public enum EndOn
            {
                None = 0,
                Release = 1 << 0,
            }

            public bool followBody;
            public CastOn castOn;
            public EndOn endOn;
            public bool useTriggerBuffer;
            [ConditionalField("useTriggerBuffer")]
            public float triggerBufferTime;
            public bool useComboSteps;
            public bool usePowerLevelAsComboStep;
            
            public readonly void ApplyToCastable(CastProperties castable)
            {
                castable.InitializeEvents();

                castable.fields.followBody = followBody;
                castable.fields.usePowerLevelAsComboStep = usePowerLevelAsComboStep;
            }
        }

        [Serializable]
        public struct Effect
        {
            public Effect(CastLocation source = CastLocation.Character, CastLocation target = CastLocation.FiringPoint, Vector2 chargeLevels = new(), Vector2 comboSteps = new())
            {
                this.name = $"{source} -> {target}";
                prefab = null;
                this.source = source;
                this.target = target;

                this.chargeLevels = chargeLevels;
                this.comboSteps = comboSteps;
            }

            public string name;
            public GameObject prefab;
            public CastLocation source;
            public CastLocation target;

            public Vector2 chargeLevels;
            public Vector2 comboSteps;

            public readonly ICastListener Generate(CastListenerDistributor distributor, CastableStats stats, bool initializeValues = false, float[] chargeTimes = null)
            {
                if (prefab != null)
                {
                    GameObject listenerObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    listenerObject.transform.SetParent(distributor.transform);

                    if (listenerObject.TryGetComponent<ICastListener>(out var listener))
                    {
                        Debug.Log("Has ICastListener component.");
                        if (initializeValues)
                        {
                            listener.ChargeTimes = chargeTimes;
                        }
                    }
                    else
                    {
                        Debug.Log("Does not have ICastListener component.");
                    }

                    return listener;
                }
                return null;
            }
        }

        [Serializable]
        public struct Execution
        {
            [Serializable]
            public struct StatusesToApply
            {
                public List<Status> chargeEffects;
                public List<Status> activeEffects;
                public List<Status> hitEffects;
                public List<Status> lingeringEffects;

                public readonly List<Status> this[StatusType type]
                {
                    get => type switch
                    {
                        StatusType.Activation => chargeEffects,
                        StatusType.Execution => activeEffects,
                        StatusType.Hit => hitEffects,
                        StatusType.Lingering => lingeringEffects,
                        _ => null
                    };
                }
            }

            public Execution(ExecutionType method=ExecutionType.Passive, CastLocation source=CastLocation.Character, CastLocation target=CastLocation.FiringPoint, Vector2 chargeLevels=new(), Vector2 comboSteps=new())
            {
                this.name = method.ToString();
                this.method = method;
                this.source = source;
                this.target = target;

                this.chargeLevels = chargeLevels;
                this.comboSteps = comboSteps;
                this.statuses = new();

                this.baseDamage = 0;
                this.tickRate = 0.25f;
                this.doTick = false;
                this.tickOnProc = true;

                this.passivePrefab = null;
                this.colliderLifeSpan = 1;
                this.colliderPrefabs = new();
                this.projectileLifeSpan = 1;
                this.projectilePrefabs = new();
            }

            public string name;
            public ExecutionType method;
            public CastLocation source;
            public CastLocation target;

            public Vector2 chargeLevels;
            public Vector2 comboSteps;
            public StatusesToApply statuses;

            // Damage
            [Header("Damage")]
            public int baseDamage;
            public float tickRate;
            public bool doTick;
            public bool tickOnProc;

            // Execution: Passive
            [Header("Settings: Passive")]
            [ConditionalField("method", false, ExecutionType.Passive)]
            public GameObject passivePrefab;

            // Execution: Collider
            [Header("Settings: Collider")]
            [ConditionalField("method", false, ExecutionType.ColliderBased)]
            public float colliderLifeSpan;
            [ConditionalField("method", false, ExecutionType.ColliderBased)]
            public List<GameObject> colliderPrefabs;

            // Execution: Projectile
            [Header("Settings: Projectile")]
            [ConditionalField("method", false, ExecutionType.ProjectileBased)]
            public float projectileLifeSpan;
            [ConditionalField("method", false, ExecutionType.ProjectileBased)]
            public List<GameObject> projectilePrefabs;
            //public Projectile projectilePrefab;

            public readonly Castables.ExecutionMethod PrepareExecutionMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot, GameObject gameObject)
            {
                ExecutionMethod preparedMethod = method switch
                {
                    ExecutionType.ColliderBased => PrepareCollisionMethod(executor, stats, pivot),
                    ExecutionType.ProjectileBased => PrepareProjectileMethod(executor, stats, pivot),
                    ExecutionType.SelectionBased => null,
                    _ => PreparePassiveMethod(executor, stats, pivot),
                };

                AddStatusListener(preparedMethod, stats, StatusType.Execution, executor.fieldEvents.onSetOwner);

                return preparedMethod;
            }

            private readonly void AddStatusListener(ExecutionMethod method, CastableStats stats, StatusType statusType, UnityEvent<ICastCompatible> ownerHook)
            {
                Assert.IsNotNull(method);
                GameObject parent = method.gameObject;

                bool hasStatuses = this.statuses[statusType] != null && this.statuses[statusType]?.Count > 0;
                if (stats.TryGetStatusClass(statusType, out StatusClass statusClass) || hasStatuses)
                {
                    var statusListener = parent.AddComponent<StatusCastListener>();

                    // Connect Owner
                    UnityEventTools.AddPersistentListener(ownerHook, statusListener.SetOwner);
                    //UnityEventTools.AddPersistentListener(method.fieldEvents.onSetOwner, statusListener.SetOwner);

                    List<Status> statuses = new();

                    if (this.statuses[statusType] != null)
                        statuses.AddRange(this.statuses[statusType]);

                    if (statusClass.statuses != null)
                    {
                        statuses.AddRange(statusClass.statuses);
                    }
                    else
                    {
                        statusClass.name = parent.name;
                        statusClass.power = new(1, $"{parent.name} Starting Power", $"{parent.name}");
                    }
                    statusClass.statuses = statuses;

                    statusListener.statusClass = statusClass;
                }
            }

            public readonly ExecutionMethod PreparePassiveMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot)
            {
                // Object
                GameObject castedObject = AddCastedObject(executor, pivot);

                // Method
                ExecutionMethod method = AddExecutionMethod(castedObject, executor);
                method.InitializeEvents();
                
                if (passivePrefab != null)
                {
                    // Prefab Instance
                    GameObject instance = PrefabUtility.InstantiatePrefab(passivePrefab) as GameObject;
                    instance.transform.SetParent(castedObject.transform);

                    if (instance.TryGetComponent<ACastCompatibleFollower>(out var follower))
                    {
                        UnityEventTools.AddPersistentListener(method.fieldEvents.onSetOwner, follower.SetOwner);
                    }
                }
                
                return method;
            }

            public readonly ExecutionMethod PrepareCollisionMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot)
            {
                pivot.enabled = false;

                ExecutionMethod method = PrepareNonPassiveMethod(executor, stats, pivot, out GameObject castedObject, out Pivot castedPivot, out Damager damager);

                // Cast Location Follower
                AddLocationFollower(method, castedPivot, CastLocation.WeaponPoint, CastLocationFollower.Mode.AlsoOnUpdate);

                // Cast Object Spawner
                CastObjectSpawner spawner = AddSpawner<CastObjectSpawner>(method, castedObject, castedPivot, colliderLifeSpan);

                // Projectiles
                if (colliderPrefabs != null)
                {
                    foreach (var prefab in colliderPrefabs)
                    {
                        // Prefab
                        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                        instance.transform.SetParent(castedPivot.transform);
                        instance.transform.localPosition = new();

                        // Projectile
                        if (instance.TryGetComponent<CastedCollider>(out var castObject))
                        {
                            spawner.castObject = castObject;
                            castObject.hitDamageable = new();
                            castObject.leftDamageable = new();

                            // Damage
                            if (damager != null)
                            {
                                UnityEventTools.AddPersistentListener(castObject.hitDamageable, damager.HitDamageable);
                                UnityEventTools.AddPersistentListener(castObject.leftDamageable, damager.LeftDamageable);
                            }
                        }
                    }
                }

                return method;
            }

            public readonly ExecutionMethod PrepareProjectileMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot)
            {
                ExecutionMethod method = PrepareNonPassiveMethod(executor, stats, pivot, out GameObject castedObject, out Pivot castedPivot, out Damager damager);

                // Cast Location Follower
                AddLocationFollower(method, castedPivot, CastLocation.FiringPoint, CastLocationFollower.Mode.OnlyOnStart);

                // Projectile Spawner
                ProjectileSpawner spawner = AddSpawner<ProjectileSpawner>(method, castedObject, castedPivot, projectileLifeSpan);

                // Projectiles
                if (projectilePrefabs != null)
                {
                    foreach (var prefab in projectilePrefabs)
                    {
                        // Prefab
                        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                        instance.transform.SetParent(castedPivot.transform);
                        instance.transform.localPosition = new();
                    
                        // Projectile
                        if (instance.TryGetComponent<Projectile>(out var projectile))
                        {
                            spawner.projectile = projectile;
                            projectile.hitDamageable = new();
                            projectile.leftDamageable = new();

                            if (damager != null)
                            {
                                UnityEventTools.AddPersistentListener(projectile.hitDamageable, damager.HitDamageable);
                                UnityEventTools.AddPersistentListener(projectile.leftDamageable, damager.LeftDamageable);
                            }
                        }
                    }
                }

                return method;
            }

            private readonly ExecutionMethod PrepareNonPassiveMethod(DelegatedExecutor executor, CastableStats stats, Pivot pivot, out GameObject castedObject, out Pivot castedPivot, out Damager damager)
            {
                // Object
                castedObject = AddCastedObject(executor, pivot);
                castedPivot = AddCastedPivot(castedObject);
                castedPivot.body = castedPivot.transform;

                // Method
                ExecutionMethod method = AddExecutionMethod(castedObject, executor);
                method.InitializeEvents();

                // Damager
                damager = AddDamager(executor, method.gameObject, stats, name, baseDamage, tickRate, doTick, tickOnProc);

                return method;
            }

            private readonly CastLocationFollower AddLocationFollower(ExecutionMethod method, Pivot castedPivot, CastLocation target, CastLocationFollower.Mode mode)
            {
                CastLocationFollower follower = castedPivot.GetOrAddComponent<CastLocationFollower>();
                follower.SetTarget(target);
                follower.SetMode(mode);
                follower.SetParent(false);
                UnityEventTools.AddPersistentListener(method.fieldEvents.onSetOwner, follower.SetOwner);

                return follower;
            }

            private readonly GameObject AddCastedObject(DelegatedExecutor executor, Pivot pivot)
            {
                GameObject castedObject = new(name);
                castedObject.transform.parent = pivot.transform;
                executor.fields.castingMethods.Add(castedObject);
                return castedObject;
            }

            private readonly Pivot AddCastedPivot(GameObject castedObject)
            {
                GameObject pivotObject = new($"{name} Pivot");
                pivotObject.transform.parent = castedObject.transform;
                return pivotObject.AddComponent<Pivot>();
            }

            private readonly T AddSpawner<T>(ExecutionMethod method, GameObject castedObject, Pivot castedPivot, float lifeSpan) where T : Spawner
            {
                T spawner = castedObject.AddComponent<T>();
                UnityEventTools.AddPersistentListener(method.onEnable, spawner.Spawn);
                UnityEventTools.AddPersistentListener(method.fieldEvents.onSetCollisionExceptions, spawner.SetExceptions);
                method.positionables.Add(spawner);
                spawner.pivot = castedPivot.transform;
                spawner.lifeSpan = lifeSpan;
                spawner.applyOnSet = false;
                spawner.spawnOnEnable = false;

                return spawner;
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
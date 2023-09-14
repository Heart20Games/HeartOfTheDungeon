using MyBox;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;
using static Loadout;
using Range = Body.Behavior.ContextSteering.CSContext.Range;

[CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/Castable Generator", order = 1)]
public class CastableGenerator : ScriptableObject
{
    public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
    public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

    [Header("Parameters")]
    public string outputName = "New Castable";
    public bool followBody = true;
    public bool castOnTrigger;
    public bool castOnRelease;
    public bool unCastOnRelease;
    public bool castOnChargeUp;
    [Space]
    public CastableStats stats;
    public CastableBody bodyPrefab;

    [Header("Targeting")]
    public TargetingMethod targetingMethod;

    [Header("Execution")]
    public ExecutionMethod executionMethod;

    // Execution: Collider
    [ConditionalField("executionMethod", false, ExecutionMethod.ColliderBased)]
    public CastedBody castedPrefab;

    // Execution: Projectile
    [ConditionalField("executionMethod", false, ExecutionMethod.ProjectileBased)]
    public float projectileLifeSpan;
    [ConditionalField("executionMethod", false, ExecutionMethod.ProjectileBased)]
    public Projectile projectilePrefab;

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

    public void GenerateCastable()
    {
        if (!Application.isEditor)
        {
            Debug.LogWarning("Cannot generate castable outside the Editor");
        }
        else
        {
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

                // Set up the Game Object
                GameObject gameObject = new(outputName);
                Castable castable = gameObject.AddComponent<Castable>();
                castable.onCast = new();
                castable.onTrigger = new();
                castable.onRelease = new();
                castable.onSetIdentity = new();
                castable.followBody = followBody;
                castable.castOnTrigger = castOnTrigger;
                castable.castOnRelease = castOnRelease;
                castable.unCastOnRelease = unCastOnRelease;

                // Pivot
                GameObject pivotObject = new("Pivot");
                Pivot pivot = pivotObject.AddComponent<Pivot>();
                pivot.transform.SetParent(gameObject.transform, false);

                // Body
                CastableBody body = null;
                if (bodyPrefab != null)
                {
                    body = Instantiate(bodyPrefab, castable.transform);
                    castable.body = body;
                }

                // Stats
                Damager damager = null;
                if (stats.dealDamage)
                {
                    damager = gameObject.AddComponent<Damager>();
                    damager.damage = stats.Damage; // TODO: Account for bonuses
                    UnityEventTools.AddPersistentListener(castable.onSetIdentity, damager.SetIdentity);
                }

                Charger charger = null;
                if (stats.useChargeUp)
                {
                    charger = gameObject.AddComponent<Charger>();
                    charger.onBegin = new();
                    charger.onCharge = new();
                    charger.onCharged = new();
                    charger.onInterrupt = new();
                    charger.length = stats.ChargeUp;
                    UnityEventTools.AddPersistentListener(castable.onTrigger, charger.Begin);
                    UnityEventTools.AddPersistentListener(castable.onRelease, charger.Interrupt);
                    UnityEventTools.AddPersistentListener(charger.onCharge, castable.SetPowerLevel);
                    if (castOnChargeUp)
                        UnityEventTools.AddPersistentListener(charger.onCharged, castable.Cast);
                }

                Timer coolDownTimer = null;
                if (stats.useCooldown)
                {
                    coolDownTimer = gameObject.AddComponent<Timer>();
                    coolDownTimer.onComplete = new();
                    coolDownTimer.length = stats.Cooldown; // TODO: Account for bonuses
                    UnityEventTools.AddVoidPersistentListener(castable.onCast, coolDownTimer.Play);
                    UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, castable.UnCast);
                }

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
                switch (executionMethod)
                {
                    case ExecutionMethod.ColliderBased:
                    {
                            pivot.enabled = false;
                            if (castedPrefab != null)
                            {
                                CastedBody collider = Instantiate(castedPrefab, pivot.transform);
                                pivot.body = collider.transform;
                                collider.enabled = false;
                                if (damager != null)
                                {
                                    collider.hitDamageable = new();
                                    collider.leftDamageable = new();
                                    UnityEventTools.AddPersistentListener(collider.hitDamageable, damager.HitDamageable);
                                    UnityEventTools.AddPersistentListener(collider.leftDamageable, damager.LeftDamageable);
                                }
                            }
                            break;
                    }
                    case ExecutionMethod.ProjectileBased:
                    {
                            ProjectileSpawner spawner = gameObject.AddComponent<ProjectileSpawner>();
                            UnityEventTools.AddPersistentListener(castable.onCast, spawner.Spawn);
                            spawner.pivot = pivot.transform;
                            spawner.lifeSpan = projectileLifeSpan;
                            pivot.enabled = false;
                            
                            if (projectilePrefab != null)
                            {
                                Projectile projectile = Instantiate(projectilePrefab, pivot.transform);
                                spawner.projectile = projectile;
                                pivot.body = projectile.transform;
                                projectile.transform.position = new();
                                if (damager != null)
                                {
                                    projectile.hitDamageable = new();
                                    projectile.leftDamageable = new();
                                    UnityEventTools.AddPersistentListener(projectile.hitDamageable, damager.HitDamageable);
                                    UnityEventTools.AddPersistentListener(projectile.leftDamageable, damager.LeftDamageable);
                                }
                            }
                            break;
                    }
                    case ExecutionMethod.SelectionBased: break;
                }

                // Save to Prefab
                prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, $"{fullDirectory}/{outputName}.prefab");
                DestroyImmediate(gameObject);
                timeOfLastGeneration = Time.time;

                // Set up Item
                CastableItem item = (CastableItem)CreateInstance(typeof(CastableItem));
                AssetDatabase.CreateAsset(item, $"{fullDirectory}/{outputName}.asset");
                item.prefab = prefab.GetComponent<Castable>();
                item.stats = stats;
                item.context = context;
                items.Add(item);

                // Set up Aliases
                foreach (var output in outputs)
                {
                    CastableItem copy = item;
                    if (!output.alias.Equals(outputName))
                    {
                        copy = Instantiate(item);
                        AssetDatabase.CreateAsset(copy, $"{fullDirectory}/{output.alias}.asset");
                        items.Add(copy);
                    }
                    foreach (var target in output.targets)
                    {
                        if (target.slot != Slot.None)
                            target.loadout.SetSlot(target.slot, copy);
                    }
                }
            }
        }
    }

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
                string path = string.Join('/', steps, 0, i+1);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder(lastPath, steps[i]);
                }
                lastPath = path;
            }
        }
    }
}

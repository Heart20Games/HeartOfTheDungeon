using MyBox;
using System.Drawing.Printing;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/CastableGenerator", order = 1)]
public class CastableGenerator : ScriptableObject
{
    public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
    public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

    [Header("Parameters")]
    public string outputName = "New Castable";
    public GameObject rig;
    public bool followBody = true;
    [Space]
    public CastableStats stats;

    [Header("Targeting")]
    public TargetingMethod targetingMethod;

    [Header("Execution")]
    public ExecutionMethod executionMethod;
    [ConditionalField("executionMethod", false, ExecutionMethod.ProjectileBased)]
    public float projectileLifeSpan;
    [ConditionalField("executionMethod", false, ExecutionMethod.ProjectileBased)]
    public Projectile projectilePrefab;

    [Header("Results")]
    public string castablesDirectory = "Assets/Configuration/Castables/";
    public bool createSubFolder = true;
    public bool overwrite = true;
    public bool replace = true;
    [ReadOnly] public string fullDirectory = "";
    [ReadOnly] public GameObject prefab;
    [ReadOnly] public CastableItem item;
    [ReadOnly] public float timeOfLastGeneration;
    
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
                if (replace && !sameDirectory)
                {
                    if (prefab != null)
                    {
                        AssetDatabase.DeleteAsset($"{oldDirectory}/{prefab.name}.prefab");
                        prefab = null;
                    }
                    if (item != null)
                    {
                        AssetDatabase.DeleteAsset($"{oldDirectory}/{item.name}.asset");
                        item = null;
                    }
                }

                // Trim the name so it doesn't look like a series of subdirectories
                outputName = outputName.Trim('/');

                // Set up the Game Object
                GameObject gameObject = new(outputName);
                Castable castable = gameObject.AddComponent<Castable>();
                castable.doCast = new();
                castable.onCast = new();
                castable.onSetIdentity = new();
                castable.followBody = followBody;

                GameObject pivotObject = new("Pivot");
                Pivot pivot = pivotObject.AddComponent<Pivot>();
                pivot.transform.SetParent(gameObject.transform, false);

                // Stats
                Damager damager = null;
                if (stats.dealDamage)
                {
                    damager = gameObject.AddComponent<Damager>();
                    damager.damage = stats.baseDamage; // TODO: Account for bonuses
                    UnityEventTools.AddPersistentListener(castable.onSetIdentity, damager.SetIdentity);
                }

                Timer coolDownTimer = null;
                if (stats.useCooldown)
                {
                    coolDownTimer = gameObject.AddComponent<Timer>();
                    coolDownTimer.onComplete = new();
                    coolDownTimer.length = stats.baseCooldown; // TODO: Account for bonuses
                    UnityEventTools.AddPersistentListener(castable.onCast, coolDownTimer.Play);
                    UnityEventTools.AddPersistentListener(coolDownTimer.onComplete, castable.UnCast);
                }

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
                    case ExecutionMethod.ColliderBased: break;
                    case ExecutionMethod.ProjectileBased:
                    {
                            ProjectileSpawner spawner = gameObject.AddComponent<ProjectileSpawner>();
                            UnityEventTools.AddPersistentListener(castable.doCast, spawner.Spawn);
                            spawner.pivot = pivot.transform;
                            spawner.lifeSpan = projectileLifeSpan;
                            pivot.enabled = false;
                            
                            if (projectilePrefab != null)
                            {
                                Projectile projectile = Instantiate(projectilePrefab, pivot.transform);
                                spawner.projectile = projectile;
                                pivot.body = projectile.transform;
                                projectile.transform.position = new();
                                projectile.hitDamageable = new();
                                projectile.leftDamageable = new();
                                UnityEventTools.AddPersistentListener(projectile.hitDamageable, damager.HitDamagable);
                                UnityEventTools.AddPersistentListener(projectile.leftDamageable, damager.LeftDamagable);
                            }
                            break;
                    }
                    case ExecutionMethod.SelectionBased: break;
                }

                // Save to Prefab
                prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, $"{fullDirectory}/{outputName}.prefab");
                DestroyImmediate(gameObject);
                timeOfLastGeneration = Time.time;

                item = (CastableItem)CreateInstance(typeof(CastableItem));
                AssetDatabase.CreateAsset(item, $"{fullDirectory}/{outputName}.asset");
                item.prefab = prefab.GetComponent<Castable>();
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

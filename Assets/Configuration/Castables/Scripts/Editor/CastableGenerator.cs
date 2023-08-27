using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/CastableGenerator", order = 1)]
public class CastableGenerator : ScriptableObject
{
    public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
    public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

    [Header("Parameters")]
    public GameObject rig;
    public CastableStats stats;
    public TargetingMethod targetingMethod;
    public ExecutionMethod executionMethod;

    [Header("Results")]
    public string castablesDirectory = "Assets/Configuration/Castables/";
    [ReadOnly] public GameObject prefab;
    [ReadOnly] public CastableItem item;
    public bool overwrite = true;
    [ReadOnly] public float timeOfLastGeneration;
    
    public void GenerateCastable()
    {
        if (!Application.isEditor)
        {
            Debug.LogWarning("Cannot generate castable outside the Editor");
        }
        else
        {
            PrepareResultDirectory();

            if (prefab == null || overwrite)
            {
                // Set up the path


                // Set up the Game Object
                GameObject gameObject = new(name);

                // Save to Prefab
                prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, $"{castablesDirectory}{name}/{name}.prefab");
                DestroyImmediate(gameObject);
                timeOfLastGeneration = Time.time;

                item = (CastableItem)CreateInstance(typeof(CastableItem));
                AssetDatabase.CreateAsset(item, $"{castablesDirectory}{name}/{name}.asset");
            }
        }
    }

    private void PrepareResultDirectory()
    {
        // Fix the Directory components to make sure they're valid.
        if (!castablesDirectory.EndsWith('/'))
        {
            castablesDirectory += "/";
        }
        name = name.Trim('/');

        // Loop through directories, creating them as necessary.
        string[] steps = (castablesDirectory + name).Split('/', System.StringSplitOptions.RemoveEmptyEntries);
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

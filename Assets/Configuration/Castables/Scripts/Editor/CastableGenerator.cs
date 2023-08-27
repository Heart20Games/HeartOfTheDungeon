using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCastableGenerator", menuName = "Loadouts/CastableGenerator", order = 1)]
public class CastableGenerator : ScriptableObject
{
    public enum TargetingMethod { TargetBased, LocationBased, DirectionBased }
    public enum ExecutionMethod { ColliderBased, ProjectileBased, SelectionBased }

    [Header("Parameters")]
    public string outputName = "New Castable";
    public GameObject rig;
    public CastableStats stats;
    public TargetingMethod targetingMethod;
    public ExecutionMethod executionMethod;

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
                gameObject.AddComponent<Castable>();

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

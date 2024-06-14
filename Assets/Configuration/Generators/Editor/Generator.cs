using UnityEditor;
using UnityEngine;

public abstract class Generator : ScriptableObject
{
    [Header("Results")]
    public string outputName = "New Object";
    public string castablesDirectory = "Assets/Configuration/TEMP/";
    public bool createSubFolder = true;
    public bool overwrite = true;
    public bool replace = true;
    public bool emptyTest = false;
    [ReadOnly] public string fullDirectory = "";
    [ReadOnly] public float timeOfLastGeneration;

    protected void PrepareResultDirectory()
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

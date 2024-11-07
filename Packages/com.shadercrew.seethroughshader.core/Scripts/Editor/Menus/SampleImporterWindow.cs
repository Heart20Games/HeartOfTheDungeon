using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class SampleImporterWindow : EditorWindow
{
    private ListRequest listRequest;
    private AddRequest addRequest;
    private bool sampleImported = false;
    private string packageName = "com.shadercrew.seethroughshader.core"; // Change this to the package name you want
    private GUIStyle redTextStyle;
    private GUIStyle textAreaBoldStyle;

    [MenuItem("Window/See-through Shader/Sample Importer")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SampleImporterWindow), false, "See-through Shader/Sample Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        packageName = EditorGUILayout.TextField("Package Name", packageName);
        redTextStyle = new GUIStyle(GUI.skin.label);
        redTextStyle.normal.textColor = Color.green;
        redTextStyle.fontSize = 15;
        redTextStyle.fontStyle = FontStyle.Bold;
        textAreaBoldStyle = new GUIStyle(GUI.skin.button);
        textAreaBoldStyle.normal.textColor = Color.white;
        textAreaBoldStyle.fontSize = 15;
        textAreaBoldStyle.fontStyle = FontStyle.Bold;
        textAreaBoldStyle.padding = new RectOffset(20, 20, 20, 20);

        if (GUILayout.Button("Import Samples", textAreaBoldStyle))
        {
            ImportSample();
        }
        if (sampleImported)
        {
            GUILayout.Label("Samples Imported", redTextStyle);
        }
    }

    void ImportSample()
    {
        //Debug.Log("Starting package list request.");
        listRequest = Client.List(); // This does not include samples information inherently
        EditorApplication.update += ProgressList;
    }

    void ProgressList()
    {
        if (listRequest.IsCompleted)
        {
            if (listRequest.Status == StatusCode.Success)
            {
                //Debug.Log("Packages listed successfully.");
                bool packageFound = false;
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageName)
                    {
                        packageFound = true;
                        //Debug.Log("Package found, attempting to find sample.");
                        foreach (Sample item in Sample.FindByPackage(packageName, package.version))
                        {
                            item.Import(Sample.ImportOptions.OverridePreviousImports);
                            sampleImported = item.isImported;
                            
                        }
                        
                       
                        



                    }
                }

                if (!packageFound)
                {
                    Debug.Log("Package not found.");
                }
            }
            else if (listRequest.Status >= StatusCode.Failure)
            {
                Debug.Log("Failed to list packages: " + listRequest.Error.message);
            }

            EditorApplication.update -= ProgressList;
        }
    }

    void ProgressAdd()
    {
        if (addRequest.IsCompleted)
        {
            if (addRequest.Status == StatusCode.Success)
            {
                string path = "Assets/Samples/See-through Shader/1.8.5/See-through Shader Examples/Scenes";

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
               
                Selection.activeObject = obj;

                EditorGUIUtility.PingObject(obj);                
                Debug.Log("Sample imported successfully.");
                Close();
            }
            else
                Debug.Log("Failed to import sample: " + addRequest.Error.message);

            EditorApplication.update -= ProgressAdd;
        }
    }
}

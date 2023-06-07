#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.SceneManagement;



using UnityEditor.SceneManagement;


[InitializeOnLoad]
public static class EditorSceneMemoryManager
{
    static EditorSceneMemoryManager()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        GarbageCollect();
    }

    [MenuItem("Tools/Force Garbage Collection")]
    static void GarbageCollect()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
    }
}
#endif
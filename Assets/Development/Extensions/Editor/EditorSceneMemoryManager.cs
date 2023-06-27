using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
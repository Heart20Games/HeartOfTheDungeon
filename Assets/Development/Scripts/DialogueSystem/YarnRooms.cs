using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static YarnRooms;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/YarnRooms", order = 1)]
public class YarnRooms : ScriptableObject
{
    [Serializable]
    public struct Room
    {
        public Room(string _name, string _sceneName)
        {
            name = _name;
            sceneName = _sceneName;
        }
        public string name;
        public string sceneName;
    }

    public string[] mainMenus = new string[2];
    public Room[] rooms = new Room[5];
    public Dictionary<string, string> bank = new();
    private List<string> validSceneNames = new();

    public bool debug;

    [ButtonMethod]
    public void Initialize()
    {
        InitializeValidSceneNames();
        bank.Clear();
        foreach (string mainMenu in mainMenus)
        {
            if (AddToBank("$MainMenu", mainMenu))
            {
                break;
            }
        }
        foreach (Room room in rooms)
        {
            AddToBank(room.name, room.sceneName);
        }
    }

    public void InitializeValidSceneNames()
    {
        validSceneNames.Clear();
        for (int i = 0; true; i++)
        {
            var s = SceneUtility.GetScenePathByBuildIndex(i);
            if (s.Length <= 0)
            {
                break;
            }
            else
            {
                validSceneNames.Add(s.Split('/').Last().RemoveEnd(".unity"));
            }
        }
    }

    private bool AddToBank(string roomKey, string sceneName)
    {
        if (validSceneNames.Contains(sceneName))
        {
            if (!roomKey.StartsWith("$"))
            {
                roomKey = "$" + roomKey;
            }
            if (debug) Debug.Log($"Adding {roomKey} : {sceneName}");
            bank[roomKey] = sceneName;
            return true;
        }
        else
        {
            if (debug) Debug.Log($"Skipping {roomKey} : {sceneName}");
            return false;
        }
    }

    [ButtonMethod]
    public void TestPrintBank()
    {
        Debug.Log("Test-Printing Room Bank.");
        foreach (var key in bank.Keys)
        {
            Debug.Log($"{key} : {bank[key]}");
        }
    }

    [ButtonMethod]
    public void TestPrintValidSceneNames()
    {
        Debug.Log("Test-Printing Valid Scene Names.");
        foreach (var name in validSceneNames)
        {
            Debug.Log($"{name}");
        }
    }
}

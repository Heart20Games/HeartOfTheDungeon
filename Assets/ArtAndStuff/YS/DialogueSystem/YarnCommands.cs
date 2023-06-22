using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class YarnCommands : BaseMonoBehaviour
{
    private InMemoryVariableStorage storage;

    // Rooms
    [SerializeField] YarnRooms rooms;
    [SerializeField] YarnTags tags;


    // Variable Initialization
    private void Awake()
    {
        if (tags != null)
            tags.Initialize();
        if (rooms != null)
            rooms.Initialize();
    }

    private void Start()
    {
        storage = GetComponent<InMemoryVariableStorage>();;
        Dictionary<string, float> tempFloats = new();
        Dictionary<string, bool> tempBools = new();
        storage.SetAllVariables(tempFloats, rooms.bank, tempBools);
    }

    // Yarn Commands
    [YarnCommand("enter_room")]
    private void EnterRoom(string roomName)
    {
        string sceneName = roomName;
        if (roomName.StartsWith("$"))
        {
            sceneName = rooms.bank[roomName];
        }
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene " + sceneName + " not in build. (Corresponds to room " + roomName + ")");
        }
    }

}

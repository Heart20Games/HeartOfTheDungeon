using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{
    private InMemoryVariableStorage storage;

    // Rooms
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
    public Room[] rooms = new Room[]
    {
        new Room("Hub","HubV2"),
        new Room("Into", "IntroSequence"),
        new Room("SadEmpty", "DungeonExitEmpty"),
        new Room("Outro", "OutroSequence"),
        new Room("OstMech", "OstMechBattle"),
        new Room("RatKing", "RatKingBattle"),
        new Room("RatHub", "RatHub"),
        new Room("MainMenu", "MainMenu"),
        new Room("GameOver", "GameOver"),
    };
    public Dictionary<string, string> roomDict = new Dictionary<string, string>();


    // Variable Initialization
    private void Start()
    {
        storage = GetComponent<InMemoryVariableStorage>();
        foreach (Room room in rooms)
        {
            roomDict["$" + room.name] = room.sceneName;
        }
        Dictionary<string, float> tempFloats = new Dictionary<string, float>();
        Dictionary<string, bool> tempBools = new Dictionary<string, bool>();
        storage.SetAllVariables(tempFloats, roomDict, tempBools);
    }

    // Yarn Commands
    [YarnCommand("enter_room")]
    private void EnterRoom(string roomName)
    {
        string sceneName = roomName;
        if (roomName.StartsWith("$"))
        {
            sceneName = roomDict[roomName];
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

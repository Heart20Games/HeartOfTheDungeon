using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Room[] rooms = new Room[5];
    public Dictionary<string, string> bank = new();

    public void Initialize()
    {
        bank.Clear();
        foreach (Room room in rooms)
        {
            string roomKey = room.name;
            if (!roomKey.StartsWith("$"))
            {
                roomKey = "$" + roomKey;
            }
            bank[roomKey] = room.sceneName;
        }
    }
}

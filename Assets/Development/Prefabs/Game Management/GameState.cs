using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameState", order = 1)]
public class GameState : ScriptableObject
{
    // We can keep track of different player stats in here.
    public int totalDefeated;

    public int currentRoom;
    public int xp;
    public int health;
    public int roomID;

    public List<string> visitedRooms;


    // Can we put functions in here?
    public void TestFunc()
    {
        Debug.Log("Hello world");
    }

}

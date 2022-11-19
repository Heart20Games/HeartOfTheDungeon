using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Yarn.Unity;

public class LevelEngine : MonoBehaviour
{
    [SerializeField] GameState gameState;
    public int targetRoom = 4;

    private List<string> SceneNames = new List<string>
    {
        "Skele00",
        "Skele01",
        "Skele02",
        "Skele03",
        "Skele04"
    };
    


    [YarnCommand("NextRoom")]
    public void NextRoom()
    {
        // print("This is the next room script");
        gameState.currentRoom += 1;
        if(gameState.currentRoom >= targetRoom)
        {
            SceneManager.LoadScene("Hub");
        }
        else
        {
            // We would actually run a cool algorithm for figuring out 
            // what room we wanted to go to next. Or something simple.
            while(true)
            {
                // Get a random number between 0 and 4.
                int rando = Random.Range(0, 4);
                if (gameState.visitedRooms.Contains(SceneNames[rando]))
                {
                    Debug.LogWarning("Already been to that room, fetching another");
                    continue;
                }
                else
                {
                    SceneManager.LoadScene(SceneNames[rando]);
                    break;
                }
            }
            
            //SceneManager.LoadScene("Gameplay");
        }
    }

    [YarnCommand("StartRun")]
    public void StartRun()
    {
        gameState.currentRoom = 0;
        // Need to set health and the like back to whatever "full" is.  Maybe 
        // add a max health value to ScriptableObject GameState?
        SceneManager.LoadScene("Gameplay");
    }
}

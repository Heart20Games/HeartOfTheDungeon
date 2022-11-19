using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Yarn.Unity;

public class LevelEngine : MonoBehaviour
{
    [SerializeField] GameState gameState;
    public int targetRoom = 4;


    [YarnCommand("NextRoom")]
    public void NextRoom()
    {
        print("This is the next room script");
        gameState.currentRoom += 1;
        if(gameState.currentRoom >= targetRoom)
        {
            SceneManager.LoadScene("Hub");
        }
        else
        {
            // We would actually run a cool algorithm for figuring out 
            // what room we wanted to go to next. Or something simple.
            SceneManager.LoadScene("Gameplay");
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

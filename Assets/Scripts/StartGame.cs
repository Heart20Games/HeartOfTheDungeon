using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class StartGame : MonoBehaviour
{
    public InMemoryVariableStorage storage;
    [SerializeField] public string targetRoom = "$Hub";

    public void StartGameplay()
    {
        print("Game starts here!");
        if (storage != null)
        {
            string targetScene = targetRoom;
            storage.TryGetValue<string>(targetRoom, out targetScene);
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Cannot find InMemoryVariableStorage componenent.");
            SceneManager.LoadScene(targetRoom);
        }
    }
}

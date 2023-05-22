using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class LoadRoom : BaseMonoBehaviour
{
    public InMemoryVariableStorage storage;
    [SerializeField] public string targetRoom = "$Hub";

    public void StartGameplay()
    {
        print("Game starts here!");
        if (targetRoom.StartsWith("$") && storage != null)
        {
            string targetScene;
            if (storage.TryGetValue<string>(targetRoom, out targetScene))
            {
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning("Can't find room " + targetRoom + ", using as Scene name instead.");
                SceneManager.LoadScene(targetRoom);
            }
        }
        else
        {
            if (storage == null)
            {
                Debug.LogWarning("Cannot find InMemoryVariableStorage componenent.");
            }
            SceneManager.LoadScene(targetRoom);
        }
    }
}

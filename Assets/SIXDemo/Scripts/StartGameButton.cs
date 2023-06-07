using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    string targetScene = "Put Scene name here please.";

    public void StartGame()
    {
        Debug.Log("Starting game");
        SceneManager.LoadScene(targetScene);
    }
}

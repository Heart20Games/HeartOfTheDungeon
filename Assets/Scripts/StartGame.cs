using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private string targetScene;
    public void StartGameplay()
    {
        
        print("Game starts here!");
        SceneManager.LoadScene(targetScene);
    }
}

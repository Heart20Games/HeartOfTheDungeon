using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCore : MonoBehaviour
{
    // For keeping track of things like health and other instance specific things.
    // Stat block here


    // Public methods here
    public void Die()
    {
        Debug.Log("You are Dead");
        SceneManager.LoadScene("GameOver");// Whisks us directly to the game over screen.

    }
}

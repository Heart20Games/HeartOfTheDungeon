using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShortcutInput : MonoBehaviour
{
    private Game game;
    public Game Game { get { game = game != null ? game : Game.game; return game; } }

    // Cheats / Shortcuts
    [Header("Shortcuts")]
    [ReadOnly][SerializeField] private bool restartLevel = false;
    [ReadOnly][SerializeField] private bool restartGame = false;
    public void OnRestartLevel(InputValue inputValue)
    {
        if (inputValue.isPressed)
            restartLevel = true;
    }
    public void OnRestartGame(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            restartGame = true;
            Game.RestartGame();
        }
    }
    public void OnTriggerRestart(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            if (restartLevel && !restartGame)
                Game.RestartScene();
            restartLevel = false;
            restartGame = false;
        }
    }
}
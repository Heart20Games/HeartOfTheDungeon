using UnityEngine;
using UnityEngine.InputSystem;

namespace HotD
{
    public class ShortcutInput : MonoBehaviour
    {
        private Game game;
        public Game Game { get { game = game != null ? game : Game.main; return game; } }

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
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotD
{
    public class ShortcutInput : BaseMonoBehaviour
    {
        private Game game;
        public Game Game { get { game = game != null ? game : Game.main; return game; } }

        private void OnEnable() {}

        // Cheats / Shortcuts
        [Header("Shortcuts")]
        [ReadOnly][SerializeField] private bool restartLevel = false;
        [ReadOnly][SerializeField] private bool restartGame = false;
        public void OnRestartLevel(InputValue inputValue)
        {
            if (!isActiveAndEnabled) return;
            
            if (inputValue.isPressed)
                restartLevel = true;
        }
        public void OnRestartGame(InputValue inputValue)
        {
            if (!isActiveAndEnabled) return;

            if (inputValue.isPressed)
            {
                restartGame = true;
                Game.RestartGame();
            }
        }
        public void OnTriggerRestart(InputValue inputValue)
        {
            if (!isActiveAndEnabled) return;
         
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
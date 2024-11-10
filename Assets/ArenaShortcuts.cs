using Attributes;
using HotD.Body;
using HotD.UI;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

// Note that any public vs private access in this file is purely for expediency and needs to be cleaned up later w/ SerializeField attributes.

namespace HotD.Cheats
{
    public class ArenaShortcuts : BaseMonoBehaviour
    {
        static bool jumpToCombat = false;

        public SceneTimeline timeline;
        [Space]
        public BossHealthMeterController bossHUD;
        [Space]
        public Character mainCharacter;
        public Respawn explodingBarrel;
        public Character bossEnemy;
        [Space]
        [ReadOnly] public bool restartHeld;
        [ReadOnly] public bool cheatHeld;

        [SerializeField] private bool jumpToCombatOnAwake;

        private BaseAttribute IntScore { get => mainCharacter.StatBlock.intelligence; }

        private void Awake()
        {
            if (jumpToCombat || jumpToCombatOnAwake) StartCoroutine(JumpToCombat());
        }

        private bool Pressed(InputValue inputValue)
        {
            return inputValue.isPressed;
        }

        public void OnRestart(InputValue iv)
        {
            restartHeld = iv.isPressed;
        }

        public void OnRestartAtCutscene(InputValue iv)
        {
            if (!Pressed(iv) || !restartHeld) return;

            SceneManager.LoadScene("Arena");
        }

        public void OnRestartAtCombat(InputValue iv)
        {
            if (!Pressed(iv) || !restartHeld) return;

            ArenaShortcuts.jumpToCombat = true;
            SceneManager.LoadScene("Arena");
        }

        // Should be called on Awake when the static jumpToCombat bool is set to true.
        private IEnumerator JumpToCombat()
        {
            // Skip the Timeline
            if (SceneTimeline.main == null)
                timeline.GetComponent<PlayableDirector>().playOnAwake = false;
            else
                timeline.Shortcut(100);

            mainCharacter.AimCamera.gameObject.SetActive(false);
            yield return null;
            mainCharacter.AimCamera.gameObject.SetActive(true);

            bossHUD.pipRevealSpeed = 0.00001f;
            bossHUD.pipInitialRevealTime = 0.1f;
            UserInterface.main.CanDismissBossHUD = false;
            Game.main.ShowBossHud();
            Print("Showing the Boss Hud");
            bossHUD.bufferBannerMove = true;
            bossHUD.onBannerMove = FinishJumpToCombat;
            bossHUD.TriggerBannerMove(false);
        }
        private void FinishJumpToCombat()
        {
            Print("Finishing Jump to Combat");
            Game.main.StartGame();
            //Game.main.SetMode(GameModes.InputMode.Character);
            //mainCharacter.Respawn();
            //explodingBarrel.RespawnTrigger();
            //bossEnemy.gameObject.SetActive(true);
            //bossEnemy.Respawn();
        }

        public void OnRestartFight(InputValue iv)
        {
            if (!Pressed(iv) || !restartHeld) return;

            mainCharacter.Respawn();
            explodingBarrel.RespawnTrigger();
            bossEnemy.Respawn();
        }

        public void OnRestartApplication(InputValue iv)
        {
            if (!Pressed(iv) || !restartHeld) return;

            Process.Start(Application.dataPath.Replace("_Data", ".exe")); // Run a copy of the program.
            Application.Quit(); // Kill the current process.
        }

        public void OnCheat(InputValue iv)
        {
            cheatHeld = iv.isPressed;
        }

        public void OnIncreasePower(InputValue iv)
        {
            if (!Pressed(iv) || !cheatHeld) return;

            Vector2 range = IntScore.BaseValueRange;
            int newValue = IntScore.BaseValue + 1;
            if (range.y < newValue)
            {
                IntScore.BaseValueRange = new(range.x, newValue);
                IntScore.BaseValue = newValue;
            }
        }

        public void OnDecreasePower(InputValue iv)
        {
            if (!Pressed(iv) || !cheatHeld) return;

            IntScore.BaseValue -= 1;
        }

        public void OnSpawnSlimes(InputValue iv)
        {
            if (!Pressed(iv) || !cheatHeld) return;
        }
    }
}
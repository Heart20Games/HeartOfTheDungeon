using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Yarn.Unity;
using Body;
using UnityEngine.EventSystems;
using HotD.Body;

namespace HotD.UI
{
    public class UserInterface : BaseMonoBehaviour
    {
        public static UserInterface main;
        private void OnDestroy()
        {
            if (UserInterface.main == this) UserInterface.main = null;
        }

        public Character controlledCharacter;
        public DialogueRunner dialogueRunner;
        public HUD hud;
        public BossHealthMeterController bossHealthMeter;
        public GameObject controlScreen;
        public CharacterSheet characterSheet;
        public GameObject simpleDialogue;
        public EventSystem menuInputSystem;
        public Transform deathScreen;

        bool canDismissBossHUD = true;
        public bool CanDismissBossHUD { get => canDismissBossHUD; set => canDismissBossHUD = value; }

        private readonly List<GameObject> panels = new();

        [SerializeField] private bool debugContinue = false;
        public UnityEvent onContinue;

        private void Awake()
        {
            UserInterface.main = this;
            panels.Add(dialogueRunner.gameObject);
            panels.Add(hud.gameObject);
            panels.Add(controlScreen);
            Portraits.main.Initialize();
        }

        public void Start()
        {
            SetDialogueActive(false);
            SetHudActive(true);
            SetControlScreenActive(false);
            SetCharacterSheetActive(false);
        }

        // Setters

        public void SetDialogueActive(bool active)
        {
            dialogueRunner.gameObject.SetActive(active);
        }

        public void SetHudActive(bool active)
        {
            hud.gameObject.SetActive(active);
        }

        public void SetBossHudActive(bool active)
        {
            if (canDismissBossHUD || active)
                bossHealthMeter.gameObject.SetActive(active);
        }

        public void SetControlScreenActive(bool active)
        {
            controlScreen.SetActive(active);
        }

        public void SetCharacterSheetActive(bool active)
        {
            characterSheet.gameObject.SetActive(active);
        }

        public void SetSimpleDialogueActive(bool active)
        {
            simpleDialogue.SetActive(active);
        }

        public void SetMenuInputsActive(bool active)
        {
            menuInputSystem.gameObject.SetActive(active);
        }

        public void SetDeathScreenActive(bool active)
        {
            deathScreen.gameObject.SetActive(active);
        }

        // Set Character

        public void SetCharacter(Character character)
        {
            controlledCharacter = character;
            characterSheet.SetCharacter(character.StatBlock);
        }

        public void UpdateWeapon()
        {
        }

        // Continue

        public void Continue()
        {
            Print("Continue called on UserInterface!", debugContinue, this);
            onContinue.Invoke();
        }

        public void Select()
        {

        }
    }
}
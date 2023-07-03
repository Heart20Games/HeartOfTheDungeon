using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class Game : BaseMonoBehaviour
{
    [Serializable]
    public struct GameSettings
    {
        public bool useD20Menu;
    }

    // Properties
    public Character playerCharacter;
    public List<Character> playableCharacters;
    public Selector selector;
    public string characterInputMap = "GroundMovement";
    public string selectorInputMap = "Selector";
    public string dialogueInputMap = "Dialogue";
    public string dismissInputMap = "Dismiss";
    public GameSettings settings = new();
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public HUD hud;
    [HideInInspector] public List<ITimeScalable> timeScalables;
    [HideInInspector] public List<Interactable> interactables;

    // Current Character
    private Character curCharacter;
    [HideInInspector] public Character CurCharacter { get { return curCharacter; } set { SetCharacter(value); } }
    private int curCharIdx = 0;
    
    // TimeScale
    public float timeScale = 1.0f;
    public float TimeScale { get { return timeScale; } set { SetTimeScale(value); } }
    
    // Game Mode
    public enum GameMode { Selection, Character, Dialogue, Dismiss };
    private GameMode mode = GameMode.Character;
    public GameMode Mode { get { return mode; } set { SetMode(value); } }

    // Input
    private PlayerInput input;

    // Cheats / Shortcuts
    public bool restartable = true;

    // Events
    public UnityEvent onPlayerDied;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Initialization

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        InitializePlayableCharacters();
    }

    public void InitializePlayableCharacters()
    {
        bool hasPlayer = false;
        if (playerCharacter != null)
        {
            hud.MainCharacterSelect(playerCharacter);
            if (!hasPlayer)
            {
                playableCharacters.Insert(0, playerCharacter);
            }
        }
        SetCharacterIdx(0);
        if (curCharacter == null)
        {
            SetMode(GameMode.Selection);
        }
    }


    // Setters

    // Time Scale
    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        foreach(ITimeScalable timeScalable in timeScalables)
        {
            if (timeScalable != null)
            {
                timeScalable.SetTimeScale(this.timeScale);
            }
            else
            {
                print("Invalid Object");
            }
        }
    }

    // Mode
    public void SetMode(GameMode mode)
    {
        switch (this.mode)
        {
            case GameMode.Character:
                userInterface.SetHudActive(false); break;
                //SetControllable(curCharacter, false); break;
            case GameMode.Selection:
                userInterface.SetHudActive(false); break;
                //SetControllable(selector, false); break;
            case GameMode.Dialogue:
                userInterface.SetDialogueActive(false); break;
            case GameMode.Dismiss:
                userInterface.SetControlScreenActive(false); break;
        }
        switch (mode)
        {
            case GameMode.Character:
                userInterface.SetHudActive(true);
                input.SwitchCurrentActionMap(characterInputMap);
                TimeScale = 1; break;
                SetControllable(curCharacter, true); break;
            case GameMode.Selection:
                userInterface.SetHudActive(true);
                input.SwitchCurrentActionMap(selectorInputMap);
                if (selector != null && curCharacter != null)
                {
                    selector.transform.position = curCharacter.body.position;
                }
                TimeScale = 0.1f;
                SetControllable(selector, true); break;
            case GameMode.Dialogue:
                userInterface.SetDialogueActive(true);
                TimeScale = 0f;
                input.SwitchCurrentActionMap(dialogueInputMap); break;
            case GameMode.Dismiss:
                userInterface.SetControlScreenActive(true);
                TimeScale = 0f;
                input.SwitchCurrentActionMap(dismissInputMap); break;
        }
        this.mode = mode;
    }

    public void SetControllable(IControllable controllable, bool shouldControl)
    {
        controllable?.SetControllable(shouldControl);
    }

    // Set Characters

    public void SetCharacterIdx(int idx)
    {
        if (playableCharacters.Count > 0)
        {
            idx = idx < 0 ? playableCharacters.Count + idx : idx;
            curCharIdx = idx % (playableCharacters.Count);
            Character character = playableCharacters[curCharIdx];
            SetCharacter(character);
            hud.CharacterSelect(character);//curCharIdx);
        }
    }

    public void SetCharacter(Character character)
    {
        if (character != null)
        {
            SetControllable(curCharacter, false);
            curCharacter = character;
            SetControllable(curCharacter, true);
            SetMode(GameMode.Character);
        }
    }
    
    public void SwitchToCompanion(InputValue inputValue, int idx)
    {
        if (curCharIdx == idx)
            SetCharacterInput(inputValue, 0);
        else
            SetCharacterInput(inputValue, idx);
    }


    // Checks
    public bool CanUseCharacter() { return curCharacter != null && Mode == GameMode.Character; }
    public bool CanUseSelector() { return selector != null && Mode == GameMode.Selection; }



    // Events

    public void OnCharacterDied(Character character)
    {
        if (character == playerCharacter)
        {
            onPlayerDied.Invoke();
        }
        else if (character == curCharacter)
        {
            SetCharacter(playerCharacter);
        }
    }


    // Actions

    // Cheats / Shortcuts

    public void OnRestartLevel(InputValue inputValue)
    {
        if (inputValue.isPressed && restartable)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Movement

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        switch (Mode)
        {
            case GameMode.Character:
                if (CanUseCharacter())
                {
                    curCharacter.MoveCharacter(inputVector);
                } break;
            case GameMode.Selection:
                if (CanUseSelector())
                {
                    print("Pass move vector to selector");
                    selector.MoveVector = inputVector;
                } break;
        }
    }

    // Aiming

    public void OnAim(InputValue inputValue) { curCharacter.AimCharacter(inputValue.Get<Vector2>()); }
    public void OnToggleAiming(InputValue inputValue) { curCharacter.SetAimModeActive(inputValue.isPressed); }

    // Castables

    public enum CastableIdx { Ability1, Ability2, Weapon1, Weapon2, Agility }
    public void UseCastable(InputValue inputValue, CastableIdx idx)
    {
        if (inputValue.isPressed)
            curCharacter.ActivateCastable((int)idx);
    }
    public void OnUseAgility(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Agility); }
    public void OnUseWeapon1(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Weapon1); }
    public void OnUseWeapon2(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Weapon2); }
    public void OnUseAbility1(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Ability1); }
    public void OnUseAbility2(InputValue inputValue) { UseCastable(inputValue, CastableIdx.Ability2); }

    // Character Switching

    public void SetCharacterInput(InputValue inputValue, int idx)
    {
        if (inputValue.isPressed)
        {
            SetCharacterIdx(idx);
        }
    }
    public void OnSwitchCharacterLeft(InputValue inputValue) { SwitchToCompanion(inputValue, 2); }
    public void OnSwitchCharacterRight(InputValue inputValue) { SwitchToCompanion(inputValue, 1); }
    public void OnSwitchCharacterCenter(InputValue inputValue) { SetCharacterInput(inputValue, 0); }
    public void OnCycleCharacterLeft(InputValue inputValue) { SetCharacterInput(inputValue, curCharIdx - 1); }
    public void OnCycleCharacterRight(InputValue inputValue) { SetCharacterInput(inputValue, curCharIdx + 1); }

    // Interaction

    public void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            foreach (Interactable interactable in interactables)
            {
                interactable.Interact();
            }
        }
    }

    // Selector

    public void OnSelect(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (CanUseSelector())
            {
                selector.Select();
            }
        }
    }

    public void OnDeSelect(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (CanUseSelector())
            {
                selector.DeSelect();
            }
        }
    }

    // Skill Wheel

    public void OnToggleSkillWheel(InputValue inputValue)
    {
        if (settings.useD20Menu && inputValue.isPressed)
        {
            Mode = mode == GameMode.Character ? GameMode.Selection : GameMode.Character;
            hud.abilityMenu.Toggle();
        }
    }

    // Control Screen

    public void OnToggleControls(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Mode = GameMode.Dismiss;
        }
    }

    public void OnDismiss(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Mode = GameMode.Character;
        }
    }

    // Dialogue

    public void OnContinue(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            userInterface.Continue();
        }
    }
}

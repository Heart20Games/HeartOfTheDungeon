using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Body;
using UnityEngine.SceneManagement;

public class Game : BaseMonoBehaviour
{
    // Properties
    public Character playerCharacter;
    public List<Character> playableCharacters;
    public Selector selector;
    public string characterInputMap = "GroundMovement";
    public string selectorInputMap = "Selector";
    public string dialogueInputMap = "Dialogue";
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
    public enum GameMode { Selection, Character, Dialogue };
    private GameMode mode = GameMode.Character;
    public GameMode Mode { get { return mode; } set { SetMode(value); } }

    // Input
    private PlayerInput input;

    // Cheats / Shortcuts
    public bool restartable = true;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        InitializePlayableCharacters();
    }

    public void InitializePlayableCharacters()
    {
        bool hasPlayer = false;
        foreach (Character character in playableCharacters)
        {
            if (character.GetComponent<PlayerCore>() != null)
            {
                hasPlayer = true;
                break;
            }
        }
        if (!hasPlayer && playerCharacter != null)
        {
            playableCharacters.Insert(0, playerCharacter);
        }
        SetCharacterIdx(0);
        if (curCharacter == null)
        {
            SetMode(GameMode.Selection);
        }
    }

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

    public void SetMode(GameMode mode)
    {
        switch (this.mode)
        {
            case GameMode.Character:
                SetControllable(curCharacter, false); break;
            case GameMode.Selection:
                SetControllable(selector, false); break;
            case GameMode.Dialogue:
                break;
        }
        switch (mode)
        {
            case GameMode.Character:
                input.SwitchCurrentActionMap(characterInputMap);
                TimeScale = 1;
                SetControllable(curCharacter, true); break;
            case GameMode.Selection:
                input.SwitchCurrentActionMap(selectorInputMap);
                if (selector != null && curCharacter != null)
                {
                    selector.transform.position = curCharacter.transform.position;
                }
                TimeScale = 0.1f;
                SetControllable(selector, true); break;
            case GameMode.Dialogue:
                input.SwitchCurrentActionMap(dialogueInputMap); break;
        }
        this.mode = mode;
    }

    public void SetControllable(IControllable controllable, bool shouldControl)
    {
        controllable?.SetControllable(shouldControl);
    }

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

    public bool CanUseCharacter()
    {
        return curCharacter != null && Mode == GameMode.Character;
    }

    public bool CanUseSelector()
    {
        return selector != null && Mode == GameMode.Selection;
    }


    // Cheats / Shortcuts

    public void OnRestartLevel(InputValue inputValue)
    {
        if (inputValue.isPressed && restartable)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    // Actions

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
                    selector.MoveVector = inputVector;
                } break;
        }
    }

    //public void OnLook(InputValue inputValue)
    //{
    //    Vector2 inputVector = inputValue.Get<Vector2>();
    //}

    public void OnAim(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        curCharacter.AimCharacter(inputVector);
    }

    public void OnToggleAiming(InputValue inputValue)
    {
        curCharacter.SetAimModeActive(inputValue.isPressed);
    }

    public void SetCharacterInput(InputValue inputValue, int idx)
    {
        if (inputValue.isPressed)
        {
            SetCharacterIdx(idx);
        }
    }

    public void OnSwitchCharacterLeft(InputValue inputValue)
    {
        SetCharacterInput(inputValue, 2);
    }

    public void OnSwitchCharacterRight(InputValue inputValue)
    {
        SetCharacterInput(inputValue, 1);
    }

    public void OnSwitchCharacterCenter(InputValue inputValue)
    {
        SetCharacterInput(inputValue, 0);
    }

    public void OnCycleCharacterLeft(InputValue inputValue)
    {
        SetCharacterInput(inputValue, curCharIdx - 1);
    }

    public void OnCycleCharacterRight(InputValue inputValue)
    {
        SetCharacterInput(inputValue, curCharIdx + 1);
    }

    public void OnSwitchSecondary(InputValue inputValue)
    {
        if (inputValue.isPressed && CanUseCharacter())
        {
            curCharacter.ChangeAbility();
        }
    }

    public void OnSwitchPrimary(InputValue inputValue)
    {
        if (inputValue.isPressed && CanUseCharacter())
        {
            curCharacter.ChangeWeapon();
        }
    }

    public void OnCastPrimary(InputValue inputValue)
    {
        if (inputValue.isPressed && CanUseCharacter())
        {
            curCharacter.ActivateWeapon();
        }
    }

    public void OnCastSecondary(InputValue inputValue)
    {
        if (inputValue.isPressed && CanUseCharacter())
        {
            curCharacter.ActivateAbility();
        }
    }

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

    public void OnToggleSkillWheel(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Mode = mode == GameMode.Character ? GameMode.Selection : GameMode.Character;
            hud.AbilityToggle();
        }
    }
}

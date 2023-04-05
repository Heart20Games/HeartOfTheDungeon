using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public Character playerCharacter;
    public List<Character> playableCharacters;
    public Selector selector;
    public string characterInputMap = "GroundMovement";
    public string selectorInputMap = "Selector";
    [HideInInspector] public Character curCharacter;
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public HUD hud;
    private int curCharIdx = 0;
    
    public enum GameMode { Selection, Character };
    private GameMode mode = GameMode.Character;
    public GameMode Mode { get { return mode; } set { SetMode(value); } }

    private PlayerInput input;

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
        SetCharacter(0);
    }

    public void SetMode(GameMode mode)
    {
        switch (this.mode)
        {
            case GameMode.Character:
                curCharacter.SetControllable(false); break;
            case GameMode.Selection:
                selector.SetControllable(false); break;
        }
        switch (mode)
        {
            case GameMode.Character:
                input.SwitchCurrentActionMap(characterInputMap);
                curCharacter.SetControllable(true); break;
            case GameMode.Selection:
                input.SwitchCurrentActionMap(selectorInputMap);
                selector.transform.position = curCharacter.transform.position;
                selector.SetControllable(true); break;
        }
        this.mode = mode;
    }

    public void SetCharacter(int idx)
    {
        idx = idx < 0 ? playableCharacters.Count + idx : idx;
        curCharIdx = idx % (playableCharacters.Count);
        curCharacter.SetControllable(false);
        curCharacter = playableCharacters[curCharIdx];
        curCharacter.SetControllable(true);
        hud.CharacterSelect(curCharIdx);
    }


    // Actions

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        curCharacter.MoveCharacter(inputVector);
        if (selector.controllable)
        {
            selector.MoveVector = inputVector;
        }
    }

    public void OnAim(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        curCharacter.AimCharacter(inputVector);
    }

    public void SetCharacterInput(InputValue inputValue, int idx)
    {
        if (inputValue.isPressed)
        {
            SetCharacter(idx);
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
        if (inputValue.isPressed)
        {
            curCharacter.ChangeAbility();
        }
    }

    public void OnSwitchPrimary(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            curCharacter.ChangeWeapon();
        }
    }

    public void OnCastPrimary(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            curCharacter.ActivateWeapon();
        }
    }

    public void OnCastSecondary(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            curCharacter.ActivateAbility();
        }
    }

    public void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            print("Interact");
            curCharacter.Interact();
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

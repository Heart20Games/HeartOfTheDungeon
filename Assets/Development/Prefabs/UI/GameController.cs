using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public Character playerCharacter;
    public List<Character> playableCharacters;
    public SelectionCursor selectionCursor;
    [HideInInspector] public Character curCharacter;
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public HUD hud;
    private int curCharIdx = 0;

    private void Start()
    {
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

    public void EnterSelectionMode()
    {
        curCharacter.SetControllable(false);
        selectionCursor.SetControllable(true);
    }

    public void ExitSelectionMode()
    {
        selectionCursor.SetControllable(false);
        curCharacter.SetControllable(true);
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

    public void OnOpenSkillWheel(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (selectionCursor.controllable)
            {
                ExitSelectionMode();
            }
            else
            {
                EnterSelectionMode();
            }
            hud.AbilityToggle();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public Character playerCharacter;
    public List<Character> playableCharacters;
    [HideInInspector] public Character curCharacter;
    [HideInInspector] public UserInterface userInterface;
    [HideInInspector] public HUD hud;

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

    public void SetCharacter(int idx)
    {
        print("Set Character: " + idx + " / " + playableCharacters.Count + " -> " + idx % (playableCharacters.Count));
        if (curCharacter != null)
        {
            print(curCharacter.name);
        }
        curCharacter.SetControllable(false);
        curCharacter = playableCharacters[idx % (playableCharacters.Count)];
        curCharacter.SetControllable(true);
        print(curCharacter.name);
        userInterface.SetCharacter(curCharacter);
        hud.CharacterSelect(idx);
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
            curCharacter.Interact();
        }
    }

    public void OnOpenSkillWheel(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            hud.AbilityToggle();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public List<Character> playableCharacters;
    public Character curCharacter;
    public UserInterface userInterface;

    private void Start()
    {
        SetCharacter(0);
    }

    public void SetCharacter(int idx)
    {
        curCharacter = playableCharacters[idx % (playableCharacters.Count)];
        userInterface.SetCharacter(curCharacter);
    }


    // Actions

    public void OnMovement(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        curCharacter.MoveCharacter(inputVector);
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
}

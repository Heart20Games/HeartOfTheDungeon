using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UserInterface : MonoBehaviour
{
    public Character controlledCharacter;

    public UnityEvent onContinue;

    public void SetCharacter(Character character)
    {
        controlledCharacter = character;
    }

    public void UpdateWeapon()
    {

    }

    public void OnContinue(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            onContinue.Invoke();
        }
    }
}

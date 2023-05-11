using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Yarn.Unity;
using Body;

public class UserInterface : MonoBehaviour
{
    public Body.Character controlledCharacter;
    public DialogueRunner dialogueRunner;

    public UnityEvent onContinue;

    public void SetCharacter(Body.Character character)
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

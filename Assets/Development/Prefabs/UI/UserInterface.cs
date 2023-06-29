using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Yarn.Unity;
using Body;

public class UserInterface : BaseMonoBehaviour
{
    public Body.Character controlledCharacter;
    public DialogueRunner dialogueRunner;
    public Canvas dialogueCanvas;

    public UnityEvent onContinue;

    public void Start()
    {
        dialogueCanvas.gameObject.SetActive(false);
    }

    public void SetDialogueActive(bool active)
    {
        dialogueCanvas.gameObject.SetActive(active);
    }

    public void SetCharacter(Body.Character character)
    {
        controlledCharacter = character;
    }

    public void UpdateWeapon()
    {

    }

    public void Continue()
    {
        onContinue.Invoke();
    }
}

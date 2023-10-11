using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Yarn.Unity;
using Body;
using UnityEngine.EventSystems;

public class UserInterface : BaseMonoBehaviour
{
    public Character controlledCharacter;
    public DialogueRunner dialogueRunner;
    public HUD hud;
    public GameObject controlScreen;
    public CharacterSheet characterSheet;
    public GameObject simpleDialogue;
    public EventSystem menuInputSystem;

    private readonly List<GameObject> panels = new();

    public UnityEvent onContinue;

    private void Awake()
    {
        panels.Add(dialogueRunner.gameObject);
        panels.Add(hud.gameObject);
        panels.Add(controlScreen);
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

    // Set Character

    public void SetCharacter(Character character)
    {
        controlledCharacter = character;
        characterSheet.SetCharacter(character.statBlock);
    }

    public void UpdateWeapon()
    {
    }

    // Continue

    public void Continue()
    {
        onContinue.Invoke();
    }

    public void Select()
    {

    }
}

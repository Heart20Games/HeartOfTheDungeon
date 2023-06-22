using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game;
using static ISelectable;
using Body;

public class GameController : BaseMonoBehaviour
{
    public GameMode mode = GameMode.Character;
    public List<SelectType> selectableTypes;
    [HideInInspector] public Game controller;
    [HideInInspector] public Selector selector;
    public UnityEvent onSelectorConfirmed;

    private readonly Stack<Character> characterStack = new();

    private void Awake()
    {
        controller = FindObjectOfType<Game>();
        selector = controller.selector;
    }

    public void CharacterDied(Character character)
    {
        controller.OnCharacterDied(character);
    }

    public void OnSelectorConfirmed()
    {
        print("Selector Confirmed (" + (selector.selected != null) + ")");
        selector.onConfirm.RemoveListener(OnSelectorConfirmed);
        onSelectorConfirmed.Invoke();
    }

    public void UseSelector()
    {
        selector.onConfirm.AddListener(OnSelectorConfirmed);
        selector.SelectableTypes = selectableTypes;
        SetMode(GameMode.Selection);
    }

    public void UseCharacter()
    {
        SetMode(GameMode.Character);
    }

    public void UseDialoge()
    {
        SetMode(GameMode.Dialogue);
    }

    public void SetMode(GameMode mode)
    {
        controller.Mode = mode;
    }

    public void PushCharacter()
    {
        if (controller.selector.selected != null)
        {
            Character character = selector.selected.source.GetComponent<Character>();
            if (character != null)
            {
                characterStack.Push(controller.CurCharacter);
                controller.SetCharacter(character);
            }
            else
            {
                Debug.LogWarning("Selectable Not A Character. (" + selector.selected + ")");
            }
        }
        else
        {
            Debug.LogWarning("Nothing Selected.");
        }
    }

    public void PopCharacter()
    {
        if (characterStack.Count > 0)
        {
            controller.SetCharacter(characterStack.Pop());
        }
    }

    public void SetTimeScale(float timeScale)
    {
        controller.TimeScale = timeScale;
    }
}

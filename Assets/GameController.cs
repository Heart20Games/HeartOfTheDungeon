using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game;
using static Selectable;

public class GameController : MonoBehaviour
{
    public GameMode mode = GameMode.Character;
    public List<SelectType> selectableTypes;
    [HideInInspector] public Game controller;
    public UnityEvent onSelectorConfirmed;

    private Stack<Character> characterStack = new Stack<Character>();

    private void Awake()
    {
        controller = FindObjectOfType<Game>();
        controller.selector.onConfirm.AddListener(OnSelectorConfirmed);
    }

    public void OnSelectorConfirmed()
    {
        onSelectorConfirmed.Invoke();
    }

    public void UseSelector()
    {
        SetMode(GameMode.Selection);
    }

    public void UseCharacter()
    {
        SetMode(GameMode.Character);
    }

    public void SetMode(GameMode mode)
    {
        controller.Mode = mode;
    }

    public void PushCharacter()
    {
        Character character = controller.selector.selected.GetComponent<Character>();
        if (character != null )
        {
            characterStack.Push(controller.CurCharacter);
            controller.SetCharacter(character);
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

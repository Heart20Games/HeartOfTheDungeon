using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game;
using static ISelectable;
using Body;
using static GameModes;

public class GameController : BaseMonoBehaviour
{
    public GameMode mode = GameMode.Character;
    public List<SelectType> selectableTypes;
    [HideInInspector] public Game game;
    public UnityEvent onSelectorConfirmed;

    public Selector Selector { get => game.curSelector; }

    private readonly Stack<Character> characterStack = new();

    private void Awake()
    {
        game = FindObjectOfType<Game>();
    }

    public void CharacterDied(Character character)
    {
        game.OnCharacterDied(character);
    }

    public void OnSelectorConfirmed()
    {
        print("Selector Confirmed (" + (Selector.selected != null) + ")");
        Selector.onConfirm.RemoveListener(OnSelectorConfirmed);
        onSelectorConfirmed.Invoke();
    }

    public void UseSelector()
    {
        Selector.onConfirm.AddListener(OnSelectorConfirmed);
        Selector.SelectableTypes = selectableTypes;
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
        game.Mode = mode;
    }

    public void PushCharacter()
    {
        if (Selector.selected != null)
        {
            Character character = Selector.selected.source.GetComponent<Character>();
            if (character != null)
            {
                characterStack.Push(game.CurCharacter);
                game.SetCharacter(character);
            }
            else
            {
                Debug.LogWarning("Selectable Not A Character. (" + Selector.selected + ")");
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
            game.SetCharacter(characterStack.Pop());
        }
    }

    public void SetTimeScale(float timeScale)
    {
        game.TimeScale = timeScale;
    }
}

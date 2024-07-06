using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ISelectable;
using Body;

namespace HotD
{
    using static GameModes;

    public class GameController : BaseMonoBehaviour
    {
        public InputMode mode = InputMode.Character;
        public List<SelectType> selectableTypes;
        [HideInInspector] public Game game;
        public UnityEvent onSelectorConfirmed;
        public UnityEvent onDataInitialized;

        public Selector Selector { get => game.curSelector; }

        private readonly Stack<Character> characterStack = new();

        private void Awake()
        {
            game = FindObjectOfType<Game>();
        }

        //public void CharacterDied(Character character)
        //{
        //    game.OnCharacterDied(character);
        //}

        public void OnSelectorConfirmed()
        {
            print("Selector Confirmed (" + (Selector.selected != null) + ")");
            Selector.onConfirm.trigger.enter.RemoveListener(OnSelectorConfirmed);
            onSelectorConfirmed.Invoke();
        }

        public void OnDataInitialized()
        {
            onDataInitialized.Invoke();
        }

        public void UseSelector()
        {
            Selector.onConfirm.trigger.enter.AddListener(OnSelectorConfirmed);
            Selector.SelectableTypes = selectableTypes;
            SetMode(InputMode.Selection);
        }

        public void UseCharacter()
        {
            SetMode(InputMode.Character);
        }

        public void UseDialoge()
        {
            SetMode(InputMode.Dialogue);
        }

        public void SetMode(InputMode mode)
        {
            game.InputMode = mode;
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
            TimeScaler.TimeScale = timeScale;
        }
    }
}
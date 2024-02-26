using Selection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotD
{
    using static Game;
    using static GameModes;
    
    public class GameModes
    {
        public enum InputMode { None, Selection, Character, Dialogue, LockOn, Menu, Cutscene };
        public enum MoveMode { None, Selector, Character }
        public enum LookMode { None, Targeter }
        public enum Menu { None, ControlSheet, CharacterSheet, Dialogue, Death }
        public enum Shader { None, Death }
        public static Dictionary<string, GameMode> ModeBank { get { return game.settings.ModeBank; } }
        public static Dictionary<InputMode, GameMode> InputBank { get { return game.settings.InputBank; } }
        public static Dictionary<Menu, GameMode> MenuBank { get { return game.settings.MenuBank; } }

        [Serializable]
        public struct GameMode
        {
            public string name;
            public InputMode inputMode;
            public MoveMode moveMode;
            public LookMode lookMode;
            public Menu activeMenu;
            public Shader activeShader;
            public bool showMouse;
            public bool hudActive;
            public bool dialogueActive;
            public bool targetLock;
            public float timeScale;
            public bool shouldBrain;
            public bool cardboardMode;
            public readonly IControllable Controllable
            {
                get
                {
                    return moveMode switch
                    {
                        MoveMode.Character => game.CurCharacter,
                        MoveMode.Selector => game.curController,
                        _ => null,
                    };
                }
            }
            public readonly ILooker Looker
            {
                get
                {
                    return lookMode switch
                    {
                        LookMode.Targeter => game.targeter,
                        _ => null,
                    };
                }
            }
            public readonly TargetFinder Finder { get => game.CurCharacter.targetFinder; }
        }
    }


    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public bool useD20Menu;
        public List<GameMode> modes;

        private Dictionary<string, GameMode> modeBank;
        public Dictionary<string, GameMode> ModeBank { get => modeBank ?? GetGameModes(); }
        public Dictionary<string, GameMode> GetGameModes()
        {
            modeBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                modeBank[modes[i].name] = modes[i];
            return modeBank;
        }

        private Dictionary<InputMode, GameMode> inputBank;
        public Dictionary<InputMode, GameMode> InputBank { get => inputBank ?? GetInputModes(); }
        public Dictionary<InputMode, GameMode> GetInputModes()
        {
            inputBank ??= new();
            for (int i = 0; i < modes.Count; i++)
            {
                if (modes[i].inputMode != InputMode.None)
                    inputBank[modes[i].inputMode] = modes[i];
            }
            return inputBank;
        }

        private Dictionary<Menu, GameMode> menuBank;
        public Dictionary<Menu, GameMode> MenuBank { get => menuBank ?? GetMenuModes(); }
        public Dictionary<Menu, GameMode> GetMenuModes()
        {
            menuBank ??= new();
            for (int i = 0; i < modes.Count; i++)
            {
                if (modes[i].activeMenu != Menu.None)
                    menuBank[modes[i].activeMenu] = modes[i];
            }
            return menuBank;
        }

    }
}
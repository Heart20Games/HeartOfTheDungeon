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
        public enum InputMode { None, Selection, Character, Dialogue, LockOn, Menu, Cutscene, Spectate };
        public enum MoveMode { None, Selector, Character }
        public enum LookMode { None, Targeter, Character }
        public enum Menu { None, ControlSheet, CharacterSheet, Dialogue, Death }
        public enum Shader { None, Death }
        public enum PlayerRespawn { None, OnEnter, OnLeave }
        public static Dictionary<string, GameMode> ModeBank { get { return Main.settings.ModeBank; } }
        public static Dictionary<InputMode, GameMode> InputBank { get { return Main.settings.InputBank; } }
        public static Dictionary<Menu, GameMode> MenuBank { get { return Main.settings.MenuBank; } }

        [Serializable]
        public struct GameMode
        {
            public string name;
            public InputMode inputMode;
            public MoveMode moveMode;
            public LookMode lookMode;
            public Menu activeMenu;
            public Shader activeShader;
            public PlayerRespawn playerRespawn;
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
                        MoveMode.Character => Main.CurCharacter,
                        MoveMode.Selector => Main.curController,
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
                        LookMode.Targeter => Main.targeter,
                        _ => null,
                    };
                }
            }
            public readonly TargetFinder Finder
            {
                get
                {
                    return Main.CurCharacter ? Main.CurCharacter.TargetFinder : null;
                }
            }
        }
    }


    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public bool useD20Menu;
        public List<GameMode> modes;

        // Modes by Name
        private Dictionary<string, GameMode> modeBank;
        public Dictionary<string, GameMode> ModeBank { get => modeBank ?? GetGameModes(); }
        public Dictionary<string, GameMode> GetGameModes()
        {
            modeBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                modeBank[modes[i].name] = modes[i];
            return modeBank;
        }

        // Modes by Input Mode
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

        // Modes by Menu
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
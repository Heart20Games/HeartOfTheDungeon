using Selection;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game;
using static GameModes;

public class GameModes
{
    public enum InputMode { None, Selection, Character, Dialogue, LockOn, Menu };
    public enum MoveMode { None, Selector, Character }
    public enum LookMode { None, Targeter }
    public enum Menu { None, Controls, StatSheet }
    public static Dictionary<InputMode, GameMode> ModeBank { get { return game.settings.ModeBank; } }

    [Serializable]
    public struct GameMode
    {
        public string name;
        public InputMode inputMode;
        public MoveMode moveMode;
        public LookMode lookMode;
        public Menu activeMenu;
        public bool hudActive;
        public bool dialogueActive;
        public bool targetLock;
        public float timeScale;
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
    public Dictionary<InputMode, GameMode> modeBank;
    public Dictionary<InputMode, GameMode> ModeBank { get => modeBank ?? GetModes(); }
    public Dictionary<InputMode, GameMode> GetModes()
    {
        modeBank ??= new();
        for (int i = 0; i < modes.Count; i++)
            modeBank[modes[i].inputMode] = modes[i];
        return modeBank;
    }
    
}

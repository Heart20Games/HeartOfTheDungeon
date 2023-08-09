using Selection;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game;
using static GameModes;

public class GameModes
{
    public enum InputMode { None, Selection, Character, Dialogue, LockedOn, Dismiss };
    public enum MoveMode { None, Selector, Character }
    public static Dictionary<InputMode, GameMode> ModeBank { get { return game.settings.ModeBank; } }

    [Serializable]
    public struct GameMode
    {
        public string name;
        public InputMode inputMode;
        public MoveMode moveMode;
        public bool hudActive;
        public bool dialogueActive;
        public bool controlScreenActive;
        public bool targetLock;
        public string inputMap;
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
                return inputMode switch
                {
                    InputMode.Character => game.CurCharacter,
                    InputMode.LockedOn => game.targeter,
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

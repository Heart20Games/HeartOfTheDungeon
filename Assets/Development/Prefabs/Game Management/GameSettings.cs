using Selection;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game;
using static GameModes;

public class GameModes
{
    public enum GameMode { Selection, Character, Dialogue, LockedOn, Dismiss };
    public static Dictionary<GameMode, ModeParameters> ModeBank { get { return game.settings.ModeBank; } }

    [Serializable]
    public struct ModeParameters
    {
        public string name;
        public GameMode mode;
        public bool hudActive;
        public bool dialogueActive;
        public bool controlScreenActive;
        public bool targetLock;
        public string inputMap;
        public float timeScale;
        public readonly IControllable Controllable { get => GetControllable(); }
        public readonly IControllable GetControllable()
        {
            return mode switch
            {
                GameMode.Character => game.CurCharacter,
                GameMode.Selection => game.curController,
                GameMode.LockedOn => game.CurCharacter,
                _ => null,
            };
        }
        public readonly ILooker Looker { get => GetLooker(); }
        public readonly ILooker GetLooker()
        {
            return mode switch
            {
                GameMode.Character => game.CurCharacter,
                GameMode.LockedOn => game.targeter,
                _ => null,
            };
        }
        public readonly TargetFinder Finder { get => game.CurCharacter.targetFinder; }
    }
}


[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public bool useD20Menu;
    public List<ModeParameters> modes;
    public Dictionary<GameMode, ModeParameters> modeBank;
    public Dictionary<GameMode, ModeParameters> ModeBank { get => modeBank ?? GetModes(); }
    public Dictionary<GameMode, ModeParameters> GetModes()
    {
        modeBank ??= new();
        for (int i = 0; i < modes.Count; i++)
            modeBank[modes[i].mode] = modes[i];
        return modeBank;
    }
    
}

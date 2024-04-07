using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD
{
    using static CharacterModes;
    using static HotD.GameModes;

    public class CharacterModes
    {
        public enum ControlMode { None, Player, Brain };
        public enum MoveMode { Disabled, GravityOnly, Active };
        public enum CollisionMode { Disabled, Tall, Short };
        public enum PipMode { Off, On, Dynamic };
        
        [Serializable]
        public struct CharacterMode
        {
            public string name;

            public ControlMode controlMode;
            public MoveMode moveMode;
            public CollisionMode collisionMode;
            public PipMode pipMode;
            
            public bool displayable;
            public bool useCaster;
            public bool alive;
        }
    }

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Character/Settings", order = 1)]
    public class CharacterSettings : ScriptableObject
    {
        public List<CharacterMode> modes;

        private Dictionary<string, CharacterMode> modeBank;
        public Dictionary<string, CharacterMode> ModeBank { get => modeBank ?? GetModes(); }
        private Dictionary<string, CharacterMode> GetModes()
        {
            modeBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                modeBank[modes[i].name] = modes[i];
            return modeBank;
        }
    }
}

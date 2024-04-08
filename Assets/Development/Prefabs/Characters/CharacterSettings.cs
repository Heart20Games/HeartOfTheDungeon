using System;
using System.Collections;
using System.Collections.Generic;
using UIPips;
using UnityEngine;

namespace HotD
{
    using static CharacterModes;
    using static HotD.GameModes;

    public class CharacterModes
    {
        public enum ControlMode { None, Player, Brain };
        public enum MovementMode { Disabled, GravityOnly, Active };
        public enum CollisionMode { Disabled, Tall, Short };
        public enum LiveMode { Despawned, Alive, Dead };
        
        [Serializable]
        public struct CharacterMode
        {
            public string name;

            public ControlMode controlMode;
            public MovementMode moveMode;
            public CollisionMode collisionMode;
            public PipGenerator.DisplayMode pipMode;
            public LiveMode liveMode;
            
            public bool displayable;
            public bool useCaster;
            public bool useMoveReticle;
            public bool useInteractor;

            public readonly bool Controllable { get => controlMode == ControlMode.Player; }
        }
    }

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Character/Settings", order = 1)]
    public class CharacterSettings : ScriptableObject
    {
        public List<CharacterMode> modes;

        public bool TryGetMode<T>(T key, out CharacterMode mode)
        {
            //if (typeof(string) == typeof(string))
            //    return ModeBank.TryGetValue((string)(object)key, out mode);
            if (typeof(T) == typeof(ControlMode))
                return ControlBank.TryGetValue((ControlMode)(object)key, out mode);
            else if (typeof(T) == typeof(LiveMode))
                return LiveBank.TryGetValue((LiveMode)(object)key, out mode);
            else
            {
                mode = new();
                return false;
            }
        }

        // Modes by Name
        private Dictionary<string, CharacterMode> modeBank;
        public Dictionary<string, CharacterMode> ModeBank { get => modeBank ?? GetModes(); }
        private Dictionary<string, CharacterMode> GetModes()
        {
            modeBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                modeBank[modes[i].name] = modes[i];
            return modeBank;
        }

        // Modes by Live Mode
        private Dictionary<ControlMode, CharacterMode> controlBank;
        public Dictionary<ControlMode, CharacterMode> ControlBank { get => controlBank ?? GetControlModes(); }
        private Dictionary<ControlMode, CharacterMode> GetControlModes()
        {
            controlBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                if (!controlBank.ContainsKey(modes[i].controlMode))
                    controlBank[modes[i].controlMode] = modes[i];
            return controlBank;
        }

        // Modes by Live Mode
        private Dictionary<LiveMode, CharacterMode> liveBank;
        public Dictionary<LiveMode, CharacterMode> LiveBank { get => liveBank ?? GetLiveModes(); }
        private Dictionary<LiveMode, CharacterMode> GetLiveModes()
        {
            liveBank ??= new();
            for (int i = 0; i < modes.Count; i++)
                if (!liveBank.ContainsKey(modes[i].liveMode))
                    liveBank[modes[i].liveMode] = modes[i];
            return liveBank;
        }
    }
}

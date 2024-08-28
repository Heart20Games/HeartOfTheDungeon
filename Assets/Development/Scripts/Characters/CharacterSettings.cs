using System;
using System.Collections;
using System.Collections.Generic;
using UIPips;
using UnityEngine;

namespace HotD
{
    using static CharacterModes;
    using static HotD.CharacterModes.CharacterModifier;
    using static HotD.GameModes;
    using static PipGenerator;

    public class CharacterModes
    {
        public enum ControlMode { Default, Brain, None, Player };
        public enum MovementMode { Default, GravityOnly, Active, Disabled };
        public enum CollisionMode { Default, Tall, Short, Disabled };
        public enum LiveMode { Default, Despawned, Dead, Alive };
        
        [Serializable]
        public struct CharacterMode
        {
            public string name;

            public ControlMode controlMode;
            public CollisionMode collisionMode;
            public DisplayMode pipMode;
            public LiveMode liveMode;

            public bool canMove;
            public bool useGravity;
            public bool displayable;
            public bool useCaster;
            public bool useMoveReticle;
            public bool useInteractor;

            public readonly bool PlayerControlled { get => controlMode == ControlMode.Player; }
            public readonly bool Alive { get => liveMode == LiveMode.Alive; }
        }

        [Serializable]
        public struct CharacterModifier
        {
            public enum DBool { Default, No, Yes }
            public enum ModifierType { None, BrainDead, KnockedDown, Stunned }

            public string name;
            public ModifierType modifierType;

            public ControlMode controlMode;
            public CollisionMode collisionMode;
            public DisplayMode pipMode;
            public LiveMode liveMode;

            public DBool canMove;
            public DBool useGravity;
            public DBool displayable;
            public DBool useCaster;
            public DBool useMoveReticle;
            public DBool useInteractor;

            public readonly bool PlayerControlled { get => controlMode == ControlMode.Player; }
            public readonly bool Alive { get => liveMode == LiveMode.Alive; }

            public CharacterMode ModifyMode(CharacterMode mode)
            {
                mode.name = $"{mode.name}/{name}";

                mode.controlMode = controlMode == ControlMode.Default ? mode.controlMode : controlMode;
                mode.collisionMode = collisionMode == CollisionMode.Default ? mode.collisionMode : collisionMode;
                mode.pipMode = pipMode == DisplayMode.Default ? mode.pipMode : pipMode;
                mode.liveMode = liveMode == LiveMode.Default ? mode.liveMode : liveMode;

                mode.canMove = canMove == DBool.Default ? mode.canMove : canMove == DBool.Yes;
                mode.useGravity = useGravity == DBool.Default ? mode.useGravity : useGravity == DBool.Yes;
                mode.displayable = displayable == DBool.Default ? mode.displayable : displayable == DBool.Yes;
                mode.useCaster = useCaster == DBool.Default ? mode.useCaster : useCaster == DBool.Yes;
                mode.useMoveReticle = useMoveReticle == DBool.Default ? mode.useMoveReticle : useMoveReticle == DBool.Yes;
                mode.useInteractor = useInteractor == DBool.Default ? mode.useInteractor : useInteractor == DBool.Yes;

                return mode;
            }
        }
    }

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Character/Settings", order = 1)]
    public class CharacterSettings : ScriptableObject
    {
        public List<CharacterMode> modes;
        public List<CharacterModifier> modifiers;

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

        public bool TryGetModifier<T>(T key, out CharacterModifier modifier)
        {
            if (typeof(T) == typeof(CharacterModifier))
            {
                modifier = (CharacterModifier)(object)key;
                return true;
            }
            if (typeof(T) == typeof(string))
                return ModifierBank.TryGetValue((string)(object)key, out modifier);
            else if (typeof(T) == typeof(ModifierType))
                return ModifierTypeBank.TryGetValue((ModifierType)(object)key, out modifier);
            else
            {
                modifier = new();
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
                if (!modeBank.ContainsKey(modes[i].name))
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

        // Modifiers by Name
        private Dictionary<string, CharacterModifier> modifierBank;
        public Dictionary<string, CharacterModifier> ModifierBank { get => modifierBank ?? GetModifiers(); }
        private Dictionary<string, CharacterModifier> GetModifiers()
        {
            modifierBank ??= new();
            for (int i = 0; i < modifiers.Count; i++)
                if (!modifierBank.ContainsKey(modifiers[i].name))
                    modifierBank[modifiers[i].name] = modifiers[i];
            return modifierBank;
        }

        // Modifiers by Type
        private Dictionary<ModifierType, CharacterModifier> modifierTypeBank;
        public Dictionary<ModifierType, CharacterModifier> ModifierTypeBank { get => modifierTypeBank ?? GetModifierTypes(); }
        private Dictionary<ModifierType, CharacterModifier> GetModifierTypes()
        {
            modifierTypeBank ??= new();
            for (int i = 0; i < modifiers.Count; i++)
                if (!modifierTypeBank.ContainsKey(modifiers[i].modifierType))
                    modifierTypeBank[modifiers[i].modifierType] = modifiers[i];
            return modifierTypeBank;
        }
    }
}

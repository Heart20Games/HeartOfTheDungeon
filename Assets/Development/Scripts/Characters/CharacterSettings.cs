using HotD.Body;
using MyBox;
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

    /*
     * This script defines the structure and available settings for the Character Modes used by the main Character script.
     */

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
            [Flags] public enum Mods {  None=0,             ControlMode=1,          CollisionMode=2,    PipMode=4,      LiveMode=8, 
                                        CanMove=16,         UseGravity=32,          Displayable=64,     UseCaster=128,  UseMoveReticle=256, 
                                        UseInteractor=512 };

            public string name;
            public ModifierType modifierType;
            public Mods modsUsed;

            [Space]
            [ConditionalField("modsUsed", false, Mods.ControlMode)]     public ControlMode controlMode;
            [ConditionalField("modsUsed", false, Mods.CollisionMode)]   public CollisionMode collisionMode;
            [ConditionalField("modsUsed", false, Mods.PipMode)]         public DisplayMode pipMode;
            [ConditionalField("modsUsed", false, Mods.LiveMode)]        public LiveMode liveMode;

            [Space]
            [ConditionalField("modsUsed", false, Mods.CanMove)]         public DBool canMove;
            [ConditionalField("modsUsed", false, Mods.UseGravity)]      public DBool useGravity;
            [ConditionalField("modsUsed", false, Mods.Displayable)]     public DBool displayable;
            [ConditionalField("modsUsed", false, Mods.UseCaster)]       public DBool useCaster;
            [ConditionalField("modsUsed", false, Mods.UseMoveReticle)]  public DBool useMoveReticle;
            [ConditionalField("modsUsed", false, Mods.UseInteractor)]   public DBool useInteractor;

            public readonly bool PlayerControlled { get => controlMode == ControlMode.Player; }
            public readonly bool Alive { get => liveMode == LiveMode.Alive; }

            public readonly bool UsesMod(Mods modsUsed)
            {
                return (this.modsUsed & modsUsed) != 0;
            }

            public readonly bool IsDefault<T>(T dBool)
            {
                return dBool.Equals(default(T));
            }
            
            public readonly bool UsesMod<T>(Mods modsUsed, T dBool)
            {
                return UsesMod(modsUsed) && !IsDefault(dBool);
            }


            public readonly CharacterMode ModifyMode(CharacterMode mode)
            {
                mode.name = $"{mode.name}/{name}";

                mode.controlMode    = !UsesMod(Mods.ControlMode, controlMode)       ? mode.controlMode      : controlMode;
                mode.collisionMode  = !UsesMod(Mods.CollisionMode, collisionMode)   ? mode.collisionMode    : collisionMode;
                mode.pipMode        = !UsesMod(Mods.PipMode, pipMode)               ? mode.pipMode          : pipMode;
                mode.liveMode       = !UsesMod(Mods.LiveMode, liveMode)             ? mode.liveMode         : liveMode;

                mode.canMove        = UsesMod(Mods.CanMove, canMove)                ? mode.canMove          : canMove == DBool.Yes;
                mode.useGravity     = UsesMod(Mods.UseGravity, useGravity)          ? mode.useGravity       : useGravity == DBool.Yes;
                mode.displayable    = UsesMod(Mods.Displayable, displayable)        ? mode.displayable      : displayable == DBool.Yes;
                mode.useCaster      = UsesMod(Mods.UseCaster, useCaster)            ? mode.useCaster        : useCaster == DBool.Yes;
                mode.useMoveReticle = UsesMod(Mods.UseMoveReticle, useMoveReticle)  ? mode.useMoveReticle   : useMoveReticle == DBool.Yes;
                mode.useInteractor  = UsesMod(Mods.UseInteractor, useInteractor)    ? mode.useInteractor    : useInteractor == DBool.Yes;

                return mode;
            }
        }
    }

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Character/Settings", order = 1)]
    public class CharacterSettings : ScriptableObject
    {
        public List<CharacterMode> modes;
        public List<CharacterModifier> modifiers;

        public bool TryGetMode<T>(T key, out CharacterMode mode, bool debug=false)
        {
            //if (typeof(string) == typeof(string))
            //    return ModeBank.TryGetValue((string)(object)key, out mode);
            if (typeof(T) == typeof(ControlMode))
            {
                if (debug) Debug.Log("Seeking mode on Control");
                return ControlBank.TryGetValue((ControlMode)(object)key, out mode);
            }
            else if (typeof(T) == typeof(LiveMode))
            {
                if (debug) Debug.Log("Seeking mode on Live");
                return LiveBank.TryGetValue((LiveMode)(object)key, out mode);
            }
            else
            {
                if (debug) Debug.Log("Return default mode.");
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

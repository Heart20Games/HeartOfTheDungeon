using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Body
{
    [CreateAssetMenu(fileName = "MovementTemplate", menuName = "Character/Movement Template", order = 1)]
    public class MovementTemplate : BaseScriptableObject
    {
        public MoveSettings settings;
        public MoveModifier[] modifiers;

        public float speed { get => settings.speed; set => settings.speed = value; }
        public float maxVelocity { get => settings.maxVelocity; set => settings.maxVelocity = value; }
        public float footstepVelocity { get => settings.footstepVelocity; set => settings.footstepVelocity = value; }
        public float moveDrag { get => settings.moveDrag; set => settings.moveDrag = value; }
        public float stopDrag { get => settings.stopDrag; set => settings.stopDrag = value; }
        public bool  useGravity { get => settings.useGravity; set => settings.useGravity = value; }
        public float normalForce { get => settings.normalForce; set => settings.normalForce = value; }
        public float gravityForce { get => settings.gravityForce; set => settings.gravityForce = value; }
        public float groundDistance { get => settings.groundDistance; set => settings.groundDistance = value; }

        public void OnValidate()
        {
            if (!initialized)
            {
                settings = new("Settings");
                initialized = true;
            }
        }
    }

    [Serializable]
    public struct MoveSettings
    {
        public string name;

        [Header("Physics")]
        public float speed, maxVelocity, footstepVelocity;
        public float moveDrag, stopDrag;
        public bool useGravity;
        public float normalForce, gravityForce, groundDistance;

        public MoveSettings(string name = "Move Settings")
        {
            this.name = name;

            speed = 15000f; maxVelocity = 3f; footstepVelocity = 1f;
            moveDrag = 0.5f; stopDrag = 7.5f;
            useGravity = true;
            normalForce = 0f; gravityForce = 1f; groundDistance = 0.25f;
        }

        public MoveSettings(MoveSettings old)
        {
            name = old.name;

            speed = old.speed; maxVelocity = old.maxVelocity; footstepVelocity = old.footstepVelocity;
            moveDrag = old.moveDrag; stopDrag = old.stopDrag;
            useGravity = old.useGravity;
            normalForce = old.normalForce; gravityForce = old.gravityForce; groundDistance = old.groundDistance;
        }

        public readonly MoveSettings Modified(List<MoveModifier> modifiers)
        {
            MoveSettings modified = new(this);

            if (modifiers != null)
            {
                foreach (var modifier in modifiers)
                {
                    modifier.Modify(ref modified);
                }
            }

            return modified;
        }
    }

    [Serializable]
    public struct MoveModifier
    {
        public enum ModType { Override, Multiplicative, Additive }
        public enum Field { Speed, MaxVelocity, FootstepVelocity, MoveDrag, StopDrag, NormalForce, GravityForce, GroundDistance,
                            UseGravity }

        public string name;
        public Field field;
        public ModType type;

        [ConditionalField(true, "IsFloatField")]
        public float floatValue;
        [ConditionalField(true, "IsBoolField")]
        public bool boolValue;

        private bool IsBoolField() { return field >= Field.UseGravity; }
        private bool IsFloatField() { return field < Field.UseGravity; }

        public readonly void Modify(ref MoveSettings settings)
        {
            switch (field)
            {
                case Field.Speed: ModifyFloat(ref settings.speed); break;
                case Field.MaxVelocity: ModifyFloat(ref settings.maxVelocity); break;
                case Field.FootstepVelocity: ModifyFloat(ref settings.footstepVelocity); break;
                case Field.MoveDrag: ModifyFloat(ref settings.moveDrag); break;
                case Field.StopDrag: ModifyFloat(ref settings.stopDrag); break;
                case Field.NormalForce: ModifyFloat(ref settings.normalForce); break;
                case Field.GravityForce: ModifyFloat(ref settings.gravityForce); break;
                case Field.GroundDistance: ModifyFloat(ref settings.groundDistance); break;
                case Field.UseGravity: ModifyBool(ref settings.useGravity); break;
            }
        }

        public readonly void ModifyBool(ref bool value)
        {
            switch (type)
            {
                case ModType.Override: value = boolValue; break;
                case ModType.Multiplicative: value &= boolValue; break;
                case ModType.Additive: value |= boolValue; break;
            }
        }

        public readonly void ModifyFloat(ref float value)
        {
            switch (type)
            {
                case ModType.Override: value = floatValue; break;
                case ModType.Multiplicative: value *= floatValue; break;
                case ModType.Additive: value += floatValue; break;
            }
        }
    }
}
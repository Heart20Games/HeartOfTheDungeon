using System;
using UnityEngine;
using Body;
using HotD.Body;

namespace HotD.Castables
{
    public class CastableToLocation
    {
        public enum CastLocation { Character, Base, WeaponPoint, FiringPoint, Center}
        static public Transform GetLocationTransform(CastLocation location, Character character)
        {
            return GetLocationTransform(location, character);
        }
        static public Transform GetLocationTransform(CastLocation location, ICastCompatible compatible)
        {
            if (compatible != null)
            {
                return location switch
                {
                    CastLocation.Character => compatible.Transform,
                    CastLocation.Base => compatible.Body,
                    CastLocation.WeaponPoint => compatible.WeaponLocation,
                    CastLocation.FiringPoint => compatible.FiringLocation,
                    CastLocation.Center => compatible.Body,
                    _ => null
                };
            }
            else
            {
                Debug.LogWarning("ICastCompatible is null in GetLocationTransform");
                return null;
            }
        }

        [Serializable]
        public struct ToLocation<T>
        {
            public ToLocation(T toMove, CastLocation source, CastLocation target)
            {
                name = $"{source}/{target}";
                this.toMove = toMove;
                this.source = source;
                this.target = target;
            }
            [SerializeField] private string name;
            public T toMove;
            public CastLocation source;
            public CastLocation target;

            public readonly Transform GetSourceTransform(Character character)
            {
                return GetLocationTransform(source, character);
            }
            public readonly Transform GetTargetTransform(Character character)
            {
                return GetLocationTransform(target, character);
            }
        }

        static public ToLocation<Positionable> PositionableToLocation(ToLocation<Casted> casted)
        {
            return new(casted.toMove, casted.source, casted.target);
        }
    }
}

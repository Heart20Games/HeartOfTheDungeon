using System;
using UnityEngine;
using Body;

namespace HotD.Castables
{
    public class CastableToLocation
    {
        public enum Location { Character, WeaponPoint, FiringPoint }
        static public Transform GetLocationTransform(Location location, Character character)
        {
            return location switch
            {
                Location.Character => character.transform,
                Location.WeaponPoint => character.weaponLocation,
                Location.FiringPoint => character.firingLocation,
                _ => null
            };
        }

        [Serializable]
        public struct ToLocation<T>
        {
            public ToLocation(T toMove, Location source, Location target)
            {
                name = $"{source}/{target}";
                this.toMove = toMove;
                this.source = source;
                this.target = target;
            }
            [SerializeField] private string name;
            public T toMove;
            public Location source;
            public Location target;

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

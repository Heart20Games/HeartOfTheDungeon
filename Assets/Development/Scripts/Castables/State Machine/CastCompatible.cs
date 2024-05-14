using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

namespace HotD.Castables
{
    public class TestCastCompatible : BaseMonoBehaviour, ICastCompatible
    {
        [SerializeField] private TestWeaponDisplay weaponDisplay;
        [SerializeField] private Transform body;
        [SerializeField] private Transform weaponLocation;
        [SerializeField] private Transform firingLocation;
        [SerializeField] private Identity identity;
        [SerializeField] private CastCoordinator coordinator;
        [SerializeField] private int castableID;

        public IWeaponDisplay WeaponDisplay { get => weaponDisplay; }
        public Transform Body { get => body; }
        //public new Transform Transform { get => transform; } // Contained in BaseMonobehaviour
        public Transform WeaponLocation { get => weaponLocation; }
        public Transform FiringLocation { get => firingLocation; }
        public Identity Identity { get => identity; set => identity = value; }
        public CastCoordinator Coordinator { get => coordinator; }
        public int CastableID { get => castableID; }
    }

    public interface ICastCompatible
    {
        public IWeaponDisplay WeaponDisplay { get; }
        public Transform Body { get; }
        public Transform Transform { get; }

        public Transform WeaponLocation { get; }
        public Transform FiringLocation { get; }
        public Identity Identity { get; set; }
        public CastCoordinator Coordinator { get; }

        public int CastableID { get; }
    }

}
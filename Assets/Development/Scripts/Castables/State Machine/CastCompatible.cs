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
        [SerializeField] private Identity identity;
        [SerializeField] private CastCoordinator coordinator;

        public IWeaponDisplay WeaponDisplay { get => weaponDisplay; }
        public Transform Body { get => body; }
        //public new Transform Transform { get => transform; } // Contained in BaseMonobehaviour
        public Transform WeaponLocation { get => weaponLocation; }
        public Identity Identity { get => identity; }
        public CastCoordinator Coordinator { get => coordinator; }
    }

    public interface ICastCompatible
    {
        public IWeaponDisplay WeaponDisplay { get; }
        public Transform Body { get; }
        public Transform Transform { get; }

        public Transform WeaponLocation { get; }
        public Identity Identity { get; }
        public CastCoordinator Coordinator { get; }
    }

}
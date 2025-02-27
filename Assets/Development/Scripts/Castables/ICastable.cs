using System;
using UnityEngine;
using UnityEngine.Events;
using Body;

namespace HotD.Castables
{
    [Flags]
    public enum CastAction
    {
        None = 0,
        Equip = 1 << 0,
        Start = 1 << 2,
        PrimaryTrigger = 1 << 3,
        PrimaryRelease = 1 << 4,
        End = 1 << 5,
        UnEquip = 1 << 6,
        Continue = 1 << 7,
        SecondaryTrigger = 1 << 8,
        SecondaryRelease = 1 << 9,
    }

    public interface ICastable : ICastProperties
    {
        /* Castables will be expected to:
         * 1. Position any models or effects they manage
         * 2. Supply art for character animations
         * 3. Cleanup after themselves
         * 4. Coordinate with the Character's Animator(s)
         */

        public void QueueAction(CastAction action);
        public Vector3 Direction { get; set; }
        public bool CanCast { get; }

        //public void Cast();
        //public void UnCast();
        //public void Trigger();
        //public void Release();
        //public UnityEvent OnCasted();
        //public void Initialize(Body.Character owner);
        //public void Disable();
        //public void Enable();
        //public void Equip();
        //public void UnEquip();
        //public CastableItem GetItem();
    }
}
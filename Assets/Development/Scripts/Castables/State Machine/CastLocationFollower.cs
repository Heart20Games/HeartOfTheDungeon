using HotD.Body;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    using static CastableToLocation;

    public interface ICastCompatibleFollower
    {
        public ICastCompatible Owner { get; set; }
        public void SetOwner(ICastCompatible owner);
    }

    public abstract class ACastCompatibleFollower : BaseMonoBehaviour, ICastCompatibleFollower
    {
        protected ICastCompatible owner;
        public virtual ICastCompatible Owner { get => owner; set => SetOwner(value); }
        public virtual void SetOwner(ICastCompatible owner) { this.owner = owner; }
    }

    public class CastLocationFollower : ACastCompatibleFollower
    {
        public enum Mode { OnlyOnStart, AlsoOnUpdate }

        [Foldout("Cast Location", true)]
        [SerializeField] protected CastLocation location;
        [SerializeField] protected bool shouldReparent;
        [ConditionalField("shouldReparent")]
        [SerializeField] protected CastLocation parent;
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected Mode mode = Mode.OnlyOnStart;
        [SerializeField][ReadOnly] protected bool hasOwner = false;
        [SerializeField][ReadOnly] protected Character character;
        [SerializeField] private bool debugOwner = false;

        public override void SetOwner(ICastCompatible owner)
        {
            Print($"Set owner to {owner}", debugOwner, this);
            this.owner = owner;
            if (owner is Character)
            {
                character = owner as Character;
            }
            hasOwner = this.owner != null;
            Print($"Owner has been set to {this.owner}", debugOwner, this);
        }

        public void SetTarget(CastLocation location, Vector3 offset=new())
        {
            this.location = location;
            this.offset = offset;
        }

        public void SetParent(bool shouldReparent, CastLocation parent=CastLocation.Character)
        {
            this.parent = parent;
            this.shouldReparent = shouldReparent;
        }

        public void SetMode(Mode mode)
        {
            this.mode = mode;
        }

        private void Start()
        {
            Reparent();
            MatchToTarget();
        }

        private void FixedUpdate()
        {
            if (isActiveAndEnabled && mode == Mode.AlsoOnUpdate)
            {
                if (owner == null && character == null)
                {
                    Debug.LogWarning("Both owner and charater are null.");
                }
                else
                {
                    Reparent();
                    MatchToTarget();
                }
            }
        }

        private void Reparent()
        {
            if (shouldReparent)
            {
                Transform parent = null;

                if (owner != null)
                {
                    parent = GetLocationTransform(this.parent, owner);
                }
                else if (character != null)
                {
                    parent = GetLocationTransform(this.parent, character);
                }

                if (parent != null && parent != transform.parent)
                {
                    transform.SetParent(parent);
                }
            }
        }

        private void MatchToTarget()
        {
            Transform target = null;

            Print($"Owner null? {owner == null} / {character == null}", debugOwner, this);
            if (owner != null)
            {
                target = GetLocationTransform(location, owner);
            }
            else if (character != null)
            {
                target = GetLocationTransform(location, character);
            }

            if (target != null)
            {
                transform.position = target.position + offset;
            }
            else
            {
                Debug.LogWarning("No valid target found to follow.", this);
            }
        }
    }
}
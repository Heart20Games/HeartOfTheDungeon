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
        [Foldout("Cast Location", true)]
        [SerializeField] protected CastLocation location;
        [SerializeField] protected Vector3 offset;
        [SerializeField][ReadOnly] protected bool hasOwner = false;
        [SerializeField][ReadOnly] protected Character character;

        public override void SetOwner(ICastCompatible owner)
        {
            Print($"Set owner to {owner}", true, this);
            this.owner = owner;
            if (owner is Character)
            {
                character = owner as Character;
            }
            hasOwner = this.owner != null;
            Print($"Owner has been set to {this.owner}", true, this);
        }

        public void SetTarget(CastLocation location, Vector3 offset=new())
        {
            this.location = location;
            this.offset = offset;
        }

        private void FixedUpdate()
        {
            if (isActiveAndEnabled)
            {
                if (owner == null && character == null)
                {
                    Debug.LogWarning("Both owner and charater are null.");
                }
                else
                {
                    Transform target = null;

                    Print($"Owner null? {owner == null} / {character == null}", true, this);
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
    }
}
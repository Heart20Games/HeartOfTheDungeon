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

        private void FixedUpdate()
        {
            if (isActiveAndEnabled)
            {
                Transform target = GetLocationTransform(location, owner);
                if (target != null)
                {
                    transform.position = target.position + offset;
                }
            }
        }
    }
}
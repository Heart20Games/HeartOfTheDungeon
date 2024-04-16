using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public enum CastableState { Init, Equipped, Activating, Executing }

    public class CastableStateExecution
    {
        public CastableState state;
    }

    public class CastableStateTransition
    {
        public CastableState source;
        public CastableState destination;
    }

    public class CastableStateBased : BaseMonoBehaviour
    {

    }
}
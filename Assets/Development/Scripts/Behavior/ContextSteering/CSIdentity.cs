using System;
using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    public class CSIdentity : ScriptableObject
    {
        [Flags]
        public enum Identity { NotAny=0, Friend=1, Foe=2, Neutral=4, Obstacle=8, Target=32, Any=63 }

        static public Identity RelativeIdentity(Identity idA, Identity idB)
        {
            bool friendOrFoe = (idA == Identity.Friend) || (idA == Identity.Foe);
            Identity opponent = (Match(idA, idB) ? Identity.Friend : Identity.Foe);
            return (!friendOrFoe) ? Identity.Neutral : (opponent);
        }

        static public Identity InvertAlignment(Identity identity)
        {
            return identity ^ (Identity.Friend | Identity.Foe); // 10XXX -> 01XXX and vice-versa. 11XXX -> 00XXX and vice-versa.
        }
        
        static public bool Match(Identity idA, Identity idB)
        {
            return (idA & idB) != 0;
            
            // Simplified using Flags
            //bool neitherIsNotAny = (idA != Identity.NotAny && idB != Identity.NotAny);
            //bool atLeastOnIsAny = (idA == Identity.Any || idB == Identity.Any);
            //bool theyMatchExactly = (idA == idB);
            //return neitherIsNotAny && (theyMatchExactly || atLeastOnIsAny);
        }
    }

}
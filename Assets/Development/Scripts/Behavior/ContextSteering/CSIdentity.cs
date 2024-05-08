using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    public class CSIdentity : ScriptableObject
    {
        public enum Identity { Neutral, Friend, Foe, Any, NotAny, Obstacle, Target }

        static public Identity RelativeIdentity(Identity idA, Identity idB)
        {
            bool friendOrFoe = (idA == Identity.Friend) || (idA == Identity.Foe);
            Identity opponent = (Match(idA, idB) ? Identity.Friend : Identity.Foe);
            return (!friendOrFoe) ? Identity.Neutral : (opponent);
        }
        
        static public bool Match(Identity idA, Identity idB)
        {
            bool neitherIsNotAny = (idA != Identity.NotAny && idB != Identity.NotAny);
            bool atLeastOnIsAny = (idA == Identity.Any || idB == Identity.Any);
            bool theyMatchExactly = (idA == idB);
            return neitherIsNotAny && (theyMatchExactly || atLeastOnIsAny);
        }
    }

}
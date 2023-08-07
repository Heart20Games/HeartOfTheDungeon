using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    public class CSIdentity : ScriptableObject
    {
        public enum Identity { Neutral, Friend, Foe, Obstacle, Target }

        static public Identity RelativeIdentity(Identity idA, Identity idB)
        {
            bool friendOrFoe = (idA == Identity.Friend) || (idA == Identity.Foe);
            Identity opponent = (idA == idB ? Identity.Friend : Identity.Foe);
            return (!friendOrFoe) ? idA : opponent;
        }
    }
}
using System;

namespace HotD.Castables
{
    public static class Coordination
    {
        [Flags]
        public enum Triggers
        {
            None = 0,
            StartAction = 1 << 0,
            StartCast = 1 << 1,
            EndAction = 1 << 2,
            EndCast = 1 << 3,
        }

        public static bool HasTrigger(Triggers triggers, Triggers trigger)
        {
            return (triggers & trigger) == trigger;
        }
    }
}

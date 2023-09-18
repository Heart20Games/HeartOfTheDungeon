using UnityEngine.Events;

namespace HotD.VFX
{
    public class VFXAdjuster : BaseMonoBehaviour
    {
        public enum Mode { Threshold, Inclusion }
        public struct KeyFrame<T>
        {
            public Mode mode;
            public string property;
            public T value;
        }

        public UnityEvent<string, bool> testEvent;
    }
}

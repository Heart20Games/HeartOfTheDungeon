using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace HotD.PostProcessing
{
    public abstract class PostProcessor : BaseMonoBehaviour
    {
        public enum Status { Inactive, Transitioning, Active };

        [SerializeField] protected VolumeProfile volumeProfile;

        [Tooltip("The number of refreshes per second. (think of it like the \"framerate\")")]
        [Range(0.01f, 200f)][SerializeField] private float refreshRate = 50f; // Refreshes per second.

        [ReadOnly] public Status status = Status.Inactive;
        private Coroutine coroutine;

        void Awake()
        {
            // Get Components
            if (!volumeProfile && TryGetComponent<Volume>(out var volume)) volumeProfile = volume.profile;
            if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        }

        [ButtonMethod] public void Activate() { SetActive(true); }
        [ButtonMethod] public void Deactivate() { SetActive(false); }
        public void SetActive(bool active)
        {
            coroutine ??= StartCoroutine(Transition(active));
        }

        public abstract float HighestRate(bool activate = true);
        public abstract void TransitionStep(float time, bool activate = true);
        IEnumerator Transition(bool activate)
        {
            status = Status.Transitioning;

            float startTime = Time.time;
            float time = Time.time - startTime;
            float endTime = HighestRate(activate);

            while (time < endTime)
            {
                TransitionStep(time, activate);
                yield return new WaitForSeconds(1 / refreshRate);
                time = Time.time - startTime;
            }

            TransitionStep(endTime, activate);

            status = activate ? Status.Active : Status.Inactive;
            coroutine = null;
        }
    }
}

using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HotD.PostProcessing
{
    public abstract class PostProcessor : BaseMonoBehaviour
    {
        public enum PStatus { Inactive, Transitioning, Active, None };

        [SerializeField] protected VolumeProfile volumeProfile;
        [SerializeField] protected Volume volume;

        [Tooltip("The number of refreshes per second. (think of it like the \"framerate\")")]
        [Range(0.01f, 200f)][SerializeField] private float refreshRate = 50f; // Refreshes per second.

        [ReadOnly] public PStatus status = PStatus.Inactive;
        [ReadOnly] public PStatus targetStatus = PStatus.Inactive;
        protected Coroutine coroutine;
        protected float startTime;
        protected float endTime;

        void Awake()
        {
            // Get Components
            if (!volumeProfile && TryGetComponent<Volume>(out var volume)) volumeProfile = volume.profile;
            if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        }

        [ButtonMethod] public void Activate() { SetActive(true); }
        [ButtonMethod] public void Deactivate() { SetActive(false); }
        public virtual void SetActive(bool active)
        {
            PStatus nextStatus = active ? PStatus.Active : PStatus.Inactive;
            if (nextStatus != targetStatus)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
                targetStatus = nextStatus;
                coroutine ??= StartCoroutine(Transition(active));
            }
        }

        public abstract float HighestRate(bool activate = true);
        public abstract void TransitionStep(float time, bool activate = true);
        IEnumerator Transition(bool activate)
        {
            status = PStatus.Transitioning;

            startTime = Time.time;
            float time = Time.time - startTime;
            endTime = HighestRate(activate);

            while (time < endTime)
            {
                TransitionStep(time, activate);
                yield return new WaitForSeconds(1 / refreshRate);
                time = Time.time - startTime;
            }

            TransitionStep(endTime, activate);

            status = activate ? PStatus.Active : PStatus.Inactive;
            coroutine = null;
        }
    }
}

using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HotD.PostProcessing
{
    public abstract class PostProcessor : BaseMonoBehaviour
    {
        [SerializeField] protected VolumeProfile volumeProfile;

        [Tooltip("The number of refreshes per second. (think of it like the \"framerate\")")]
        [Range(0.01f, 200f)][SerializeField] private float refreshRate = 50f; // Refreshes per second.

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
            StartCoroutine(Transition(active));
        }

        public abstract float HighestRate(bool activate = true);
        public abstract void TransitionStep(float time, bool activate = true);
        IEnumerator Transition(bool activate)
        {
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
        }
    }
}

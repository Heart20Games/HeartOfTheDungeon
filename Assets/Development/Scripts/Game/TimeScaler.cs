using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotD
{
    public interface ITimeScalable : INamed
    {
        public float TimeScale { get; set; }
        public float SetTimeScale(float timeScale);
    }

    public class TimeScaler : BaseMonoBehaviour
    {
        // Singleton
        public static TimeScaler main;
        public static float TimeScale
        {
            get { return main == null ? 1 : main.timeScale; }
            set { if (main != null) main.SetTimeScale(value); }
        }
        public static void UpdateScalables() { TimeScale = TimeScale; }

        // Local Fields
        [SerializeField] private float timeScale = 1.0f;
        [SerializeField] private bool debugTimeScale = false;

        private void Awake()
        {
            TimeScaler.main = this;
        }

        private List<ITimeScalable> FindITimeScalables()
        {
            return new List<ITimeScalable>(FindObjectsOfType<BaseMonoBehaviour>().OfType<ITimeScalable>());
        }

        public void SetTimeScale(float timeScale)
        {
            Print($"Setting time scale: {this.timeScale} -> {timeScale}", debugTimeScale, this);
            this.timeScale = timeScale;
            List<ITimeScalable> timeScalables = FindITimeScalables();
            foreach (ITimeScalable timeScalable in timeScalables)
            {
                if (timeScalable != null)
                {
                    Print($"Passing along time scale to {timeScalable.Name}.", debugTimeScale, this);
                    timeScalable.SetTimeScale(this.timeScale);
                }
                else
                {
                    Debug.LogWarning("Invalid ITimeScalable Object", this);
                }
            }
        }
    }
}
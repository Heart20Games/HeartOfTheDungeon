using UnityEngine;
using UnityEngine.Events;

namespace HotD.VFX
{
    public class VFXAdjuster : BaseMonoBehaviour
    {
        public int MaxLevel { get => maxLevel; set => maxLevel = value; }
        [SerializeField] private int maxLevel;
        [SerializeField] private string levelPrefix = "Level";
        [SerializeField] private string levelProperty = "Charge Value";
        [SerializeField] private float postRampSpeed = 1f;

        [ReadOnly][SerializeField] private float baseValue = 0f;
        [ReadOnly][SerializeField] private int currentLevel = 0;
        [ReadOnly][SerializeField] private float currentAmount = 0f;

        [SerializeField] private float dischargeTime = 1f;
        [SerializeField] private bool discharge;

        [Header("Connections")]
        public UnityEvent<string, float> toFloatProperty;
        public UnityEvent<string, int> toIntProperty;
        public UnityEvent<string, bool> toBoolProperty;

        private float initialDischargeValue = 0f;
        private bool dischargeInitialized = false;
        private float timeOfDischarge = 0f;
        private void Update()
        {
            if (discharge)
            {
                if (!dischargeInitialized)
                {
                    initialDischargeValue = baseValue;
                    timeOfDischarge = Time.time;
                    dischargeInitialized = true;
                }
                if (baseValue > 0f)
                {
                    SetCharge(Mathf.Lerp(initialDischargeValue, 0, (Time.time - timeOfDischarge)/dischargeTime));
                }
                else
                {
                    discharge = false;
                }
            }
        }

        // Set Charge Times
        public void SetCharge(float value)
        {
            maxLevel = Mathf.Max(1, maxLevel);
            baseValue = Mathf.Max(0, value);
            currentLevel = Mathf.Max(1, Mathf.FloorToInt((baseValue * maxLevel)+1));
            
            // Current Ammount
            float levelFilled = 1 / (float)maxLevel;
            if (currentLevel <= maxLevel)
            {
                currentAmount = (baseValue % levelFilled) / levelFilled ;
            }
            else
            {
                currentAmount = ((baseValue / levelFilled) - maxLevel) * postRampSpeed;
            }
            
            // Previous Levels
            int lastLevel = Mathf.Min(currentLevel - 1, maxLevel);
            for (int i = 1; i <= lastLevel; i++)
            {
                toFloatProperty.Invoke($"{levelPrefix} {i} {levelProperty}", ((lastLevel+1) - i) + (currentAmount));
            }
            
            // Current Level
            if (currentLevel <= maxLevel)
            {
                 toFloatProperty.Invoke($"{levelPrefix} {currentLevel} {levelProperty}", currentAmount);
            }
            
            // Later Levels
            for (int i = currentLevel+1; i <= maxLevel; i++)
            {
                toFloatProperty.Invoke($"{levelPrefix} {i} {levelProperty}", 0);
            }
        }

        public void Discharge()
        {
            discharge = true;
        }
    }
}

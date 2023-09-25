using UnityEngine;
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

        public int MaxChargeLevel { get => maxChargeLevel; set => maxChargeLevel = value; }
        [SerializeField] private int maxChargeLevel;
        [SerializeField] private string chargeLimitProperty = "Charge Limit";
        [SerializeField] private string chargeLevelProperty = "Charge Level";
        [SerializeField] private string chargeAmountProperty = "Charge Amount";
        [SerializeField] private string elapsedTimeProperty = "Elapsed Time";

        [SerializeField] private float elapsedTime = 0;

        [Header("Connections")]
        public UnityEvent<string, float> toFloatProperty;
        public UnityEvent<string, int> toIntProperty;
        public UnityEvent<string, bool> toBoolProperty;

        private void OnEnable()
        {
            elapsedTime = 0;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            toFloatProperty.Invoke(elapsedTimeProperty, elapsedTime);
        }

        // Set Charge Times
        public void SetCharge(float value)
        {
            int level = Mathf.RoundToInt(maxChargeLevel / value);
            float amount = value % (1/maxChargeLevel);
            toIntProperty.Invoke(chargeLevelProperty, level);
            toFloatProperty.Invoke(chargeAmountProperty, amount);
        }

        // Set Charge Limit
        public void SetChargeLimit(int value)
        {
            toIntProperty.Invoke(chargeLimitProperty, value);
        }
    }
}

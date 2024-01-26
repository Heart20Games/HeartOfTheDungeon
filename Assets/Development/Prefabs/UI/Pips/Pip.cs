
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace UIPips
{
    //[ExecuteAlways]
    public class Pip : BaseMonoBehaviour
    {
        enum LabelPolicy { Never, OnlyMultiples, OnlyPositives, Always }

        // Fields
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool invertFilled;
        [SerializeField] internal int amount;
        [SerializeField] private LabelPolicy labelPolicy;
        [ReadOnly][SerializeField] private bool filled;

        // Events
        [Foldout("Events", true)] public UnityEvent<Sprite> onSprite;
        public UnityEvent<string> onLabel;
        public UnityEvent<bool> onFilled;
        [SerializeField] private string filledProperty;
        [Foldout("Events")] public UnityEvent<string, bool> onFilledProp;

        // Properties
        private Sprite Sprite { get => sprite; set => SetSprite(value); }
        public bool Filled { get => filled; set => SetFilled(value); }
        public int Amount { get => amount; set => SetAmount(value); }

        public void OnEnable()
        {
            onSprite.Invoke(sprite);
            onFilled.Invoke(filled != invertFilled);
            onFilledProp.Invoke(filledProperty, filled != invertFilled);
            onLabel.Invoke(LabelFromAmount(amount));
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
            if (isActiveAndEnabled)
            {
                onSprite.Invoke(sprite);
            }
        }

        public void SetFilled(bool filled)
        {
            this.filled = filled;
            if (isActiveAndEnabled)
            {
                onFilled.Invoke(filled != invertFilled);
                onFilledProp.Invoke(filledProperty, filled != invertFilled);
            }
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            if (isActiveAndEnabled)
            {
                onLabel.Invoke(LabelFromAmount(amount));
            }
        }

        public string LabelFromAmount(int amount)
        {
            return labelPolicy switch
            {
                LabelPolicy.Never => "",
                LabelPolicy.Always => amount >= 0 ? $"{amount}" : "",
                LabelPolicy.OnlyPositives => amount > 0 ? $"{amount}" : "",
                LabelPolicy.OnlyMultiples => amount > 1 ? $"{amount}" : "",
                _ => "",
            };
        }
    }
}

using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIPips
{
    [ExecuteAlways]
    public class Pip : BaseMonoBehaviour
    {
        // Fields
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool invertFilled;
        [SerializeField] internal int amount;
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

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
            onSprite.Invoke(sprite);
        }

        public void SetFilled(bool filled)
        {
            this.filled = filled;
            onFilled.Invoke(filled != invertFilled);
            onFilledProp.Invoke(filledProperty, filled != invertFilled);
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            onLabel.Invoke(amount >= 0 ? $"{amount}" : "");
        }
    }
}
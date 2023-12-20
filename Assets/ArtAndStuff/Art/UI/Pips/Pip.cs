
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]
//[RequireComponent (typeof(Image))]
[ExecuteAlways]
public class Pip : BaseMonoBehaviour
{
    // Fields
    private Animator animator;
    [SerializeField] private string filledProperty = "IsFilled";
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool invertFilled;
    [ReadOnly][SerializeField] private bool filled;

    // Events
    public UnityEvent<Sprite> onSprite;

    // Properties
    private Sprite Sprite { get => sprite; set => SetSprite(value); }
    public bool Filled { get => filled; set => SetFilled(value); }

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
        onSprite.Invoke(sprite);
    }

    public void SetFilled(bool filled)
    {
        this.filled = filled;
        if (animator != null)
            animator.SetBool(filledProperty, filled != invertFilled);
    }
}

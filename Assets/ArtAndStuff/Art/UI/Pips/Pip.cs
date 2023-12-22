
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Image))]
public class Pip : BaseMonoBehaviour
{
    private Animator animator;
    [SerializeField] private string filledProperty = "IsFilled";
    [ReadOnly][SerializeField] private bool filled;
    public bool Filled { get => filled; set => SetFilled(value); }

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetFilled(bool filled)
    {
        this.filled = filled;
        animator.SetBool(filledProperty, filled);
    }
}

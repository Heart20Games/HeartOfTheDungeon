using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityMenu : MonoBehaviour
{
    [Header("Ability Menu")]
    [SerializeField] private Canvas abilityMenu;
    [SerializeField] private Animator animator;
    [SerializeField] private bool abilityMenuActive = false;
    public Image selectedAbility;

    private void Awake()
    {
        if (animator == null)
            animator = abilityMenu.GetComponent<Animator>();
    }


    // Selection

    public void Toggle()
    {
        Select(!abilityMenuActive);
    }

    public void Select(bool activate)
    {
        if (abilityMenuActive != activate)
        {
            if (animator.HasParameter("AbilityMenuActive"))
            {
                animator.SetBool("AbilityMenuActive", activate);
                abilityMenuActive = activate;
            }
            else
            {
                Debug.LogWarning("HUD cannot find Animator parameter: AbilityMenuActive");
            }
        }
    }
}

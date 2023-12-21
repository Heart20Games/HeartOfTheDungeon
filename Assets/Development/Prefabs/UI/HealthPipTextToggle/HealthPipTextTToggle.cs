using UnityEngine;

public class HealthPipTextTToggle : MonoBehaviour
{
    [SerializeField] private Animator healthPipAnimator;

    private bool shouldDisable;

    //Animator Event.
    public void DisableText()
    {
        if(shouldDisable)
        {
            gameObject.SetActive(false);
            shouldDisable = false;
        }  
    }


    //Animator Event
    public void TickOnShouldDisable()
    {
        healthPipAnimator.Play("FadeOut");
        shouldDisable = true;
    }
}
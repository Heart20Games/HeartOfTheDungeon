using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public float startingHealth = 20f;
    public float previousHealth = 20f;
    [SerializeField]
    private GameObject healthFill;
    [SerializeField]
    private TextMeshPro healthNumber;
    [SerializeField]
    private Animator healthAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        healthFill = GameObject.Find("PlayerHealthFill");
        healthNumber = this.transform.GetComponentInChildren<TMPro.TextMeshPro>();
        healthAnimator = GetComponent<Animator>();        
    }
  
    public void UpdateHealth(float currentHealth, float totalHealth)
    {
        if(currentHealth < previousHealth)
        {
            healthAnimator.SetTrigger("Health Down");
        }
        else if(currentHealth > previousHealth)
        {
            healthAnimator.SetTrigger("Health Up");
        }
        else if(totalHealth > startingHealth)
        {
            healthAnimator.SetTrigger("Health Max Up");
        }
        else
        {
            return;
        }

        previousHealth = currentHealth;
        startingHealth = totalHealth;

        float fillPosition = (currentHealth / startingHealth);
        healthFill.transform.localPosition = new Vector3 (0, Mathf.Lerp(-220f, 0f, fillPosition), 0);
        healthNumber.text = currentHealth.ToString();
    }
}

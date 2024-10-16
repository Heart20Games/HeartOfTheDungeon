using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthTester : BaseMonoBehaviour
{
    [SerializeField]
    private PlayerStatusDisplay playerHealthUI;
    public float startingHealth = 20;
    public float currentHealth = 20;
    public bool callUpdateHealth = false;

    // Start is called before the first frame update
    void Start()
    {
        playerHealthUI = GetComponent<PlayerStatusDisplay>();
    }

    private void Update() 
    {
        if(callUpdateHealth == true)
        {
            playerHealthUI.UpdateHealth(currentHealth, startingHealth);
            callUpdateHealth = false;
        }
    }
}

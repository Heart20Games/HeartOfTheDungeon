using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlots : BaseMonoBehaviour
{
    [SerializeField]
    private List<GameObject> spellSlots = new List<GameObject>();
    [SerializeField]
    private List<GameObject> activeSpellSlots = new List<GameObject>();
    [SerializeField]
    private int totalSpellSlots;
    [SerializeField]
    private int filledSpellSlots;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateSpellSlotTotal();
    }

 
    public void UpdateSpellSlotTotal()
    {
        activeSpellSlots.Clear();
        
        if(filledSpellSlots > totalSpellSlots)
        {
            filledSpellSlots = totalSpellSlots;
        }

        for(int i = 0; i < (totalSpellSlots); i++)
        {
            activeSpellSlots.Add(spellSlots[i]);
            activeSpellSlots[i].SetActive(true);
        }

        for(int i = totalSpellSlots; i < 10; i++)
        {
            spellSlots[i].SetActive(false);
        }

        for(int i = 0; i < filledSpellSlots; i++)
        {
            GameObject thisFill = activeSpellSlots[i].transform.GetChild(1).gameObject;
            thisFill.SetActive(true);
        }

        for(int i = filledSpellSlots; i < totalSpellSlots; i++)
        {
            GameObject thisFill = activeSpellSlots[i].transform.GetChild(1).gameObject;
            thisFill.SetActive(false);
        }
    }

    public void SpendSpellSlots(int number)
    {
        

        for(int i = (filledSpellSlots); i > (filledSpellSlots - number) && i > 0; i--)
        {
            GameObject thisFill = activeSpellSlots[i - 1].transform.GetChild(1).gameObject;
            thisFill.SetActive(false);
        }

        filledSpellSlots = Mathf.Clamp((filledSpellSlots - number), 0, totalSpellSlots);
    }

    public void GainSpellSlots(int number)
    {
        
        for(int i = (Mathf.Clamp(filledSpellSlots, 0, totalSpellSlots)); i < (filledSpellSlots + number) && i < (totalSpellSlots); i++)
        {
            GameObject thisFill = activeSpellSlots[i].transform.GetChild(1).gameObject;
            thisFill.SetActive(true);                    
        }
        
        filledSpellSlots = Mathf.Clamp((filledSpellSlots + number), 0, totalSpellSlots);
    }

    public void IncreaseSpellSlots(int number)
    {
        totalSpellSlots = Mathf.Clamp((totalSpellSlots + number), 1, 10);
        Debug.Log(totalSpellSlots);
        UpdateSpellSlotTotal();
    }
}

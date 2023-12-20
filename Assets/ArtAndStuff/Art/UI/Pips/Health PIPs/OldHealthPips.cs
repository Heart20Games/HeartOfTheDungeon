using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using TMPro;
using static Body.Behavior.ContextSteering.CSIdentity;

public class OldHealthPips : Health
{
    [SerializeField]
    private List<GameObject> healthPips = new();
    private readonly List<Animator> pipAnimator = new();
    private Animator healthPipTextAnimator;
    [SerializeField]
    private TextMeshProUGUI healthTextPipPrefab;
    private TextMeshProUGUI healthTextPipObject;
    [SerializeField]
    private GameObject healthPipPrefab;
    [SerializeField]
    private Transform healthPipCanvas;
    public int lastDamaged = -1;

    void Start()
    {
        SetHealthTotal(healthTotal);
    }

    private void FixedUpdate()
    {
        transform.TrueLookAt(Camera.main.transform.position);
    }

    private void ClearPips()
    {
        foreach (GameObject pip in healthPips)
        {
            Destroy(pip);
        }
        healthPips.Clear();
        pipAnimator.Clear();
    }

    public override void SetHealthBase(int amount, int total)
    {
        health = amount;
        SetHealthTotal(total);
        CreateHealthPipText();
    }

    public override void SetHealthTotal(int amount)
    {
        healthTotal = amount;
        ClearPips();
        for (int i = 0; i < healthTotal; i++)
        {
            GameObject pip = Instantiate(healthPipPrefab, healthPipCanvas);
            healthPips.Add(pip);
            pipAnimator.Add(pip.GetComponent<Animator>());
        }
        SetHealth(Mathf.Min(healthTotal, health));
    }

    private void CreateHealthPipText()
    {
        var pipText = Instantiate(healthTextPipPrefab, healthPipCanvas);

        pipText.text = "";

        pipText.gameObject.SetActive(false);

        healthTextPipObject = pipText;
        healthPipTextAnimator = pipText.GetComponent<Animator>();
    }

    public override void SetHealth(int amount)
    {
        int healthDifference = Mathf.Abs(health - amount);

        health = amount;
        int damage = Mathf.Min(healthTotal - health, healthTotal);
        if (isActiveAndEnabled)
        {
            for (int i = 0; i < damage; i++)
            {
                pipAnimator[i].SetBool("IsDamaged", true);
                lastDamaged = i;
            }
            for (int i = damage; i < healthPips.Count; i++)
            {
                pipAnimator[i].SetBool("IsDamaged", false);
            }
        }

        if (damage > 0)
        {
            ShowHealthPipText(true, healthDifference, Color.red);
        }
    }

    public override void TakeDamage(int amount, Identity id = Identity.Neutral)
    {
        int damageToTake = Mathf.Clamp(amount, 0, (healthTotal - (lastDamaged + 1)));
        for(int i = 0; i < damageToTake; i++)
        {                                               
            lastDamaged++;
            if (isActiveAndEnabled)
            {
                pipAnimator[lastDamaged].SetBool("IsDamaged", true);                                        
            }
        }
    }

    public override void HealDamage(int amount)
    {
        int damageToHeal = Mathf.Clamp(amount, 0, (lastDamaged + 1));
        for(int i = 0; i < damageToHeal; i++)
        {                                    
            if (isActiveAndEnabled)
            {
                pipAnimator[lastDamaged].SetBool("IsDamaged", false);
            }
            lastDamaged--;                                       
        }
    }

    private void ShowHealthPipText(bool isDamage, int value, Color color)
    {
        if (healthTextPipObject == null) return;

        healthTextPipObject.gameObject.SetActive(true);

        healthPipTextAnimator.Play("FadeIn");

        if (isDamage)
        {
            healthTextPipObject.text = "-" + value;
        }
        else
        {
            healthTextPipObject.text = "+" + value;
        }

        healthTextPipObject.color = color;
    }
}

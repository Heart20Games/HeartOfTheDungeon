using Attributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CastableField : MonoBehaviour
{
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text castableNameText;

    [SerializeField] private AttributeLabel final;

    [SerializeField] private AttributeLabel bonusPrefab;
    [SerializeField] private Transform bonusParent;
    [SerializeField] private List<AttributeLabel> bonuses;
    
    [ReadOnly][SerializeField] private DependentAttribute attribute;

    public string CharacterName { get => characterNameText.name; set => characterNameText.text = value; }
    public string CastableName { get => name; set => SetCastableName(value); }
    public string FinalName { get => final.Name; set => final.Name = value; }

    public void SetCastableName(string value)
    {
        name = value;
        castableNameText.text = value;
    }

    public void SetAttribute(DependentAttribute attribute)
    {
        this.attribute = attribute;
        final.SetAttribute(attribute);
        ClearBonuses();
        if (bonusPrefab != null)
        {
            foreach (var sub in attribute.OtherAttributes)
            {
                AttributeLabel label = Instantiate(bonusPrefab, bonusParent);
                bonuses.Add(label);
                label.abbreviate = true;
                label.SetAttribute(sub.value);
            }
        }
        if (attribute != null)
            UpdateField();
    }

    private void UpdateField()
    {
        final.UpdateField();
        foreach (var bonus in bonuses)
        {
            bonus.UpdateField();
        }
    }

    public void ClearBonuses()
    {
        foreach(var bonus in bonuses)
        {
            Destroy(bonus);
        }
    }
}

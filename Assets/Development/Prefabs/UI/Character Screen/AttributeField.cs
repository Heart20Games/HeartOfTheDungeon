using Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttributeField : BaseMonoBehaviour
{
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Pips pips;
    [SerializeField] private int pipOffset = 0;
    [ReadOnly][SerializeField] private BaseAttribute attribute;

    public string Name { get => name; set => SetName(value); }

    public void SetName(string name)
    {
        this.name = name;
        nameText.text = name;
    }

    public void SetAttribute(BaseAttribute attribute)
    {
        this.attribute = attribute;
        if (attribute != null)
            UpdateField();
    }

    private void UpdateField()
    {
        bonusText.text = attribute.BaseValue.ToString();
        pips.SetFilled(attribute.BaseValue + pipOffset);
    }

    public void AddPoint()
    {
        if (attribute != null)
        {
            attribute.BaseValue += 1;
            UpdateField();
        }
    }

    public void RemovePoint()
    {
        if (attribute != null)
        {
            attribute.BaseValue -= 1;
            UpdateField();
        }
    }
}

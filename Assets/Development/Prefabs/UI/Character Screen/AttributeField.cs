using Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttributeField : BaseMonoBehaviour
{
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private Pips pips;
    [SerializeField] private int pipOffset = 0;

    public void SetAttribute(BaseAttribute attribute)
    {
        bonusText.text = attribute.BaseValue.ToString();
        pips.SetFilled(attribute.BaseValue + pipOffset);
    }
}

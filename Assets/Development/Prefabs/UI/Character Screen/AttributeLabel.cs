using Attributes;
using TMPro;
using UnityEngine;
using static Attributes.BaseAttribute;

public class AttributeLabel : BaseMonoBehaviour
{
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Part part;

    public string Name { get => name; set => SetName(value); }

    [ReadOnly][SerializeField] private BaseAttribute attribute;

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

    public void UpdateField()
    {
        bonusText.text = part switch
        {
            Part.BaseValue => attribute.BaseValue.ToString(),
            Part.BaseMultiplier => attribute.BaseMultiplier.ToString(),
            _ => "X"
        };
    }
}

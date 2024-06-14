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

    public bool abbreviate = false;

    public void SetName(string name)
    {
        this.name = name;
        nameText.text = !abbreviate ? name : name.Substring(0, 3);
    }

    public void SetAttribute(BaseAttribute attribute)
    {
        this.attribute = attribute;
        SetName(attribute.name);
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

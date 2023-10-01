using Attributes;
using UnityEngine;

public class AttributeField : BaseMonoBehaviour
{
    [SerializeField] private AttributeLabel label;
    [SerializeField] private Pips pips;
    [SerializeField] private int pipOffset = 0;
    [ReadOnly][SerializeField] private BaseAttribute attribute;

    public string Name { get => label.Name; set => label.Name = value; }

    public void SetAttribute(BaseAttribute attribute)
    {
        this.attribute = attribute;
        label.SetAttribute(attribute);
        if (attribute != null)
            UpdateField();
    }

    public void UpdateField()
    {
        label.UpdateField();
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

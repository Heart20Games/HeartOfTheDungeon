using Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UIPips;

public class AttributeField : BaseMonoBehaviour
{
    [SerializeField] private AttributeLabel label;
    [SerializeField] private PipGenerator pips;
    [SerializeField] private int pipOffset = 0;
    [SerializeField] private GameObject[] selectedIndicators = new GameObject[0];
    [ReadOnly][SerializeField] private BaseAttribute attribute;

    public bool limitReached = false;

    public bool selected;
    public int statIndex;
    public UnityEvent<int> onSelect;
    public UnityAction onUpdateField;

    [SerializeField] private bool debug = false;

    public new string Name { get => label.Name; set => label.Name = value; }

    private void Awake()
    {
        SetSelected(false);
    }

    private void Start()
    {
        SetAttribute(attribute);
    }

    private void OnEnable()
    {
        SetAttribute(attribute);
    }

    public void SetAttribute(BaseAttribute attribute)
    {
        if (debug) print($"Set Attribute: {attribute}");
        this.attribute = attribute;
        label.SetAttribute(attribute);
        if (attribute != null)
            UpdateField();
    }

    public void UpdateField()
    {
        if (debug) print($"Update Field: {name}");
        label.UpdateField();
        pips.SetFilled(Mathf.FloorToInt(attribute.FinalValue) + pipOffset);
        onUpdateField?.Invoke();
    }

    public void AddPoint()
    {
        if (attribute != null && !limitReached)
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

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        foreach (var item in selectedIndicators)
            item.SetActive(selected);
        if (selected)
            onSelect.Invoke(statIndex);
    }

    public void OnChangeSelection(InputValue inputValue)
    {
        Vector2 vector = inputValue.Get<Vector2>();
        if (vector != Vector2.zero)
        {
            if (vector.x != 0)
            {
                if (selected)
                {
                    if (vector.x < 0)
                        RemovePoint();
                    else
                        AddPoint();
                }
            }
        }
    }
}

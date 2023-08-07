using Body;
using TMPro;
using UnityEngine;

public class TargetCharacterPanel : BaseMonoBehaviour
{
    [ReadOnly] [SerializeField] private IIdentifiable target;
    public IIdentifiable Target { get => target; set => SetTarget(value); }
    public Portraits portraits;
    public SpriteRenderer portrait;
    public TMP_Text text;

    public void SetTarget(IIdentifiable target)
    {
        this.target = target;
        if (target != null)
        {
            portrait.sprite = target.Image;
            text.text = target.Name;
        }
    }
}

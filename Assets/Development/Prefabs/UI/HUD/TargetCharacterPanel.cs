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

    [SerializeField] private bool debug;

    public void SetTarget(IIdentifiable target)
    {
        this.target = target;
        if (target != null)
        {
            if (debug) print($"Has Target: {target}");
            gameObject.SetActive(true);
            portrait.sprite = target.Image;
            text.text = target.Name;
        }
        else
        {
            if (debug) print($"No Target");
            gameObject.SetActive(false);
        }
    }
}

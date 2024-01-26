using Body;
using Modifiers;
using TMPro;
using UnityEngine;
using UIPips;

public class TargetCharacterPanel : BaseMonoBehaviour
{
    [ReadOnly] [SerializeField] private IIdentifiable target;
    public IIdentifiable Target { get => target; set => SetTarget(value); }
    public Portraits portraits;
    public SpriteRenderer portrait;
    public TMP_Text text;
    public PipGenerator pips;

    [SerializeField] private bool debug;

    public void SetTarget(IIdentifiable target)
    {
        if (this.target != null)
        {
            this.target.Health?.UnSubscribe(SetHealth, SetMaxHealth);
            SetMaxHealth(0);
            SetHealth(0);
        }

        this.target = target;
        if (target != null)
        {
            if (debug) print($"Has Target: {target}");
            gameObject.SetActive(true);
            portrait.sprite = target.Image;
            text.text = target.Name;

            SetMaxHealth(target.Health.max.Value);
            SetHealth(target.Health.current.Value);
            target.Health?.Subscribe(SetHealth, SetMaxHealth);
        }
        else
        {
            if (debug) print($"No Target");
            gameObject.SetActive(false);
        }
    }

    public void SetMaxHealth(int final)
    {
        pips.SetTotal(final, PipType.Health);
    }

    public void SetHealth(int final)
    {
        pips.SetFilled(final, PipType.Health);
    }
}

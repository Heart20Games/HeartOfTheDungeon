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
    public Pips pips;

    private Modified<int> healthMax;
    private Modified<int> health;

    [SerializeField] private bool debug;

    public void SetTarget(IIdentifiable target)
    {
        if (this.target != null)
        {
            this.target.MaxHealthModder?.UnSubscribe(SetMaxHealth);
            this.target.HealthModder?.UnSubscribe(SetHealth);
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

            SetMaxHealth(target.MaxHealthModder.Value);
            SetHealth(target.HealthModder.Value);
            target.HealthModder?.Subscribe(SetHealth);
            target.MaxHealthModder?.Subscribe(SetMaxHealth);
        }
        else
        {
            if (debug) print($"No Target");
            gameObject.SetActive(false);
        }
    }

    public void SetMaxHealth(int final)
    {
        pips.SetPipCount(final);
    }

    public void SetHealth(int final)
    {
        pips.SetFilled(final);
    }
}

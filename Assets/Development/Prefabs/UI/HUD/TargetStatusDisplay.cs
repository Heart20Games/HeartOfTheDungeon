using Body;
using Modifiers;
using TMPro;
using UnityEngine;
using UIPips;

public class TargetStatusDisplay : BaseMonoBehaviour
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
        // Disconnect the old target.
        this.target?.DisconnectPips(pips);

        // Decide what replaces it
        this.target = target;
        if (target != null)
        {
            // Connect the new target.
            if (debug) print($"Has Target: {target}");
            gameObject.SetActive(true);
            gameObject.name = $"{target.Name} Status Display";
            if (portrait != null) portrait.sprite = target.Image;
            if (text != null) text.text = target.Name;

            target.ConnectPips(pips, true);
        }
        else
        {
            // There's nothing to see here, hide it.
            if (debug) print($"No Target");
            gameObject.SetActive(false);
            gameObject.name = "Empty Status Display";
        }
    }
}

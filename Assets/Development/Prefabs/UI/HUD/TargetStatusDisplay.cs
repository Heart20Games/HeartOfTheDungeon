using Body;
using Modifiers;
using TMPro;
using UnityEngine;
using UIPips;
using MyBox;

public class TargetStatusDisplay : BaseMonoBehaviour
{
    [ReadOnly] [SerializeField] private IIdentifiable target;
    public IIdentifiable Target { get => target; set => SetTarget(value); }
    public Portraits portraits;
    public SpriteRenderer portrait;
    public TMP_Text text;
    public PipGenerator pips;

    [SerializeField] private bool helpfulHierarchyName = true;

    [Space][Header("Testing")]
    [SerializeField] private bool debug;
    [ReadOnly][SerializeField] private bool hasTarget = false;
    [SerializeField] private string testEmotion = "neutral";

    public void SetTarget(IIdentifiable target)
    {
        // Disconnect the old target.
        if (target != null)
        {
            if (portrait != null)
                target.DisconnectImage(UpdatePortraitImage);
            if (pips != null)
                this.target?.DisconnectPips(pips);
        }

        // Decide what replaces it
        this.target = target;
        if (target != null)
        {
            // Connect the new target.
            if (debug) print($"Has Target: {target}");
            gameObject.SetActive(true);
            if (helpfulHierarchyName)
                gameObject.name = $"{target.Name} Status Display";
            if (portrait != null)
                target.ConnectImage(UpdatePortraitImage, true);
            if (text != null)
                text.text = target.Name;
            if (pips != null)
                target.ConnectPips(pips, true);
        }
        else
        {
            // There's nothing to see here, hide it.
            if (debug) print($"No Target");
            gameObject.SetActive(false);
            if (helpfulHierarchyName)
                gameObject.name = "Empty Status Display";
        }

        hasTarget = target != null;
    }

    public void UpdatePortraitImage(Sprite image)
    {
        if (debug) print("Updating portrait image.");
        portrait.sprite = image;
    }

    // Testing
    [ButtonMethod]
    public void TestEmotion()
    {
        string name = target.Name;
        UpdatePortraitImage(portraits.GetImage(name, testEmotion));
    }
}

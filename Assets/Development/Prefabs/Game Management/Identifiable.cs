using System;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IIdentifiable
{
    public Identity Identity { get; set; }
    public PortraitImage Portrait { get; set; }
    public Sprite Image { get; set; }
    public string Emotion { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Identifiable : BaseMonoBehaviour, IIdentifiable
{
    [Header("Identity")]
    public Portraits portraits = null;
    public Identity identity = Identity.Neutral;
    public string emotion = "neutral";

    public virtual Identity Identity { get => identity; set => identity=value; }
    public virtual PortraitImage Portrait
    {
        get => portraits.GetPortrait(Name, Emotion);
        set => NULL();
    }
    public virtual Sprite Image
    {
        get => portraits.GetImage(Name, Emotion);
        set => NULL();
    }
    public virtual string Emotion { get => emotion; set => emotion=value; }
    public virtual string Name { get => name; set => name=value; }
    public virtual string Description { get => ""; set => NULL(); }

    private void NULL() { /* Do Nothing */ }
}

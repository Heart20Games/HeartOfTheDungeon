using Modifiers;
using MyBox;
using Palmmedia.ReportGenerator.Core;
using System;
using UIPips;
using UnityEngine;
using UnityEngine.Events;
using static Body.Behavior.ContextSteering.CSIdentity;

public interface IIdentifiable
{
    public Identity Identity { get; set; }
    public PortraitImage Portrait { get; set; }
    public Sprite Image { get; set; }
    public string Emotion { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public MaxModField<int> Health { get; }

    public void ConnectPortrait(UnityAction<PortraitImage> action, bool initialize = false);
    public void DisconnectPortrait(UnityAction<PortraitImage> action);
    public void ConnectImage(UnityAction<Sprite> action, bool initialize = false);
    public void DisconnectImage(UnityAction<Sprite> action);
    public void ConnectPips(PipGenerator generator, bool initialize = false);
    public void DisconnectPips(PipGenerator generator);
}

public abstract class AIdentifiable : BaseMonoBehaviour, IIdentifiable
{
    // Fields
    [Foldout("Identity", true)]
    [Header("Identity")]
    public Portraits portraits = null;
    public Identity identity = Identity.Neutral;
    [Foldout("Identity")] public ModField<string> emotion = new("Emotion", "neutral");

    // Events
    [Foldout("Portrait Events", true)]
    [Header("Portrait Events")]
    public UnityEvent<PortraitImage> onPortrait;
    [Foldout("Portrait Events")] public UnityEvent<Sprite> onImage;

    // Properties
    public abstract Identity Identity { get; set; }
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
    public virtual string Emotion { get => emotion.current.Value; set => emotion.current.Value = value; }
    public virtual string Name { get; set; }
    public virtual string Description { get => ""; set => NULL(); }

    // Mod Fields
    public abstract MaxModField<int> Health { get; }
    public abstract MaxModField<int> Armor { get; }

    protected void NULL() { /* Do Nothing */ }

    // Initialization
    public virtual void Awake()
    {
        emotion.Subscribe(UpdatePortraits);
    }

    // Portrait Connections
    private void UpdatePortraits(string _emotion)
    {
        onPortrait.Invoke(Portrait);
        onImage.Invoke(Image);
    }

    public void ConnectPortrait(UnityAction<PortraitImage> action, bool initialize = false)
    {
        onPortrait.AddListener(action);
        if (initialize)
            action.Invoke(Portrait);
    }
    public void DisconnectPortrait(UnityAction<PortraitImage> action)
    {
        onPortrait.RemoveListener(action);
    }

    public void ConnectImage(UnityAction<Sprite> action, bool initialize = false)
    {
        onImage.AddListener(action);
        if (initialize)
            action.Invoke(Image);
    }
    public void DisconnectImage(UnityAction<Sprite> action)
    {
        onImage.RemoveListener(action);
    }


    // Pip Connections
    public void ConnectPipType(MaxModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total, bool initialize = false)
    {
        Health.Subscribe(current, total);
        if (initialize)
        {
            total(Health.max.Value);
            current(Health.current.Value);
        }
    }
    public virtual void ConnectPips(PipGenerator generator, bool initialize = false)
    {
        if (generator != null)
        {
            ConnectPipType(Health, generator.SetHealth, generator.SetHealthTotal, initialize);
            ConnectPipType(Armor, generator.SetArmor, generator.SetArmorTotal, initialize);
        }
    }

    public void DisconnectPipType(MaxModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total)
    {
        Health.UnSubscribe(current, total);
    }
    public virtual void DisconnectPips(PipGenerator generator)
    {
        if (generator != null)
        {
            DisconnectPipType(Health, generator.SetHealth, generator.SetHealthTotal);
            DisconnectPipType(Armor, generator.SetArmor, generator.SetArmorTotal);
        }
    }
}

public class Identifiable : AIdentifiable
{
    public MaxModField<int> health = new("Health", 1, 1);
    public MaxModField<int> armor = new("Armor", 0, 0);

    public override Identity Identity { get => identity; set => identity = value; }
    public override string Name { get => name; set => name = value; }
    public override MaxModField<int> Health { get => health; }
    public override MaxModField<int> Armor { get => armor; }
}

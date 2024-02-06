using Modifiers;
using MyBox;
using Palmmedia.ReportGenerator.Core;
using System;
using UIPips;
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
    public ModField<int> Health { get; }

    public void ConnectPips(PipGenerator generator, bool initialize = false);
    public void DisconnectPips(PipGenerator generator);
}

public abstract class AIdentifiable : BaseMonoBehaviour, IIdentifiable
{
    [Foldout("Identity", true)]
    [Header("Identity")]
    public Portraits portraits = null;
    public Identity identity = Identity.Neutral;
    [Foldout("Identity")] public string emotion = "neutral";
    
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
    public virtual string Emotion { get => emotion; set => emotion = value; }
    public virtual string Name { get; set; }
    public virtual string Description { get => ""; set => NULL(); }

    // Mod Fields
    public abstract ModField<int> Health { get; }
    public abstract ModField<int> Armor { get; }

    protected void NULL() { /* Do Nothing */ }

    // Pip Connections
    public void ConnectPipType(ModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total, bool initialize = false)
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

    public void DisconnectPipType(ModField<int> modField, Modded<int>.Listen current, Modded<int>.Listen total)
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
    public override Identity Identity { get => identity; set => identity = value; }
    public override string Name { get => name; set => name = value; }
    public override ModField<int> Health { get => null; }
    public override ModField<int> Armor { get => null; }
}

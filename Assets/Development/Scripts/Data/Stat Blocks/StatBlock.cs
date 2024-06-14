using Attributes;
using DataManagement;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using static StatBlock;
using Attribute = Attributes.Attribute;

[Serializable]
public struct StatBonus
{
    public Stat stat;
    public Bonus bonus;
}

[Serializable]
public struct StatAttribute
{
    public StatAttribute(Stat stat, float weight=1)
    {
        this.stat = stat;
        this.weight = weight;
    }

    public Stat stat;
    public float weight;
}

[CreateAssetMenu(fileName ="StatBlock", menuName = "Stats/StatBlock", order = 1)]
public class StatBlock : PersistentScriptableObject
{
    public enum Stat { Strength, Dexterity, Constitution, Intelligence }

    [Foldout("Stat Block", true)]
    [Header("Stat Block")]
    public Attribute strength; // = new(0, "Strength");
    public Attribute dexterity; // = new(0, "Dexterity");
    public Attribute constitution; // = new(0, "Constitution");
    public Attribute intelligence; // = new(0, "Intelligence");

    [Foldout("Stat Block")]
    public List<StatBonus> bonuses = new();

    private StatBlockData statData;

    public virtual void Initialize() { }

    public void ApplyStats(List<Stat> stats, DependentAttribute dependent)
    {
        foreach (Stat stat in stats)
        {
            Attribute attribute = GetAttribute(stat);
            if (!dependent.HasAttribute(attribute))
                dependent.AddAttribute(attribute);
        }
    }

    public void ApplyStats(List<StatAttribute> stats, DependentAttribute dependent)
    {
        foreach (StatAttribute stat in stats)
        {
            Attribute attribute = GetAttribute(stat.stat);
            if (!dependent.HasAttribute(attribute, stat.weight))
                dependent.AddAttribute(attribute, stat.weight);
        }
    }

    public Attribute GetAttribute(Stat stat)
    {
        return stat switch
        {
            Stat.Strength => strength,
            Stat.Dexterity => dexterity,
            Stat.Constitution => constitution,
            Stat.Intelligence => intelligence,
            _ => null,
        };
    }

    // Persistent Scriptable Object
    public override IPersistent GetInstance() => this;
    public override void ClearData()
    {
        statData = null;
        data.Clear();
    }

    // IPersistent
    public override List<IData> GetData()
    {
        if (statData == null) SaveToData();
        return data;
    }

    public override void LoadFromData()
    {
        if (statData != null)
        {
            strength.BaseValue = statData.strength;
            dexterity.BaseValue = statData.dexterity;
            constitution.BaseValue = statData.constitution;
            intelligence.BaseValue = statData.intelligence;
        }
    }

    public override void SaveToData()
    {
        statData ??= new StatBlockData(name);
        statData.strength = strength.BaseValue;
        statData.dexterity = dexterity.BaseValue;
        statData.constitution = constitution.BaseValue;
        statData.intelligence = intelligence.BaseValue;
        data.Add(statData);
    }
}

[System.Serializable]
public class StatBlockData : PersistentData
{
    public int strength = 1;
    public int dexterity = 1;
    public int constitution = 1;
    public int intelligence = 1;

    public StatBlockData(string name) : base(name) { }

    public override void RegisterOn(GameData gameData)
    {
        // Adds data to the appropriate list
        gameData.statBlocks.Add(this);
    }

    public override bool LoadData(GameData gameData)
    {
        StatBlockData toLoad = gameData.statBlocks.Find((StatBlockData data) => { return data.name == name; });
        if (toLoad != null)
        {
            this.strength = toLoad.strength;
            this.dexterity = toLoad.dexterity;
            this.constitution = toLoad.constitution;
            this.intelligence = toLoad.intelligence;
        }
        // Returns whether data was able to be loaded
        return toLoad != null;
    }

    public override bool SaveData(GameData gameData)
    {
        StatBlockData toSave = gameData.statBlocks.Find((StatBlockData data) => { return data.name == name; });
        if (toSave != null)
        {
            toSave.strength = this.strength;
            toSave.dexterity = this.dexterity;
            toSave.constitution = this.constitution;
            toSave.intelligence = this.intelligence;
        }
        // Returns whether data was able to be saved
        return toSave != null;
    }
}

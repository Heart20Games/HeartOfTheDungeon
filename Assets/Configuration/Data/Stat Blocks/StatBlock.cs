using Attributes;
using DataManagement;
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
    public Stat stat;
    public float weight;
}

[CreateAssetMenu(fileName ="StatBlock", menuName = "Stats/StatBlock", order = 1)]
public class StatBlock : PersistentScriptableObject
{
    public enum Stat { Strength, Dexterity, Constituion, Intelligence }
    //public enum ModType { Inc, Mul, Quad, Exp, Log }
    
    public Attribute strength;
    public Attribute dexterity;
    public Attribute constitution;
    public Attribute intelligence;

    public List<StatBonus> bonuses = new();

    private StatBlockData statData;

    // Modifiers
    //public int ModifyStat(int score, StatMod mod, float rate = 1f)
    //{
    //    return ModifyStat(score, mod.stat, mod.modType, rate);
    //}

    //public int ModifyStat(int score, Stat stat, ModType modType, float rate = 1f)
    //{
    //    return ModifyNumber(score, GetStat(stat), modType, rate);
    //}

    //public int ModifyNumber(int number, int modifier, ModType modType, float rate = 1f)
    //{
    //    return modType switch
    //    {
    //        ModType.Inc => number + (int)(modifier * rate),
    //        ModType.Mul => number * (int)(modifier * rate),
    //        ModType.Quad => (int)Mathf.Pow(modifier * rate, number),
    //        ModType.Exp => (int)Mathf.Pow(number, modifier * rate),
    //        ModType.Log => (int)Mathf.Log(number, modifier * rate),
    //        _ => number
    //    };
    //}

    public Attribute GetStat(Stat stat)
    {
        return stat switch
        {
            Stat.Strength => strength,
            Stat.Dexterity => dexterity,
            Stat.Constituion => constitution,
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
        StatBlockData toLoad = gameData.statBlocks.Find((StatBlockData data) => { return data.name == name; });
        if (toLoad != null)
        {
            toLoad.strength = this.strength;
            toLoad.dexterity = this.dexterity;
            toLoad.constitution = this.constitution;
            toLoad.intelligence = this.intelligence;
        }
        // Returns whether data was able to be saved
        return toLoad != null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static GameData;

[CreateAssetMenu(fileName ="StatBlock", menuName = "Stats/StatBlock", order = 1)]
public class StatBlock : PersistentScriptableObject
{
    public enum Stat { Strength, Dexterity, Constituion, Intelligence }
    public enum ModType { Inc, Mul, Quad, Exp, Log }
    
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;

    private StatBlockData statData;

    // Modifiers
    public int Modify(int score, Stat stat, ModType modType, float rate = 1f)
    {
        return modType switch
        {
            ModType.Inc => score + (int)(GetStat(stat) * rate),
            ModType.Mul => score * (int)(GetStat(stat) * rate),
            ModType.Quad => (int)Mathf.Pow(GetStat(stat) * rate, score),
            ModType.Exp => (int)Mathf.Pow(score, GetStat(stat) * rate),
            ModType.Log => (int)Mathf.Log(score, GetStat(stat) * rate),
            _ => score
        };
    }

    public int GetStat(Stat stat)
    {
        return stat switch
        {
            Stat.Strength => strength,
            Stat.Dexterity => dexterity,
            Stat.Constituion => constitution,
            Stat.Intelligence => intelligence,
            _ => -1,
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
            strength = statData.strength;
            dexterity = statData.dexterity;
            constitution = statData.constitution;
            intelligence = statData.intelligence;
        }
    }

    public override void SaveToData()
    {
        statData ??= new StatBlockData(name);
        statData.strength = strength;
        statData.dexterity = dexterity;
        statData.constitution = constitution;
        statData.intelligence = intelligence;
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

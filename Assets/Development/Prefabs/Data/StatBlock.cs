using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static GameData;

[CreateAssetMenu(fileName ="StatBlock", menuName = "Stats/StatBlock", order = 1)]
public class StatBlock : PersistentScriptableObject
{
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    private StatBlockData statData;

    // Persistent Scriptable Object
    public override IPersistent GetInstance()
    {
        return this;
    }

    public override void ClearData()
    {
        statData = null;
    }

    // IPersistent
    public override IData GetData()
    {
        if (statData == null) SaveToData();
        return statData;
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
    }
}

[System.Serializable]
public class StatBlockData : IData
{
    public string name;
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;

    public StatBlockData()
    {
        this.name = "Test";
        this.strength = 1;
        this.dexterity = 1;
        this.constitution = 1;
        this.intelligence = 1;
    }
    public StatBlockData(string name)
    {
        this.name = name;
        this.strength = 1;
        this.dexterity = 1;
        this.constitution = 1;
        this.intelligence = 1;
    }

    public void RegisterOn(GameData gameData)
    {
        // Adds data to the appropriate list
        gameData.statBlocks.Add(this);
    }

    public bool LoadData(GameData gameData)
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

    public bool SaveData(GameData gameData)
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

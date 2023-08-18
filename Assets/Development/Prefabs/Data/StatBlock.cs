using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

[CreateAssetMenu(fileName ="StatBlock", menuName = "Stats/StatBlock", order = 1)]
public class StatBlock : ScriptableObject, IPersistent
{
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    private StatBlockData statData = new();

    public IData GetData()
    {
        return statData;
    }

    public void LoadFromData()
    {
        if (statData != null)
        {
            strength = statData.strength;
            dexterity = statData.dexterity;
            constitution = statData.constitution;
            intelligence = statData.intelligence;
        }
    }

    public void SaveToData()
    {
        statData ??= new StatBlockData();
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
        this.strength = 1;
        this.dexterity = 1;
        this.constitution = 1;
        this.intelligence = 1;
    }

    public void RegisterOn(GameData gameData)
    {
        gameData.statBlocks.Add(this);
    }

    public void LoadData()
    {

    }

    public void SaveData()
    {
    }
}

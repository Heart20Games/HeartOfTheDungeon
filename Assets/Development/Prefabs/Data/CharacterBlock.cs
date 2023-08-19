using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBlock", menuName = "Character/Block", order = 1)]
public class CharacterBlock : StatBlock
{
    public CastableLibrary castableBank;
    public int portraitIndex = 0;
    public string characterName = "Nobody";
    public int healthBase = 1;
    public Loadout loadout = null;
    
    private CharacterData charData = null;
    
    public int MaxHealth
    {
        get => Modify(healthBase, Stat.Constituion, ModType.Quad);
    }

    // Persisent Data
    public override IPersistent GetInstance() => this;
    public override void ClearData()
    {
        Debug.Log("Clear Character Data?");
        charData = null;
        base.ClearData();
    }

    // IPersistent
    public override List<IData> GetData()
    {
        if (charData == null) SaveToData();
        return base.GetData();
    }

    public override void LoadFromData()
    {
        if (charData != null)
        {
            loadout = castableBank.GetLoadout(charData.name);
        }
        base.LoadFromData();
    }

    public override void SaveToData()
    {
        charData ??= new CharacterData(name);
        if (loadout != null)
            charData.loadout = loadout.name;
        data.Add(charData);
        base.SaveToData();
    }
}

[System.Serializable]
public class CharacterData : PersistentData
{
    public string loadout = "None";

    public CharacterData(string name) : base(name) { }

    public override void RegisterOn(GameData gameData)
    {
        // Adds data to the appropriate list
        gameData.characterBlocks.Add(this);
    }

    public override bool LoadData(GameData gameData)
    {
        CharacterData toLoad = gameData.characterBlocks.Find((CharacterData data) => { return data.name == name; });
        if (toLoad != null)
        {
            loadout = toLoad.loadout;
        }
        // Returns whether data was able to be loaded
        return toLoad != null;
    }

    public override bool SaveData(GameData gameData)
    {
        CharacterData toSave = gameData.characterBlocks.Find((CharacterData data) => { return data.name == name; });
        if (toSave != null)
        {
            toSave.loadout = loadout;
        }
        // Returns whether data was able to be saved
        return toSave != null;
    }
}
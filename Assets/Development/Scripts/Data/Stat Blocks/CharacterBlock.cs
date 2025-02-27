using Attributes;
using DataManagement;
using HotD.Castables;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBlock", menuName = "Character/Block", order = 1)]
public class CharacterBlock : StatBlock
{
    [Header("Character")]
    public CastableLibrary castableBank;
    public int portraitIndex = 0;
    public string characterName = "Nobody";
    public Loadout loadout = null;
    public int skillPoints = 0;

    [Header("Health")]
    public DependentAttribute healthMax = new(1, "Health Max");
    public List<StatAttribute> healthAttributes = new();

    [Header("Armor Class")]
    public DependentAttribute armorClass = new(0, "Armor Class");
    public List<StatAttribute> armorAttributes = new();

    private BaseAttribute[] allAttributes;
    
    private CharacterData charData = null;

    public int spentSkillPoints
    {
        get
        {
            return (int)(strength.FinalValue + constitution.FinalValue + dexterity.FinalValue + intelligence.FinalValue);
        }
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        // Names
        strength.name = "Strength";
        dexterity.name = "Dexterity";
        constitution.name = "Constitution";
        intelligence.name = "Intelligence";
        healthMax.name = "Health Max";
        armorClass.name = "Armor Class";

        // Owner
        allAttributes = new BaseAttribute[] { strength, dexterity, constitution, intelligence, healthMax, armorClass }; 
        foreach (var attribute in allAttributes)
        {
            attribute.owner = characterName;
        }

        healthMax.Clear();
        ApplyStats(healthAttributes, healthMax);
        armorClass.Clear();
        ApplyStats(armorAttributes, armorClass);
    }

    public int MaxHealth { get => (int)healthMax.FinalValue; }
    public int ArmorClass { get => (int)armorClass.FinalValue; }

    // Persisent Data
    public override IPersistent GetInstance() => this;
    public override void ClearData()
    {
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
        if (charData.loadout == "None")
            charData = null;
        else if (charData != null)
            loadout = castableBank.GetLoadout(charData.loadout);
        base.LoadFromData();
    }

    public override void SaveToData()
    {
        charData ??= new CharacterData(name);
        if (loadout != null)
            charData.loadout = loadout.name;
        else
            charData.loadout = "None";
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
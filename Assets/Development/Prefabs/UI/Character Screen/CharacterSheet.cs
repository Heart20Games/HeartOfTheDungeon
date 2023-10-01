using HotD.Castables;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using static StatBlock;

public class CharacterSheet : BaseMonoBehaviour
{
    [Serializable]
    public struct StatField
    {
        public AttributeField field;
        public Stat stat;
    }

    public CharacterBlock character;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private StatField[] statFields;
    [SerializeField] private CastableField castablePrefab;
    [SerializeField] private Transform castableParent;
    [SerializeField] private List<CastableField> castables;

    public void SetCharacter(CharacterBlock character)
    {
        Assert.IsNotNull(character);
        this.character = character;

        // Set Character Name

        // Set Character Portrait

        // Set Character Points

        // Set Character Stats
        foreach (StatField stat in statFields)
        {
            stat.field.Name = stat.stat.ToString();
            stat.field.SetAttribute(character.GetStat(stat.stat));
        }

        // Set Character Loadout
        Assert.IsNotNull(character.loadout);
        foreach (CastableItem item in character.loadout.All())
        {
            if (item.stats != null && item.stats.dealDamage)
            {
                CastableField field = Instantiate(castablePrefab, castableParent);
                castables.Add(field);
                field.CharacterName = character.characterName;
                field.CastableName = item.name;
                field.FinalName = "Damage";
                field.SetAttribute(item.stats.damage);
            }
        }
    }

    public void Clear()
    {
        foreach(CastableField field in castables)
        {
            Destroy(field);
        }
    }
}

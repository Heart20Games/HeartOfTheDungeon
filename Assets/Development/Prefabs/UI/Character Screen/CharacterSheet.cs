using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private StatField[] statFields;

    public void SetCharacter(CharacterBlock character)
    {
        // Set Character Name

        // Set Character Portrait

        // Set Character Points

        // Set Character Stats
        foreach (StatField stat in statFields)
        {
            stat.field.Name = stat.stat.ToString();
            stat.field.SetAttribute(character.GetStat(stat.stat));
        }
    }
}

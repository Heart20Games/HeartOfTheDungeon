using HotD.Castables;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
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

    [SerializeField] private int selectedIndex;

    public void Start()
    {
        for (int i = 0; i < statFields.Length; i++)
        {
            statFields[i].field.statIndex = i;
            statFields[i].field.onSelect.AddListener(OnStatSelected);
        }
        OnStatSelected(0);
    }

    public void SetCharacter(CharacterBlock character)
    {
        Assert.IsNotNull(character);
        this.character = character;

        // Clear the old bits
        Clear();

        // Set Character Name

        // Set Character Portrait

        // Set Character Points

        // Set Character Stats
        foreach (StatField stat in statFields)
        {
            stat.field.Name = stat.stat.ToString();
            stat.field.SetAttribute(character.GetAttribute(stat.stat));
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

    public void OnStatSelected(int index)
    {
        selectedIndex = (int)Mathf.Repeat(index, statFields.Length);
        for (int i = 0; i < statFields.Length; i++)
        {
            if (i != selectedIndex)
                statFields[i].field.SetSelected(false);
            else if (!statFields[i].field.selected)
                statFields[i].field.SetSelected(true);
        }
    }

    public void OnChangeSelection(InputValue inputValue)
    {
        Vector2 vector = inputValue.Get<Vector2>();
        if (vector != Vector2.zero)
        {
            if (vector.y != 0)
            {
                OnStatSelected(selectedIndex - (int)Mathf.Sign(vector.y));
            }
        }
    }

    public void Clear()
    {
        foreach(CastableField field in castables)
        {
            Destroy(field.gameObject);
        }
        castables.Clear();
    }
}

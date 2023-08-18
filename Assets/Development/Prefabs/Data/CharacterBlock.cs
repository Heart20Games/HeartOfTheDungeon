using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBlock", menuName = "Character/Block", order = 1)]
public class CharacterBlock : StatBlock
{
    public int portraitIndex = 0;
    public string characterName = "Nobody";
    public int healthBase = 1;
    public Loadout loadout = null;
    public int MaxHealth
    {
        get => MaxHealthAt(constitution);
    }

    public int MaxHealthAt(int modifer)
    {
        return healthBase + (int)Mathf.Pow(2, modifer);
    }
}

[System.Serializable]
public class CharacterBlockData : StatBlockData { }
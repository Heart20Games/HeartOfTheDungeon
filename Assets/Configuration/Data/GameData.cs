using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int lastId = -1;
    public List<StatBlockData> statBlocks = new();
    public List<CharacterData> characterBlocks = new();

    public void Initialize()
    {
        statBlocks ??= new();
        characterBlocks ??= new();
    }
}

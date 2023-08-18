using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int lastId = -1;
    public List<StatBlockData> statBlocks = new() { new() };

    public GameData(List<IPersistent> persistents)
    {
        Debug.Log($"Persistents? {persistents.Count}");
        statBlocks ??= new();
        foreach (IPersistent persistent in persistents)
        {
            IData data = persistent.GetData();
            data.RegisterOn(this);
        }
    }

    public void LoadPersistents(List<IPersistent> persistents)
    {
        if (persistents != null)
        {
            foreach (IPersistent persistent in persistents)
            {

            }
        }
    }

    public void SavePersistents(List<IPersistent> persistents)
    {
        if (persistents != null)
        {
            foreach (IPersistent persistent in persistents)
            {

            }
        }
    }
}

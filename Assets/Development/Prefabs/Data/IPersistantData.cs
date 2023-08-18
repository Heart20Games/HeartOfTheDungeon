using System;
using static GameData;

public interface IPersistent
{
    public IData GetData();
    public void LoadFromData();
    public void SaveToData();
}

public interface IData
{
    public void RegisterOn(GameData gameData);
    public void LoadData();
    public void SaveData();
}

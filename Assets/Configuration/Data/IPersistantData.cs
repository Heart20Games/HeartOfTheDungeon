
using System.Collections.Generic;

public interface IPersistent
{
    public List<IData> GetData();
    public void LoadFromData();
    public void SaveToData();
}

public interface IData
{
    public string Name { get; }
    public void RegisterOn(GameData gameData);
    public bool LoadData(GameData gameData);
    public bool SaveData(GameData gameData);
}

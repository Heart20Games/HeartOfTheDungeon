
public interface IPersistent
{
    public IData GetData();
    public void LoadFromData();
    public void SaveToData();
}

public interface IData
{
    public void RegisterOn(GameData gameData);
    public bool LoadData(GameData gameData);
    public bool SaveData(GameData gameData);
}

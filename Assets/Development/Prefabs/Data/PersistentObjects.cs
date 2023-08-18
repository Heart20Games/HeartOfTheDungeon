using UnityEngine;

public abstract class PersistentScriptableObject : BaseScriptableObject, IPersistent
{
    public override void Init()
    {
        initialized = true;
        ClearData();
        DataManager.Instance.RegisterPersistent(GetInstance());
    }
    public abstract IPersistent GetInstance();
    public abstract void ClearData();

    // IPersistent
    public abstract IData GetData();
    public abstract void LoadFromData();
    public abstract void SaveToData();
}

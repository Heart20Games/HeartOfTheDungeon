using System.Collections.Generic;
using UnityEngine;

namespace DataManagement
{
    public abstract class PersistentScriptableObject : BaseScriptableObject, IPersistent
    {
        public List<IData> data;

        private void OnDisable()
        {
            data?.Clear();
            data = null;
        }
        public override void Init()
        {
            data ??= new();
            initialized = true;
            ClearData();
            if (DataManager.Instance != null)
                DataManager.Instance.RegisterPersistent(GetInstance());
            else
                Debug.LogWarning($"No DataManager present to register {name} to.");

        }
        public abstract IPersistent GetInstance();
        public abstract void ClearData();

        // IPersistent
        public abstract List<IData> GetData();
        public abstract void LoadFromData();
        public abstract void SaveToData();
    }

    public abstract class PersistentData : IData
    {
        public string name = "Unknown";
        public PersistentData(string name)
        {
            this.name = name;
        }

        // IPersistent
        public string Name { get => name; }
        public abstract bool LoadData(GameData gameData);
        public abstract void RegisterOn(GameData gameData);
        public abstract bool SaveData(GameData gameData);
    }
}

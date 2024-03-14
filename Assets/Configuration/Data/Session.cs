using DataManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewSession", menuName = "Game/Session", order = 1)]
public class Session : PersistentScriptableObject
{
    public string scene;
    public string checkpoint;

    public SessionData sessionData;

    public virtual void Initialize() { }

    // Persistent Scriptable Object
    public override IPersistent GetInstance() => this;
    public override void ClearData()
    {
        sessionData = null;
        data.Clear();
    }

    // IPersistent
    public override List<IData> GetData()
    {
        if (sessionData == null) SaveToData();
        return data;
    }

    public override void LoadFromData()
    {
        if (sessionData != null)
        {
            this.scene = sessionData.scene;
            this.checkpoint = sessionData.checkpoint;
        }
    }

    public override void SaveToData()
    {
        sessionData ??= new SessionData(name);
        sessionData.scene= scene;
        sessionData.checkpoint = checkpoint;
        data.Add(sessionData);
    }
}

[System.Serializable]
public class SessionData : PersistentData
{
    public string scene;
    public string checkpoint;

    public SessionData(string name) : base(name)
    {
    }

    // IPersistent
    public override bool LoadData(GameData gameData)
    {
        SessionData session = gameData.session;
        if (gameData.session != null)
        {
            this.scene = session.scene;
            this.checkpoint = session.checkpoint;
        }
        return gameData != null;
    }

    public override void RegisterOn(GameData gameData)
    {
        // Add data to the GameData object.
        gameData.session = this;
    }

    public override bool SaveData(GameData gameData)
    {
        SessionData session = gameData.session;
        if (gameData.session != null)
        {
            session.scene = this.scene;
            session.checkpoint = this.checkpoint;
        }
        return gameData != null;
    }
}
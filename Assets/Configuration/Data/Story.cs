using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public struct SceneProgress
{
    public SceneProgress(string name, bool initialize=true)
    {
        this.name = name;
        checkpointsReached = initialize ? new() : null;
        partiesDefeated = initialize ? new() : null;
    }
    public SceneProgress(SceneProgressData data)
    {
        name = data.name;
        checkpointsReached = new(data.checkpointsReached);
        partiesDefeated = new(data.partiesDefeated);
    }
    public string name;
    public HashSet<string> checkpointsReached;
    public HashSet<string> partiesDefeated;
}

[CreateAssetMenu(fileName = "NewStory", menuName = "Game/Story", order = 1)]
public class Story : PersistentScriptableObject
{
    public Dictionary<string, SceneProgress> sceneProgress = new();
    public HashSet<string> yarnNodesReached = new();

    public StoryData storyData;

    // Getters and Such

    public SceneProgress GetProgress(string sceneName)
    {
        Assert.IsNotNull(sceneProgress);
        if (sceneProgress.TryGetValue(sceneName, out var scene))
            return scene;
        else
            return new(sceneName);
    }

    public bool TryGetProgress(string sceneName, out SceneProgress result, bool addNew=false)
    {
        Assert.IsNotNull(sceneProgress);
        if (sceneProgress.TryGetValue(sceneName, out result))
        {
            return true;
        }
        else if (addNew)
        {
            result = new(sceneName);
            sceneProgress.Add(sceneName, result);
            return true;
        }
        else
        {
            result = new("NULL", false);
            return false;
        }
    }


    // Persistent Stuff

    public override void ClearData()
    {
        storyData = null;
        data.Clear();
    }

    public override List<IData> GetData()
    {
        if (storyData == null) SaveToData();
        return data;
    }

    public override IPersistent GetInstance() => this;

    public override void LoadFromData()
    {
        if (storyData != null)
        {
            sceneProgress = new();
            foreach (var progress in storyData.sceneProgress)
            {
                sceneProgress.Add(progress.name, new(progress));
            }
            yarnNodesReached = new(storyData.yarnNodesReached);
        }
    }

    public override void SaveToData()
    {
        storyData ??= new(name);
        storyData.sceneProgress = new();
        foreach (var progress in sceneProgress.Values)
        {
            storyData.sceneProgress.Add(new(progress));
        }
        storyData.yarnNodesReached = new(yarnNodesReached);
        data.Add(storyData);
    }
}

[Serializable]
public struct SceneProgressData
{
    public SceneProgressData(SceneProgress progress)
    {
        name = progress.name;
        checkpointsReached = new(progress.checkpointsReached);
        partiesDefeated = new(progress.partiesDefeated);
    }
    public string name;
    public List<string> checkpointsReached;
    public List<string> partiesDefeated;
}

[Serializable]
public class StoryData : PersistentData
{
    public List<SceneProgressData> sceneProgress = new();
    public List<string> yarnNodesReached = new();

    public StoryData(string name) : base(name)
    {
        sceneProgress ??= new();
        yarnNodesReached ??= new();
    }

    public override bool LoadData(GameData gameData)
    {
        if (gameData.story != null)
        {
            sceneProgress = gameData.story.sceneProgress;
            yarnNodesReached = gameData.story.yarnNodesReached;
        }
        return gameData.story != null;
    }

    public override void RegisterOn(GameData gameData)
    {
        gameData.story = this;
    }

    public override bool SaveData(GameData gameData)
    {
        if (gameData.story != null)
        {
            gameData.story.sceneProgress = sceneProgress;
            gameData.story.yarnNodesReached = yarnNodesReached;
        }
        return gameData.story != null;
    }
}
using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SceneProgress
{
    public SceneProgress(string name)
    {
        this.name = name;
        checkpointsReached = new();
        groupsDefeated = new();
    }
    public SceneProgress(SceneProgress other)
    {
        name = other.name;
        checkpointsReached = other.checkpointsReached;
        groupsDefeated = other.groupsDefeated;
    }
    public string name;
    public List<string> checkpointsReached;
    public List<string> groupsDefeated;
}

[CreateAssetMenu(fileName = "NewStory", menuName = "Game/Story", order = 1)]
public class Story : PersistentScriptableObject
{
    public List<SceneProgress> sceneProgress = new();
    public List<string> yarnNodesReached = new();

    public StoryData storyData;

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
            sceneProgress = storyData.sceneProgress;
            yarnNodesReached = storyData.yarnNodesReached;
        }
    }

    public override void SaveToData()
    {
        storyData ??= new(name);
        storyData.sceneProgress = sceneProgress;
        storyData.yarnNodesReached = yarnNodesReached;
        data.Add(storyData);
    }
}

[Serializable]
public class StoryData : PersistentData
{
    public List<SceneProgress> sceneProgress = new();
    public List<string> yarnNodesReached = new();

    public StoryData(string name) : base(name) { }

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
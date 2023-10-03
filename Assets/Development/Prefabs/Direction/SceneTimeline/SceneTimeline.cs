using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class SceneTimeline : BaseMonoBehaviour
{
    private PlayableDirector director;
    public struct Cutscene
    {
        public string name;
        public PlayableAsset asset;
    }
    public List<Cutscene> cutscenes = new();
    public Dictionary<string, PlayableAsset> cutsceneBank;

    [ReadOnly][SerializeField] private int paused = 0;

    public void Start()
    {
        director = GetComponent<PlayableDirector>();
        UpdateCutsceneBank();
    }

    [ButtonMethod]
    public void UpdateCutsceneBank()
    {
        cutsceneBank.Clear();
        foreach (var cutscene in cutscenes)
        {
            cutsceneBank[cutscene.name] = cutscene.asset;
        }
    }

    [YarnCommand("cutsceneIdx")]
    public void Trigger(int index)
    {
        if (index >= 0 && index < cutscenes.Count)
        {
            Trigger(cutscenes[index].asset);
        }
        else
        {
            Debug.LogWarning($"Cutscene index {index} out of bounds.");
        }
    }

    [YarnCommand("cutscene")]
    public void Trigger(string name)
    {
        if (cutsceneBank.TryGetValue(name, out var asset))
        {
            Trigger(asset);
        }
        else
        {
            Debug.LogWarning($"Cutscene {name} does not exist.");
        }
    }

    public void Trigger(PlayableAsset asset)
    {
        director.Stop();
        director.playableAsset = asset;
        director.Play();
    }

    [YarnCommand("pause")]
    public void Pause()
    {
        paused -= 1;
        if (paused <= 0)
            director.Pause();
    }

    [YarnCommand("unpause")]
    public void UnPause()
    {
        paused += 1;
        if (paused >= 0)
            director.Play();
    }
}

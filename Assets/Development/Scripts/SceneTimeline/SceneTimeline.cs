using HotD;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Yarn.Unity;
using static HotD.GameModes;

public class SceneTimeline : BaseMonoBehaviour
{
    public static SceneTimeline main;

    private PlayableDirector director;
    [Serializable] public struct Cutscene
    {
        public string name;
        public PlayableDirector director;
    }
    public List<Cutscene> cutscenes = new();
    public Dictionary<string, PlayableDirector> cutsceneBank = new();

    public UnityEvent onCutsceneCompleted;

    [SerializeField] private int paused = 0;
    [SerializeField] private float shortcutTo = 0f;
    [SerializeField] private bool shortcutOnStart = false;

    private void Awake()
    {
        main = main != null ? main : this;
    }

    public void Start()
    {
        director = GetComponent<PlayableDirector>();
        UpdateCutsceneBank();
        if (shortcutOnStart) Shortcut();
    }

    private void OnDestroy()
    {
        if (SceneTimeline.main == this) SceneTimeline.main = null;
    }

    public void OnCutsceneCompleted(PlayableDirector director)
    {
        if (this.director == director)
            director.stopped -= OnCutsceneCompleted;
        onCutsceneCompleted.Invoke();
    }

    [ButtonMethod]
    public void Shortcut()
    {
        Shortcut(shortcutTo);
    }
    public void Shortcut(float shortcutTo)
    {
        if (director == null) director = GetComponent<PlayableDirector>();
        director.time = shortcutTo;
    }

    [ButtonMethod]
    public void UpdateCutsceneBank()
    {
        cutsceneBank ??= new();
        cutsceneBank.Clear();
        foreach (var cutscene in cutscenes)
        {
            cutsceneBank[cutscene.name] = cutscene.director;
        }
    }

    [YarnCommand("cutsceneIdx")]
    public void Trigger(int index)
    {
        if (index >= 0 && index < cutscenes.Count)
            Trigger(cutscenes[index].director);
        else
            Debug.LogWarning($"Cutscene index {index} out of bounds.");
    }

    [YarnCommand("cutscene")]
    public void Trigger(string name)
    {
        if (cutsceneBank.TryGetValue(name, out var director))
            Trigger(director);
        else
            Debug.LogWarning($"Cutscene {name} does not exist.");
    }

    public void Trigger(PlayableDirector director)
    {
        if (this.director != null)
        {
            this.director.Stop();
            this.director.stopped -= OnCutsceneCompleted;
        }
        this.director = director;
        this.director.stopped += OnCutsceneCompleted;
        this.director.Play();
    }

    [YarnCommand("end-cutscene")]
    public void End()
    {
        if (this.director != null)
            this.director.Stop();
        else
            Debug.LogWarning("Tried stopping a cutscene that does not exist.");
    }

    [YarnCommand("pause")]
    public void Pause()
    {
        if (this.director != null && this.director.playableGraph.IsValid() && this.director.playableGraph.GetPlayableCount() > 0)
        {
            paused -= 1;
            if (paused <= 0)
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            //director.Pause();
        }
        else
            Debug.LogWarning("Tried pausing a cutscene that does not exist.");
    }

    [YarnCommand("unpause")]
    public void UnPause()
    {
        if (this.director != null && this.director.playableGraph.IsValid() && this.director.playableGraph.GetPlayableCount() > 0)
        {
            paused += 1;
            if (paused >= 0)
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
            //director.Play();

            Game.main.SetMode(Game.main.dialogueMode);
        }
        else
            Debug.LogWarning("Tried unpausing a cutscene that does not exist.");
    }
}

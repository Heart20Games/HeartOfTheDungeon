using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace HotD
{
    using static GameModes;
    using static YarnTags;

    public interface ITalker
    {
        // Actions
        public void CompleteTalking();
        public void Talk();
        public void Talk(string targetNode);
    }

    public class Talker : BaseMonoBehaviour, ITalker
    {
        [Header("Parts")]
        public Game game;
        [ConditionalField(true, "ShouldUseDialogueRunner")]
        public DialogueRunner dialogueRunner;
        [ConditionalField(true, "ShouldUseSceneTimeline")]
        public SceneTimeline sceneTimeline;

        public enum TalkMode { DialogueRunner, SceneTimeline, EventsOnly }
        [Header("Settings")]
        public TalkMode talkMode;
        public GameObject virtualCamera;
        public string targetNode = "";

        [Foldout("Events", true)]
        public UnityEvent onStartTalking;
        [Foldout("Events")] public UnityEvent onDoneTalking;
        
        private InputMode prevMode;

        private readonly List<IViewable> viewables = new();
        private List<IViewable> Viewables
        {
            get
            {
                viewables.Clear();
                DialogueViewBase[] dialogueViews = dialogueRunner.dialogueViews;
                for (int i = 0; i < dialogueViews.Length; i++)
                {
                    if (dialogueViews[i] is IViewable)
                    {
                        viewables.Add(dialogueViews[i] as IViewable);
                    }
                }
                return viewables;
            }
        }

        // Initialization

        private void Start()
        {
            game = Game.main;
            if (game != null && game.userInterface != null)
                dialogueRunner = game.userInterface.dialogueRunner;
            sceneTimeline = SceneTimeline.main;
        }

        // Actions

        public virtual void CompleteTalking()
        {
            dialogueRunner.onDialogueComplete.RemoveListener(CompleteTalking);
            sceneTimeline.onCutsceneCompleted.RemoveListener(CompleteTalking);
            game.InputMode = prevMode;
            if (virtualCamera != null)
                virtualCamera.SetActive(false);
            ResetMode();
            onDoneTalking.Invoke();
        }
        public void Talk() { Talk(targetNode); }
        public void Talk(string targetNode)
        {
            if (isActiveAndEnabled && (!ShouldUseDialogueRunner() || NodeIsValid(targetNode)))
            {
                if (virtualCamera != null)
                    virtualCamera.SetActive(true);

                switch (talkMode)
                {
                    case TalkMode.DialogueRunner:
                        game.StartDialogue(targetNode, OnNodeStarted, CompleteTalking); break;
                    case TalkMode.SceneTimeline:
                        if (sceneTimeline != null)
                        {
                            sceneTimeline.onCutsceneCompleted.AddListener(CompleteTalking);
                            sceneTimeline.Trigger(targetNode);
                        }
                        break;
                }

                onStartTalking.Invoke();
            }
        }

        // Helpers and Such

        private bool ShouldUseDialogueRunner()
        {
            return talkMode == TalkMode.DialogueRunner;
        }

        private bool ShouldUseSceneTimeline()
        {
            return talkMode == TalkMode.SceneTimeline;
        }

        private void ResetMode()
        {
            game.InputMode = prevMode;
            if (virtualCamera != null)
            {
                virtualCamera.SetActive(false);
            }
        }

        private bool NodeIsValid(string node)
        {
            if (dialogueRunner == null)
            {
                Debug.LogWarning("No Dialogue Runner to Start Talking");
                return false;
            }
            else if (node == "" || !dialogueRunner.NodeExists(node))
            {
                Debug.LogWarning($"No target node '{node}' exists. ({name})");
                return false;
            }
            else return true;
        }

        private void OnNodeStarted(string node)
        {
            IEnumerable<string> tags = dialogueRunner.Dialogue.GetTagsForNode(targetNode);
            SetNodeInclusion(tags, Viewables);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using static Game;
using static YarnTags;

public class Talker : BaseMonoBehaviour
{
    [HideInInspector] public Game game;
    [HideInInspector] public DialogueRunner dialogueRunner;
    public GameObject virtualCamera;
    public string targetNode = "";

    public UnityEvent onStartTalking;
    public UnityEvent onDoneTalking;
    private GameMode prevMode;

    void Start()
    {
        onDoneTalking.AddListener(ResetMode);
    }

    private void ResetMode()
    {
        game.Mode = prevMode;
        if (virtualCamera != null)
        {
            virtualCamera.SetActive(false);
        }
    }

    // Actions
    public void CompleteTalking()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(CompleteTalking);
        onDoneTalking.Invoke();
    }
    public void Talk() { Talk(targetNode); }
    public void Talk(string targetNode)
    {
        if (dialogueRunner != null)
        {
            if (targetNode != "" && dialogueRunner.NodeExists(targetNode))
            {
                if (virtualCamera != null)
                {
                    virtualCamera.SetActive(true);
                }
                prevMode = game.Mode;
                game.Mode = GameMode.Dialogue;
                dialogueRunner.Stop();
                dialogueRunner.onNodeStart.AddListener(OnNodeStarted);
                dialogueRunner.onDialogueComplete.AddListener(CompleteTalking);
                dialogueRunner.StartDialogue(targetNode);
                onStartTalking.Invoke();
            }
            else
            {
                Debug.LogWarning($"No target node '{targetNode}' exists. ({name})");
            }
        }
        else
        {
            Debug.LogWarning("No Dialogue Runner to Start Talking");
        }
    }

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

    public void OnNodeStarted(string node)
    {
        IEnumerable<string> tags = dialogueRunner.Dialogue.GetTagsForNode(targetNode);
        SetNodeInclusion(tags, Viewables);
    }
}

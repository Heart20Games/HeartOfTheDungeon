using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using static Game;

public class Talker : BaseMonoBehaviour
{
    [HideInInspector] public Game game;
    [HideInInspector] public DialogueRunner dialogueRunner;
    public GameObject virtualCamera;
    public string targetNode = "";

    public UnityEvent onStartTalking;
    public UnityEvent onDoneTalking;
    private GameMode prevMode;

    // Start is called before the first frame update
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
            if (targetNode != "")
            {
                if (virtualCamera != null)
                {
                    virtualCamera.SetActive(true);
                }
                prevMode = game.Mode;
                game.Mode = GameMode.Dialogue;
                dialogueRunner.Stop();
                dialogueRunner.onDialogueComplete.AddListener(CompleteTalking);
                dialogueRunner.StartDialogue(targetNode);
                onStartTalking.Invoke();
            }
            else
            {
                Debug.LogWarning("No target node");
            }
        }
        else
        {
            Debug.LogWarning("No Dialogue Runner to Start Talking");
        }
    }
}

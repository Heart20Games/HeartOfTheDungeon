using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class Interactor : MonoBehaviour
{
    public bool talkable = false;
    public string targetNode = "";

    public DialogueRunner dialogueRunner;
    public UnityEvent onStartTalking;
    public UnityEvent onDoneTalking;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.AddListener(onDoneTalking.Invoke);
        }
    }

    public void FoundTalkable(string dialogueNode)
    {
        talkable = true;
        targetNode = dialogueNode;
    }

    public void LeftTalkable(string dialogueNode)
    {
        if (targetNode == dialogueNode)
        {
            talkable = false;
            targetNode = "";
        }
    }

    public void StartTalking()
    {
        Debug.Log("Trying to Talk");
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue(targetNode);
        talkable = false;
        onStartTalking.Invoke();
    }


    // Actions

    public bool Talk()
    {
        if (talkable && dialogueRunner != null)
        {
            if (targetNode != "")
            {
                StartTalking();
                return true;
            }
            else
            {
                Debug.LogWarning("No target node");
            }
        }
        else if (dialogueRunner == null)
        {
            Debug.LogWarning("No Dialogue Runner to Start Talking");
        }
        return false;
    }

    
}

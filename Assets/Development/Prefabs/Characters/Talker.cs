using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class Talker : MonoBehaviour
{
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

    // Actions
    public void Talk() { Talk(targetNode); }
    public void Talk(string targetNode)
    {
        if (dialogueRunner != null)
        {
            if (targetNode != "")
            {
                dialogueRunner.Stop();
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

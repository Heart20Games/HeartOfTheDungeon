using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class PlayerCore : MonoBehaviour
{
    public bool talkable = false;
    public string targetNode = "";

    public GameObject dialogueHolder;

    DialogueRunner dialogueRunner;

    private void Start()
    {
        if (dialogueHolder != null)
        {
            dialogueRunner = dialogueHolder.GetComponent<DialogueRunner>();
            dialogueRunner.onDialogueComplete.AddListener(DoneTalking);
        }
    }
    // For keeping track of things like health and other instance specific things.
    // Stat block here


    // Public methods here
    public void Die()
    {
        Debug.Log("You are Dead");
        SceneManager.LoadScene("GameOver"); // Whisks us directly to the game over screen.
    }

    public void Talk()
    {
        if (talkable && dialogueRunner != null && targetNode != "")
        {
            dialogueRunner.Stop();
            Debug.Log("I'm talking now");
            dialogueRunner.StartDialogue(targetNode);
            gameObject.GetComponent<Movement>().canMove = false;
        }
        else
        {
            Debug.LogWarning("No Dialogue Runner to Start Talking");
        }
    }

    private void DoneTalking()
    {
        Debug.Log("I'm done talking now");
        gameObject.GetComponent<Movement>().canMove = true;
    }

    [YarnCommand("enter_room")]
    private void EnterRoom(string roomName)
    {
        if (SceneUtility.GetBuildIndexByScenePath(roomName) >= 0)
        {
            SceneManager.LoadScene(roomName);
        }
        else
        {
            Debug.LogWarning("Scene " + roomName + " not in build.");
        }
    }
}

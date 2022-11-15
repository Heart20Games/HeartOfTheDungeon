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
        dialogueRunner = dialogueHolder.GetComponent<DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(DoneTalking);
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
        Debug.Log("I'm talking now");
        if (talkable)
        {
            dialogueRunner.StartDialogue(targetNode);
            gameObject.GetComponent<Movement>().canMove = false;
        }
    }

    private void DoneTalking()
    {
        gameObject.GetComponent<Movement>().canMove = true;
    }
}

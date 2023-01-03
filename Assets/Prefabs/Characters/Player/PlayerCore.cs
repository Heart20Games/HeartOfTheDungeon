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

    public Movement moveControls;
    public GameObject dialogueHolder;
    DialogueRunner dialogueRunner;
    private PlayerAttack attacker;

    private void Start()
    {
        moveControls = GetComponent<Movement>();
        attacker = GetComponent<PlayerAttack>();
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
        SceneManager.LoadScene("GameOver"); // Whisks us directly to the game over screen.
    }

    public void Talk()
    {
        if (talkable && dialogueRunner != null)
        {
            if (targetNode != "")
            {
                dialogueRunner.Stop();
                dialogueRunner.StartDialogue(targetNode);
                moveControls.canMove = false;
                attacker.canAttack = false;
                talkable = false;
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

    private void DoneTalking()
    {
        moveControls.canMove = true;
        attacker.canAttack = true;
    }
}

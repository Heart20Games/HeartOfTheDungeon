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

    public DialogueRunner dialogueRunner;
    private Character character;
    private Movement moveControls;
    private PlayerAttack Attacker;

    private void Start()
    {
        character = GetComponent<Character>();
        moveControls = GetComponent<Movement>();
        Attacker = GetComponent<PlayerAttack>();
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.AddListener(DoneTalking);
        }
        print(character.weapon.name);
        Attacker.Castable = character.weapon;
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
                Attacker.active = false;
                talkable = false;
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
    }

    private void DoneTalking()
    {
        moveControls.canMove = true;
        Attacker.active = true;
    }

    public void Special()
    {
        if (Attacker.active)
        {
            Attacker.Castable = character.ability;
            Attacker.Slashie(moveControls.getAttackVector());
        }
    }

    public void Attack()
    {
        if (Attacker.active)
        {
            Attacker.Castable = character.weapon;
            Attacker.Slashie(moveControls.getAttackVector());
        }
    }
}

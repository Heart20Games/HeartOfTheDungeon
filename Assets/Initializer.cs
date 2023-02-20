using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Initializer : MonoBehaviour
{
    private Character player;
    private Character[] characters;
    private DialogueRunner dialogueRunner;

    private GameController gameController;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerCore>().GetComponent<Character>();
        characters = FindObjectsOfType<Character>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (gameController.playerCharacter == null)
        {
            gameController.playerCharacter = player;
        }

        foreach (Character character in characters)
        {
            Interactor interactor = character.GetComponent<Interactor>();
            if (interactor != null )
            {
                interactor.dialogueRunner = dialogueRunner;
            }
        }
    }
}

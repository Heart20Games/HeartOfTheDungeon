using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Initializer : MonoBehaviour
{
    private Character player;
    private Character[] characters;
    private DialogueRunner dialogueRunner;
    private UserInterface userInterface;
    private HUD hud;

    private GameController gameController;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
        player = FindObjectOfType<PlayerCore>().GetComponent<Character>();
        characters = FindObjectsOfType<Character>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        userInterface = FindObjectOfType<UserInterface>();
        hud = FindAnyObjectByType<HUD>();

        if (gameController.playerCharacter == null)
        {
            gameController.playerCharacter = player;
        }

        foreach (Character character in characters)
        {
            Interactor interactor = character.GetComponent<Interactor>();
            if (interactor != null)
            {
                interactor.dialogueRunner = dialogueRunner;
            }
        }

        gameController.userInterface = userInterface;
        gameController.hud = hud;

        AssetNonNull("GameController", gameController, "on GameObject");
        AssetNonNull("PlayerCore", player);
        AssetNonNull("DialogueRunner", dialogueRunner);
        AssetNonNull("UserInterface", userInterface);
        AssetNonNull("HUD", hud);
    }

    private void AssetNonNull(string typeName, MonoBehaviour monoBehaviour, string context= "in scene")
    {
        if (monoBehaviour == null)
        {
            Debug.LogWarning("Can't find any " + typeName + " " + context + ".");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}

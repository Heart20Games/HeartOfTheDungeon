using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class Initializer : MonoBehaviour
{
    public FModEventLibary defaultFModLibrary;

    private Character player;
    private Character[] characters;
    private Talker[] talkers;
    private FModEventPlayer[] fmodPlayers;
    private DialogueRunner dialogueRunner;
    private UserInterface userInterface;
    private List<ITimeScalable> timeScalables;
    private List<Interactable> interactables;
    private GameController[] gameControls;
    private HUD hud;

    private Game game;

    private void Awake()
    {
        game = GetComponent<Game>();
        player = FindObjectOfType<Character>();
        characters = FindObjectsOfType<Character>();
        talkers = FindObjectsOfType<Talker>();
        fmodPlayers = FindObjectsOfType<FModEventPlayer>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        userInterface = FindObjectOfType<UserInterface>();
        timeScalables = new List<ITimeScalable>(FindObjectsOfType<MonoBehaviour>().OfType<ITimeScalable>());
        interactables = new List<Interactable>(FindObjectsOfType<Interactable>());
        hud = FindAnyObjectByType<HUD>();


        if (game.playerCharacter == null)
        {
            game.playerCharacter = player;
        }

        if (dialogueRunner == null && userInterface != null)
        {
            dialogueRunner = userInterface.dialogueRunner;
        }

        print("Dialogue Runner: " + (dialogueRunner != null));

        foreach (Talker talker in talkers)
        {
            talker.game = game;
            talker.dialogueRunner = dialogueRunner;
        }

        if (defaultFModLibrary != null)
        {
            defaultFModLibrary.Initialize();
            foreach (FModEventPlayer fmodPlayer in fmodPlayers)
            {
                if (fmodPlayer.libary == null)
                {
                    fmodPlayer.libary = defaultFModLibrary;
                }
            }
        }

        game.userInterface = userInterface;
        game.timeScalables = timeScalables;
        game.interactables = interactables;
        game.hud = hud;

        AssetNonNull("Game", game, "on GameObject");
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
}

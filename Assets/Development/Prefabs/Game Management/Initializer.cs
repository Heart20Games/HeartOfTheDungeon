using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;
using Body;

public class Initializer : BaseMonoBehaviour
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
        timeScalables = new List<ITimeScalable>(FindObjectsOfType<BaseMonoBehaviour>().OfType<ITimeScalable>());
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
        AssetNonNull("Character", player);
        AssetNonNull("DialogueRunner", dialogueRunner);
        AssetNonNull("UserInterface", userInterface);
        AssetNonNull("HUD", hud);
    }

    private void AssetNonNull(string typeName, MonoBehaviour BaseMonoBehaviour, string context= "in scene")
    {
        if (BaseMonoBehaviour == null)
        {
            Debug.LogWarning("Can't find any " + typeName + " " + context + ".");
        }
    }
}

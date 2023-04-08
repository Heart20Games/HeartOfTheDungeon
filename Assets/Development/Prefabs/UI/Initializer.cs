using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class Initializer : MonoBehaviour
{
    public FModEventLibary defaultFModLibrary;

    private Character player;
    private Character[] characters;
    private FModEventPlayer[] fmodPlayers;
    private DialogueRunner dialogueRunner;
    private UserInterface userInterface;
    private List<ITimeScalable> timeScalables;
    private GameController[] gameControls;
    private HUD hud;

    private Game gameController;

    private void Awake()
    {
        gameController = GetComponent<Game>();
        player = FindObjectOfType<PlayerCore>().GetComponent<Character>();
        characters = FindObjectsOfType<Character>();
        fmodPlayers = FindObjectsOfType<FModEventPlayer>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        userInterface = FindObjectOfType<UserInterface>();
        timeScalables = new List<ITimeScalable>(FindObjectsOfType<MonoBehaviour>().OfType<ITimeScalable>());
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

        gameController.userInterface = userInterface;
        gameController.timeScalables = timeScalables;
        gameController.hud = hud;

        AssetNonNull("Game", gameController, "on GameObject");
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

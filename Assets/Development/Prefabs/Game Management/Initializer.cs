using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;
using Body;
using HotD.PostProcessing;

namespace HotD
{
    public class Initializer : BaseMonoBehaviour
    {
        public FModEventLibary defaultFModLibrary;

        private BaseScriptableObject[] scriptableObjects;
        private Character player;
        private Character[] characters;
        private Talker[] talkers;
        private FModEventPlayer[] fmodPlayers;
        private DialogueRunner dialogueRunner;
        private UserInterface userInterface;
        private VolumeManager volumeManager;
        private List<ITimeScalable> timeScalables;
        private List<Interactable> interactables;
        private GameController[] gameControls;
        private HUD hud;
        private List<Checkpoint> checkpoints;

        private Game game;

        private void Awake()
        {
            game = GetComponent<Game>();
            characters = FindObjectsOfType<Character>();
            game.allCharacters = new List<Character>(characters);
            game.cardboardCutouts = new List<Cutouts>(FindObjectsOfType<Cutouts>());
            gameControls = FindObjectsOfType<GameController>();

            if (game.checkpoints == null || game.checkpoints.Count == 0)
            {
                checkpoints = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
                game.checkpoints = checkpoints;
                foreach (var checkpoint in checkpoints)
                {
                    if (checkpoint.session == null || (game.session != null && checkpoint.session != game.session))
                        checkpoint.session = game.session;
                }
            }

            scriptableObjects = (BaseScriptableObject[])Resources.FindObjectsOfTypeAll(typeof(BaseScriptableObject));
            foreach (var scriptableObject in scriptableObjects)
            {
                scriptableObject.Init();
            }

            if (dialogueRunner == null && userInterface != null)
            {
                dialogueRunner = userInterface.dialogueRunner;
            }

            talkers = FindObjectsOfType<Talker>();
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            foreach (Talker talker in talkers)
            {
                talker.game = game;
                talker.dialogueRunner = dialogueRunner;
            }

            if (defaultFModLibrary != null)
            {
                defaultFModLibrary.Initialize();
                fmodPlayers = FindObjectsOfType<FModEventPlayer>();
                foreach (FModEventPlayer fmodPlayer in fmodPlayers)
                {
                    if (fmodPlayer.libary == null)
                    {
                        fmodPlayer.libary = defaultFModLibrary;
                    }
                }
            }

            userInterface = FindObjectOfType<UserInterface>();
            game.userInterface = userInterface;

            volumeManager = FindObjectOfType<VolumeManager>();
            game.volumeManager = volumeManager;

            timeScalables = new List<ITimeScalable>(FindObjectsOfType<BaseMonoBehaviour>().OfType<ITimeScalable>());
            game.timeScalables = timeScalables;

            interactables = new List<Interactable>(FindObjectsOfType<Interactable>());
            game.interactables = interactables;

            hud = FindAnyObjectByType<HUD>();
            game.hud = hud;

            AssetNonNull("Game", game, "on GameObject");
            AssetNonNull("DialogueRunner", dialogueRunner);
            AssetNonNull("UserInterface", userInterface);
            AssetNonNull("HUD", hud);
        }

        private void AssetNonNull(string typeName, MonoBehaviour BaseMonoBehaviour, string context = "in scene")
        {
            if (BaseMonoBehaviour == null)
            {
                Debug.LogWarning("Can't find any " + typeName + " " + context + ".");
            }
        }

        public void OnGameDataInitialized()
        {
            foreach (GameController controller in gameControls)
            {
                controller.OnDataInitialized();
            }
        }
    }
}
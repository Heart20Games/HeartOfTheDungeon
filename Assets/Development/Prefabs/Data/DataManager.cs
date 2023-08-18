using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * As per Trevor Mock's video "How to make a Save & Load System in Unity | 2022"
 * Link: https://www.youtube.com/watch?v=aUi9aijvpgs&list=PL3viUl9h9k7-tMGkSApPdu4hlUBagKial&index=2
 */

public class DataManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    
    private GameData gameData;
    private FileDataHandler dataHandler;

    public List<IPersistent> persistents;
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Expected only on Data Persistence Manager in the scene, but found multiple.");
        }
        Instance = this;
    }

    private void Start()
    {
        ScriptableObject[] objects = FindObjectsOfType<ScriptableObject>();
        print($"Scriptable objects: {objects.Length}");
        IEnumerable<IPersistent> iePersistents = objects.OfType<IPersistent>();
        print($"IE Persistents: {iePersistents.Count()}");
        persistents = iePersistents.ToList();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData(persistents);
    }

    public void LoadGame()
    {
        // TODO - Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        // if no data can be loaded, intiailze to a new game
        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }

        // TODO - push the loaded data to all other scripts that need it
    }

    public void SaveGame()
    {
        // TODO - pass the data to other scripts so they can update it

        // TODO - save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

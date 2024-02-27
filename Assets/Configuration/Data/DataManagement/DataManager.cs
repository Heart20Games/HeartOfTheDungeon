using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * As per Trevor Mock's video "How to make a Save & Load System in Unity | 2022"
 * Link: https://www.youtube.com/watch?v=aUi9aijvpgs&list=PL3viUl9h9k7-tMGkSApPdu4hlUBagKial&index=2
 */

namespace DataManagement
{
    public class DataManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
    
        private GameData gameData;
        private FileDataHandler dataHandler;

        public List<IPersistent> persistents = new();
        [ReadOnly] public bool initialLoadComplete = false;
        public UnityEvent onInitialLoadComplete;
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
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            LoadGame();
        }

        public void NewGame()
        {
            this.gameData = new GameData();
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
            this.gameData.Initialize();
            LoadPersistents(persistents);

            initialLoadComplete = true;
            onInitialLoadComplete.Invoke();
        }

        public void SaveGame()
        {
            // Stop trying to save if it's not possible anyway.
            if (dataHandler == null)
            {
                Debug.LogWarning("No data handler found, aborting Save Game.");
                return;
            }

            // TODO - pass the data to other scripts so they can update it
            SavePersistents(persistents);

            // TODO - save that data to a file using the data handler
            dataHandler.Save(gameData);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        // IPersistent Registry
        public void RegisterPersistent(IPersistent persistent)
        {
            if (!persistents.Contains(persistent))
            {
                persistents.Add(persistent);
                if (initialLoadComplete)
                {
                    LoadPersistent(persistent);
                }
            }
        }

        public void LoadPersistent(IPersistent persistent)
        {
            if (persistent != null)
            {
                List<IData> data = persistent.GetData();
                foreach (IData datum in data)
                {
                    if (!datum.LoadData(this.gameData))
                        datum.RegisterOn(this.gameData);
                }
                persistent.LoadFromData();
            }
        }

        public void LoadPersistents(List<IPersistent> persistents)
        {
            if (persistents != null)
            {
                foreach (IPersistent persistent in persistents)
                {
                    LoadPersistent(persistent);
                }
            }
        }

        public void SavePersistent(IPersistent persistent)
        {
            if (persistent != null)
            {
                persistent.SaveToData();
                List<IData> data = persistent.GetData();
                foreach ( IData datum in data)
                {
                    if (!datum.SaveData(this.gameData))
                        datum.RegisterOn(this.gameData);
                }
            }
        }

        public void SavePersistents(List<IPersistent> persistents)
        {
            if (persistents != null)
            {
                foreach (IPersistent persistent in persistents)
                {
                    SavePersistent(persistent);
                }
            }
        }
    }
}

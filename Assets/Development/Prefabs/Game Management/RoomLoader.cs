using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class RoomLoader : BaseMonoBehaviour
{
    public InMemoryVariableStorage storage;
    public string targetRoom = "$Hub";
    public bool asynchronous = false;
    public float progressThreshold = 0.9f;
    public bool canActivate = true;
    public UnityEvent onSceneLoaded;

    public bool debug = false;

    private AsyncOperation loading;

    private void Start()
    {
        if (storage == null)
        {
            storage = FindObjectOfType<InMemoryVariableStorage>();
            if (storage == null)
                Debug.LogWarning("Can't find InMemoryVariableStorage componenent in scene.");
        }
    }

    private void Update()
    {
        if (loading != null)
        {
            if (debug) print($"Progress: {loading.progress}");
            if (canActivate && (loading.isDone || loading.progress >= progressThreshold))
            {
                if (debug) print("Scene done loading, activation allowed.");
                loading.allowSceneActivation = true;
                onSceneLoaded.Invoke();
            }
        }
    }

    public void SetCanActivate(bool canActivate)
    {
        this.canActivate = canActivate;
    }

    public void Load()
    {
        Load(targetRoom);
    }

    public void Load(string targetRoom)
    {
        if (debug) print("Game starts here!");
        if (targetRoom.StartsWith("$") && storage != null)
        {
            LoadRoom(targetRoom);
        }
        else
        {
            if (targetRoom.StartsWith("$"))
            {
                Debug.LogWarning("No InMemoryVariableStorage componenent.");
            }
            LoadScene(targetRoom);
        }
    }

    public void LoadRoom(string targetRoom)
    {
        if (storage.TryGetValue(targetRoom, out string targetScene))
        {
            LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Can't find room " + targetRoom + ", using as Scene name instead.");
            LoadScene(targetRoom);
        }
    }

    public void LoadScene(string targetScene)
    {
        if (asynchronous)
        {
            if ((loading = SceneManager.LoadSceneAsync(targetScene)) != null)
                loading.allowSceneActivation = false;
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}

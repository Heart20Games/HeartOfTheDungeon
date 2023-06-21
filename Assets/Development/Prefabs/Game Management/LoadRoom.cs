using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class LoadRoom : BaseMonoBehaviour
{
    public InMemoryVariableStorage storage;
    public string targetRoom = "$Hub";
    public bool asynchronous = false;
    public float asyncProgressThreshold = 0.9f;
    public UnityEvent onSceneLoaded;

    public bool debug = false;

    private AsyncOperation loading;

    private void Update()
    {
        if (debug && loading != null)
        {
            if (debug) print($"Progress: {loading.progress}");
            if (loading.isDone || loading.progress >= 0.9f)
            {
                if (debug) print("Scene done loading, activation allowed.");
                loading.allowSceneActivation = true;
                onSceneLoaded.Invoke();
            }
        }
    }

    public void StartGameplay()
    {
        if (debug) print("Game starts here!");
        if (targetRoom.StartsWith("$") && storage != null)
        {
            string targetScene;
            if (storage.TryGetValue(targetRoom, out targetScene))
            {
                LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning("Can't find room " + targetRoom + ", using as Scene name instead.");
                LoadScene(targetRoom);
            }
        }
        else
        {
            if (storage == null)
            {
                Debug.LogWarning("Cannot find InMemoryVariableStorage componenent.");
            }
            LoadScene(targetRoom);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (asynchronous)
        {
            loading = SceneManager.LoadSceneAsync(sceneName);
            loading.allowSceneActivation = false;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

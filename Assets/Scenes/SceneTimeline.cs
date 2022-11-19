using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class SceneTimeline : MonoBehaviour
{
    private PlayableDirector director;
    public List<PlayableAsset> timelines = new List<PlayableAsset>();

    public void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    [YarnCommand("cutscene")]
    public void Trigger(int index)
    {
        if (index > 0 && index < timelines.Count)
        {
            director.Stop();
            director.playableAsset = timelines[index];
            director.Play();
        }
        else
        {
            Debug.LogWarning("SceneTimeline index out of bounds: " + index);
        }
    }
}

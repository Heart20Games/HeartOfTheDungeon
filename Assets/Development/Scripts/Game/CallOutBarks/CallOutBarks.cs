using UnityEngine;
using FMODUnity;

public class CallOutBarks : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter callOutEmitter;

    [SerializeField] private EventReference[] callOutEvents;

    public void PlayBark(int callIndex)
    {
        switch(callIndex)
        {
            case 0:
                Debug.Log(gameObject.name + ": Small missile incoming!");
                //callOutEmitter.EventReference = callOutEvents[0];
                //callOutEmitter.Play();
                break;
            case 1:
                Debug.Log(gameObject.name + ": Medium missile incoming!");
                //callOutEmitter.EventReference = callOutEvents[1];
                //callOutEmitter.Play();
                break;
            case 2:
                Debug.Log(gameObject.name + ": Large missile incoming!");
                //callOutEmitter.EventReference = callOutEvents[2];
                //callOutEmitter.Play();
                break;
            default:
                Debug.Log(gameObject.name + ": Small missile incoming!");
                //callOutEmitter.EventReference = callOutEvents[0];
                //callOutEmitter.Play();
                break;
        }
    }
}
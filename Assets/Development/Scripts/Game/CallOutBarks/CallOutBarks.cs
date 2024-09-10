using UnityEngine;
using FMODUnity;

public class CallOutBarks : MonoBehaviour
{
    [SerializeField] private EventReference[] callOutEvents;

    public void PlayBark(int callIndex)
    {
        switch(callIndex)
        {
            case 0:
                RuntimeManager.PlayOneShotAttached(callOutEvents[0], gameObject);
                break;
            case 1:
                RuntimeManager.PlayOneShotAttached(callOutEvents[1], gameObject);
                break;
            case 2:
                RuntimeManager.PlayOneShotAttached(callOutEvents[2], gameObject);
                break;
            default:
                RuntimeManager.PlayOneShotAttached(callOutEvents[0], gameObject);
                break;
        }
    }
}
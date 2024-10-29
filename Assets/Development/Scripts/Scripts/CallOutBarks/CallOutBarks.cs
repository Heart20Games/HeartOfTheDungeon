using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using Articy.Unity;

public class CallOutBarks : MonoBehaviour
{
    [SerializeField] private EventReference[] callOutEvents;

    [SerializeField] private ArticyReference articyReference;

    [SerializeField] private UnityEvent playerDamaged;

    private ArticyObject availableDialogue;

    private void Start()
    {
        //availableDialogue = articyReference.reference.GetObject();
        //playerDamaged.AddListener(() => DialogueManager.Instance.StartDialogue(availableDialogue));
    }

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
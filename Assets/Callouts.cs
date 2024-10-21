using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICallouts
{
    public void TriggerCallout(int idx);
}

public class Callouts : MonoBehaviour, ICallouts
{
    public static ICallouts main;

    private MyDialogueHandler dialogueHandler;

    private void Awake()
    {
        dialogueHandler = GetComponent<MyDialogueHandler>();
        main = this;
    }
   
    public void TriggerCallout(int idx)
    {
        dialogueHandler.SetTo(idx);
    }

    [SerializeField] private int testIdx = 3;
    [ButtonMethod]
    public void TestCallout()
    {
        Callouts.main?.TriggerCallout(testIdx);
    }
}

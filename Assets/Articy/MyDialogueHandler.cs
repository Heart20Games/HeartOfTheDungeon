using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using Articy.Unity.Interfaces;
using TMPro;
using System;

public class MyDialogueHandler : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    private ArticyFlowPlayer flowPlayer;

    [Serializable]
    public struct ArticyObjectWrapper
    {
        [SerializeField]
        [ArticyTypeConstraint(new Type[]
        {
            typeof(IDialogue),
            typeof(IFlowFragment),
            typeof(IHub),
            typeof(ICondition),
            typeof(IInstruction),
            typeof(IDialogueFragment)
        })]
        internal ArticyRef articyRef;
    }

    [SerializeField]
    [ArticyTypeConstraint(new Type[]
    {
        typeof(IDialogue),
        typeof(IFlowFragment),
        typeof(IHub),
        typeof(ICondition),
        typeof(IInstruction),
        typeof(IDialogueFragment)
    })]
    internal ArticyRef startOn;

    [SerializeField] private List<ArticyObjectWrapper> changeTos = new();
    public TMP_Text descriptionLabel;

    public bool useHandlerNodes = false;

    private void Awake()
    {
        flowPlayer = GetComponent<ArticyFlowPlayer>();
    }

    private void Start()
    {
        if (useHandlerNodes)
        {
            //Callouts.main?.TriggerCallout(3);
            //flowPlayer.StartOn = startOn.GetObject<ArticyObject>();
            //StartCoroutine(ChangeToAfterSeconds(2));
        }
    }

    public void SetTo(int idx)
    {
        if (idx < changeTos.Count)
        {
            flowPlayer.StartOn = changeTos[idx].articyRef.GetObject<ArticyObject>();
        }
    }

    private IEnumerator ChangeToAfterSeconds(float seconds)
    {
        foreach (ArticyObjectWrapper changeTo in changeTos)
        {
            yield return new WaitForSeconds(seconds);
            flowPlayer.StartOn = changeTo.articyRef.GetObject<ArticyObject>();
        }
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        var objWithText = aObject as IObjectWithLocalizableText;
        
        if(objWithText != null)
        {
            Debug.Log($"Line: {objWithText.Text}");
            descriptionLabel.text = objWithText.Text;
        }
    }
}

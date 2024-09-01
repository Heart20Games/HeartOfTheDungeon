using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using Articy.Unity.Interfaces;
using TMPro;

public class MyDialogueHandler : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
    }
    public TMP_Text descriptionLabel;

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        var objWithText = aObject as IObjectWithLocalizableText;
        if(objWithText != null)
        {
            descriptionLabel.text = objWithText.Text;
        }
    }
}

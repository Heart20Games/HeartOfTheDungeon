using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using Articy.Unity.Interfaces;

public class MyDialogueHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
    }
    public UnityEngine.UI.Text descriptionLabel;

public void OnFlowPlayerPaused(IFlowObject aObject)
{
    var objWithText = aObject as IObjectWithLocalizableText;
    if(objWithText != null)
    {
        descriptionLabel.text = objWithText.Text;
    }
}
}

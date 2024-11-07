using System.Collections.Generic;
using UnityEngine;
using Articy.Unity;
using Articy.Unity.Interfaces;
using Articy.Heart_Of_The_Dungeon_Prologue;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class DialogueManager : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    public static DialogueManager Instance = null;

    [Header("UI")]
    [SerializeField] private GameObject dialogueWidget;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    [SerializeField] public StudioEventEmitter calloutsAudio;

    public bool DialogueActive { get; set; }
    private bool lookForStop = false;
    private EventInstance calloutInstance;
    private ArticyString articyStageDirection;
    private IObjectWithStageDirections stageDirectionObject;
    private string stageDirectionString;

    private ArticyFlowPlayer flowPlayer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        flowPlayer = GetComponent<ArticyFlowPlayer>();
    }

    void Update()
    {
        if (lookForStop)
        {
            calloutInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED)
            {
                CloseDialogueBox();
                lookForStop = false;
            }
        }
    }

    public void StartDialogue(IArticyObject aObject)
    {
        DialogueActive = true;
        dialogueWidget.SetActive(DialogueActive);
        flowPlayer.StartOn = aObject;
        stageDirectionObject = aObject as IObjectWithStageDirections;
    }

    public void CloseDialogueBox()
    {
        DialogueActive = false;
        dialogueWidget.SetActive(DialogueActive);
    }

    // This is called every time the flow player reaches an object of interest
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        //Clear data
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;

        // If we paused on an object that has a "Text" property fetch this text and present it        
        var objectWithText = aObject as IObjectWithLocalizableText;
        if (objectWithText != null)
        {
            Debug.Log(objectWithText.Text);
            dialogueText.text = objectWithText.Text;
        }
        else
        {
            Debug.Log("NULL");
        }

        // If the object has a "Speaker" property try to fetch the speaker
        var objectWithSpeaker = aObject as IObjectWithSpeaker;
        if (objectWithSpeaker != null)
        {
            // If the object has a "Speaker" property, fetch the reference
            // and ensure it is really set to an "Entity" object to get its "DisplayName"
            var speakerEntity = objectWithSpeaker.Speaker as Entity;
            if (speakerEntity != null)
            {
                dialogueSpeaker.text = speakerEntity.DisplayName;
            }
        }

        Debug.Log(stageDirectionObject);

        if (stageDirectionObject != null)
        {
            articyStageDirection = stageDirectionObject.StageDirections;
            stageDirectionString = articyStageDirection.ToString();
            //string letter = "#";
            /*if (stageDirectionString.Contains(letter))
            {
                stageDirectionString.Replace(letter, "");
                
            }*/
            Debug.Log(stageDirectionString);
            PlayCalloutAudio(stageDirectionString);
        }
        else
        {
            //PlayCalloutAudio("DBO1");
            StartCoroutine(WaitToCloseDialogue());
        } 
    }

    private IEnumerator WaitToCloseDialogue()
    {
        yield return new WaitForSeconds(5);
        CloseDialogueBox();
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {

    }

    private void PlayCalloutAudio(string articyId)
    {
        string clean = articyId.Remove(0, 1);
        Debug.Log(clean);
        calloutInstance = calloutsAudio.EventInstance;
        //calloutsAudio.Stop();
        ///RuntimeManager.StudioSystem.setParameterByNameWithLabel("WizardDuelCallouts", articyId);
        calloutInstance.setParameterByNameWithLabel("WizardDuelCallouts", clean, false);
        calloutInstance.start();
        lookForStop = true;
    }
}
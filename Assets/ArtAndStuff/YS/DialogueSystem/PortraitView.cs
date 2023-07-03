using System.Collections;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static YarnTags;

public class PortraitView : DialogueViewBase, IViewable
{
    public readonly ViewType viewType = ViewType.Portrait;

    // Dictionary of Name:Image Pairs
    [SerializeField] Portraits portraits;

    public string emotionTag = "emotion";
    public string defaultEmotion = "neutral";

    // UI Images to Update with Characters
    [SerializeField] public Image leftImage;
    [SerializeField] public Image rightImage;

    string lastCharacter = "";
    bool leftNext = false;
    private Inclusion viewable = Inclusion.NA;

    //Action advanceHandler = null;

    public void Awake()
    {
        portraits.Initialize();
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false || !ShouldIncludeView(dialogueLine.Metadata, viewType, viewable))
        {
            onDialogueLineFinished();
            return;
        }

        string character = dialogueLine.CharacterName;
        if (portraits.bank.ContainsKey(character))
        {
            bool sameCharacter = lastCharacter == character;

            HasPairTag(dialogueLine.Metadata, emotionTag, out string emotion, defaultEmotion);
            
            PortraitImage portrait = portraits.bank[character][emotion];
            bool orientation = portrait.orientation;
            if (!sameCharacter)
            {
                leftNext = !leftNext;
            }
            Image image = leftNext ? leftImage : rightImage;
            int xScale = (leftNext && !orientation) || (!leftNext && orientation) ? -1 : 1;
            image.rectTransform.localScale = new Vector2(xScale, 1);
            image.sprite = portrait.image;
            image.color = Color.white;
            lastCharacter = character;
        }

        onDialogueLineFinished();
        //base.RunLine(dialogueLine, onDialogueLineFinished);
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDismissalComplete();
            return;
        }

        leftImage.sprite = null;
        rightImage.sprite = null;
        leftImage.color = Color.clear;
        rightImage.color = Color.clear;

        onDismissalComplete();

        //base.DismissLine(onDismissalComplete);
    }

    public void SetViewable(Inclusion viewable)
    {
        this.viewable = viewable;
    }

    public ViewType GetViewType()
    {
        return viewType;
    }
}

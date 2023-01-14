using System.Collections;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PortraitView : DialogueViewBase
{

    // Dictionary of Name:Image Pairs
    [SerializeField] Portraits portraits;

    // UI Images to Update with Characters
    [SerializeField] public Image leftImage;
    [SerializeField] public Image rightImage;

    string lastCharacter = "";
    bool leftNext = false;

    //Action advanceHandler = null;

    public void Awake()
    {
        portraits.Initialize();
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDialogueLineFinished();
            return;
        }

        string character = dialogueLine.CharacterName;
        if (portraits.bank.ContainsKey(character))
        {
            bool sameCharacter = lastCharacter == character;
            
            Portrait portrait = portraits.bank[character];
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
}

using System.Collections;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CharacterView : DialogueViewBase
{

    // Dictionary of Name:Image Pairs
    [SerializeField] List<string> characters;
    [SerializeField] List<Sprite> portraits;
    [SerializeField] List<bool> orientations;
    Dictionary<string, Sprite> characterPortraits = new Dictionary<string, Sprite>();
    Dictionary<string, bool> characterOrientations = new Dictionary<string, bool>();

    // UI Images to Update with Characters
    [SerializeField] Image leftImage;
    [SerializeField] Image rightImage;

    string lastCharacter = "";
    bool leftNext = false;

    //Action advanceHandler = null;

    public void Awake()
    {
        int count = Mathf.Min(characters.Count, portraits.Count, orientations.Count);
        for (int i = 0; i < count; i++)
        {
            string character = characters[i];
            Sprite portrait = portraits[i];
            bool orientation = orientations[i];
            characterPortraits.Add(character, portrait);
            characterOrientations.Add(character, orientation);
        }
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (gameObject.activeInHierarchy == false)
        {
            onDialogueLineFinished();
            return;
        }

        string character = dialogueLine.CharacterName;
        if (characterPortraits.ContainsKey(character))
        {
            bool sameCharacter = lastCharacter == character;
            Sprite portrait = characterPortraits[character];
            bool orientation = characterOrientations[character];
            if (!sameCharacter)
            {
                leftNext = !leftNext;
            }
            Image image = leftNext ? leftImage : rightImage;
            int xScale = (leftNext && !orientation) || (!leftNext && orientation) ? -1 : 1;
            image.rectTransform.localScale = new Vector2(xScale, 1);
            image.sprite = portrait;
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

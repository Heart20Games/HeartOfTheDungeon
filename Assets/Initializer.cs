using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Initializer : MonoBehaviour
{
    private Character[] characters;
    private DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        characters = FindObjectsOfType<Character>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        foreach (Character character in characters)
        {
            Interactor interactor = character.GetComponent<Interactor>();
            if (interactor != null )
            {
                interactor.dialogueRunner = dialogueRunner;
            }
        }
    }
}

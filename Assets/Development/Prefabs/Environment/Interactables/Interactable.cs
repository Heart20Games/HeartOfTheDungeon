using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string yarnNode;
    public Character player;

    public void NotifyPlayerEnteredInteractable()
    {
        player.interactor.FoundTalkable(yarnNode);
    }

    public void NotifyPlayerLeftInteractable()
    {
        player.interactor.LeftTalkable(yarnNode);
    }
}

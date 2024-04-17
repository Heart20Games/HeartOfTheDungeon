using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIPips;
using Body;
using HotD.Body;

public class AllyHealthDisplay : MonoBehaviour
{
    public PipGenerator generator;
    public Character character;

    public Character Character { get => character; set => SetCharacter(value); }

    private void Awake()
    {
        if (generator == null)
        {
            generator = GetComponentInChildren<PipGenerator>();
        }
    }

    public void SetCharacter(Character character)
    {
        if (generator != null)
        {
            this.character.DisconnectPips(generator);
            character.ConnectPips(generator, true);
        }
        this.character = character;
    }
}

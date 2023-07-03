using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Portraits", order = 1)]
public class Portraits : ScriptableObject
{
    public Portrait[] portraits = new Portrait[5];
    public Dictionary<string, Dictionary<string, PortraitImage>> bank = new();

    public void Initialize()
    {
        bank.Clear();
        foreach (Portrait portrait in portraits)
        {
            bank[portrait.name] = new();
            foreach (var image in portrait.images)
            {
                bank[portrait.name][image.name] = image;
            }
        }
    }
}

using System;
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

    public PortraitImage GetPortrait(string name, string emotion)
    {
        if (portraits != null && bank.TryGetValue(name, out var emotions))
            if (emotions.TryGetValue(emotion, out PortraitImage portrait))
                return portrait;
        return new();
    }

    public Sprite GetImage(string name, string emotion)
    {
        return GetPortrait(name, emotion).image;
    }
}

[Serializable]
public struct PortraitImage
{
    public PortraitImage(string _name, Sprite _image, bool _orientation)
    {
        name = _name;
        image = _image;
        orientation = _orientation;
    }
    public string name;
    public Sprite image;
    public bool orientation;
}

[Serializable]
public struct Portrait
{
    public Portrait(string _name, PortraitImage[] _images)
    {
        name = _name;
        images = _images;
    }
    public string name;
    public PortraitImage[] images;
}

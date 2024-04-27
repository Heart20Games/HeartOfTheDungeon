using MyBox;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Portraits", order = 1)]
public class Portraits : ScriptableObject
{
    public static Portraits main;
    public Portrait[] portraits = new Portrait[5];
    public Dictionary<string, Dictionary<string, PortraitImage>> bank = new();

    public bool debug = false;

    private void OnEnable()
    {
        main = this;
    }

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

    [ButtonMethod]
    public void TestPrintBank()
    {
        Debug.Log("Test-Printing Portrait Bank.");
        foreach (var key in bank.Keys)
        {
            Debug.Log($"Key: {key}");
            foreach (var subKey in bank[key].Keys)
            {
                var pi = bank[key][subKey];
                Debug.Log($"- {pi.name} / {pi.image.name} / {pi.orientation}");
            }
        }
    }

    public PortraitImage GetPortrait(string name, string emotion)
    {
        name = name == null ? "" : name;
        if (debug) Debug.Log($"Getting Portrait by {name} and {emotion}.", this);
        if (portraits != null && bank.TryGetValue(name, out var emotions))
        {
            if (debug) Debug.Log($"Got emotion list.");
            if (emotions.TryGetValue(emotion, out PortraitImage portrait))
            {
                if (debug) Debug.Log($"Found portrait/image pair.");
                return portrait;
            }
            else
            {
                if (debug) Debug.Log("No portrait/image pair found.");
            }
        }
        else
        {
            if (debug) Debug.Log("No valid emotion list found.");
        }
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

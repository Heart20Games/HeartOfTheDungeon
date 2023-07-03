using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

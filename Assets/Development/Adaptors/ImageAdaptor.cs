using UnityEngine;

public class ImageAdaptor : Adaptor
{
    public EventAdaptor<Sprite> sprite;
    public EventAdaptor<Color> color;

    public Sprite Sprite { set { sprite.Value = value; } }
    public Color Color { set { color.Value = value; } }

    private void Update()
    {
        sprite.SendChanges();
        color.SendChanges();
    }
}

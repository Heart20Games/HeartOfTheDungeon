using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSpriteSwap : BaseMonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;

    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite selectSprite;

    private bool selected = false;

    private void Awake()
    {
        button ??= GetComponent<Button>();
        image ??= GetComponent<Image>();
    }

    private void Update()
    {
        var selected = (EventSystem.current.currentSelectedGameObject == button.gameObject);
        if (this.selected != selected)
        {
            this.selected = selected;
            image.sprite = selected ? selectSprite : baseSprite;
        }
    }
}

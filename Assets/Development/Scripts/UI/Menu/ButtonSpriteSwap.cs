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
        button = button != null ? button : GetComponent<Button>();
        image = image != null ? image : GetComponent<Image>();
    }

    private void OnEnable()
    {
        UpdateSelection(true);
    }

    private void Update()
    {
        UpdateSelection();
    }

    private void UpdateSelection(bool alwaysSet = false)
    {
        var selected = (EventSystem.current.currentSelectedGameObject == button.gameObject);
        if (alwaysSet || this.selected != selected)
        {
            SetSelected(selected);
        }
    }

    private void SetSelected(bool selected)
    {
        this.selected = selected;
        image.sprite = selected ? selectSprite : baseSprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPrefab;
    private TextMeshProUGUI textObject;
    private Animator healthPipTextAnimator;
    [SerializeField] private Color decreaseColor = Color.red;
    [SerializeField] private Color increaseColor = Color.green;

    private void Start()
    {
        CreateText();
    }

    private void CreateText()
    {
        var pipText = Instantiate(textPrefab, transform);

        pipText.text = "";
        pipText.gameObject.SetActive(false);

        textObject = pipText;
        healthPipTextAnimator = pipText.GetComponent<Animator>();
    }

    public void ShowText(int value)
    {
        bool decreasing = value < 0;
        string text = (decreasing ? "-" : "+") + value;
        Color color = decreasing ? decreaseColor : increaseColor;
        ShowText(text, color);
    }

    public void ShowText(string text, Color color)
    {
        if (textObject == null) return;

        textObject.gameObject.SetActive(true);

        healthPipTextAnimator.Play("FadeIn");

        textObject.text = text;
        textObject.color = color;
    }
}

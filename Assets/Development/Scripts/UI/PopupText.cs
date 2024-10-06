using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrefab;
    private TMP_Text textObject;
    private Animator textAnimator;
    [SerializeField] private Color decreaseColor = Color.red;
    [SerializeField] private Color zeroColor = Color.white;
    [SerializeField] private Color increaseColor = Color.green;

    [Header("Testing")]
    [SerializeField] int testNumber = 0;

    private bool shouldDisplayNumber;

    private void Start()
    {
        CreateText();
    }

    private void OnEnable()
    {
        if (textObject)
            textObject.gameObject.SetActive(false);
    }

    private void CreateText()
    {
        var pipText = Instantiate(textPrefab, transform);

        pipText.text = "";
        pipText.gameObject.SetActive(false);

        textObject = pipText;
        textAnimator = pipText.GetComponent<Animator>();
    }

    public void ShowText(int value) { ShowText<int>(value); }
    public void ShowText(float value) { ShowText<float>(value); }
    public void ShowText<T>(T value) where T : IComparable
    {
        float sign = value.CompareTo(default(T));
        string text = (sign == 0 ? " " : sign < 0 ? "" : "+") + value;
        Color color = sign == 0 ? zeroColor : sign < 0 ? decreaseColor : increaseColor;
        ShowText(text, color);
    }

    public void ShowText(string text, Color color)
    {
        if (textObject == null) return;

        if(shouldDisplayNumber)
        {
            textObject.gameObject.SetActive(true);

            textAnimator.SetTrigger("FadeIn"); // textAnimator.Play("FadeIn");

            textObject.text = text;
            textObject.color = color;
        }

        shouldDisplayNumber = true;
    }

    // Testing
    [ButtonMethod]
    public void TestShowText()
    {
        ShowText(testNumber);
    }
}

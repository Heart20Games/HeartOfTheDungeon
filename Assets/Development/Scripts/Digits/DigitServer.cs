using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DigitServer : BaseMonoBehaviour
{
    public DigitLibrary library;

    public string digit1Property;
    public UnityEvent<string, Texture> onDigit1;

    public string digit2Property;
    public UnityEvent<string, Texture> onDigit2;

    public string digit3Property;
    public UnityEvent<string, Texture> onDigit3;

    public string evenProperty;
    public UnityEvent<string, bool> onEven;

    public bool serveWithSign = false;

    public void ServeNumber(int number)
    {
        if (serveWithSign)
        {
            ServeNumberWithSign(number);
        }
        else
        {
            int hundreds = number / 100;
            int remainder = number % 100;
            if (hundreds > 0)
                onDigit3.Invoke(digit3Property, library.GetNumber(hundreds).texture);

            ServeTensAndOnes(Mathf.Abs(remainder), hundreds > 0);
        }
    }

    public void ServeNumberWithSign(int number)
    {
        if ((number / 100) > 0)
        {
            ServeNumber(number);
        }
        else
        {
            Sprite sprite = library.GetSign(number);
            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                onDigit2.Invoke(digit2Property, texture);
                onDigit3.Invoke(digit3Property, texture);
            }

            ServeTensAndOnes(Mathf.Abs(number), false, true);
        }
    }

    private void ServeTensAndOnes(int number, bool hasThirdDigit = false, bool hasSign = false)
    {
        int tens = number / 10;
        if (tens > 0 || hasThirdDigit)
            onDigit2.Invoke(digit2Property, library.GetNumber(tens).texture);
        else if (!hasSign)
            onDigit2.Invoke(digit2Property, null);

        int ones = number % 10;
        onDigit1.Invoke(digit1Property, library.GetNumber(ones).texture);

        onEven.Invoke(evenProperty, !hasThirdDigit && tens > 0); //((tens == 0 && hasSign) || (tens > 0 && !hasSign)));
    }
}

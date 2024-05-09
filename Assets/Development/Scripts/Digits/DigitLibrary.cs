using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DigitLibrary", menuName = "DigitLibrary", order = 1)]
public class DigitLibrary : BaseScriptableObject
{
    public Sprite[] numbers;
    public Sprite plus;
    public Sprite minus;

    public Sprite GetNumber(int number)
    {
        return numbers[number];
    }

    public Sprite GetSign(int sign)
    {
        return sign < 0 ? minus : sign > 0 ? plus : null;
    }
}

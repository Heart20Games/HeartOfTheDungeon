using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    public TMP_Text loadingText;
    public float animationSpeed = 0.5f;

    private readonly string[] loadingDots = { "", ".", "..", "..." };
    private int currentDotIndex = 0;
    private float timer = 0f;

    private void Awake()
    {
        if (loadingText == null)
            loadingText = GetComponent<TMP_Text>();
        loadingText.text = "Loading";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= animationSpeed)
        {
            timer = 0f;
            currentDotIndex = (currentDotIndex + 1) % loadingDots.Length;
            loadingText.text = "Loading" + loadingDots[currentDotIndex];
        }
    }
}
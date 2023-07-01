using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectPanel : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material defaultSpriteMat;
    [SerializeField] private Material shimmerMat;
    [SerializeField] private float shimmerSpeed = .02f;
    private bool isShimmering = false;

    [Header("Portrait Images")]
    [SerializeField] private List<GameObject> characterImages;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private int initialIndex;
    private int latestIndex;

    private void Awake()
    {
        Initialize(initialIndex);
    }

    private void FixedUpdate()
    {
        if (isShimmering)
        {
            Shimmer(latestIndex);
        }
    }

    public void Initialize(int idx)
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            if (i == idx)
                SelectImage(i);
            else
                DeSelectImage(i);
        }
    }


    // Selection

    public void Select(int idx)
    {
        // Swap Images
        idx %= characterImages.Count;
        DeSelectImage(latestIndex);
        SelectImage(idx);
    }

    public void SelectImage(int idx)
    {
        GameObject image = characterImages[idx];
        image.GetComponent<SpriteRenderer>().sortingOrder = 3;
        image.GetComponent<SpriteRenderer>().material = shimmerMat;
        portraitAnimator.SetTrigger($"SelectCharacter{idx}");
        latestIndex = idx;

        // Reset Shimmering
        shimmerMat.SetFloat("_SheenPosition", 0f);
        isShimmering = true;
    }

    public void DeSelectImage(int idx)
    {
        GameObject image = characterImages[idx];
        image.GetComponent<SpriteRenderer>().sortingOrder = 1;
        image.GetComponent<SpriteRenderer>().material = defaultSpriteMat;
    }


    // Shimmer
    public void Shimmer(int idx)
    {
        float endingPos = -.9f;
        float currentPos = shimmerMat.GetFloat("_SheenPosition");
        if (currentPos >= endingPos)
            shimmerMat.SetFloat("_SheenPosition", currentPos - shimmerSpeed);
        else
            isShimmering = false;
    }
}

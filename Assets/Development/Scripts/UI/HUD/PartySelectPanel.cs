using System.Collections.Generic;
using UnityEngine;

public class PartySelectPanel : BaseMonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material defaultSpriteMat;
    [SerializeField] private Material shimmerMat;
    [SerializeField] private float shimmerSpeed = .02f;
    private bool isShimmering = false;

    [Header("Portrait Images")]
    [SerializeField] private List<TargetStatusDisplay> displays;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private int initialIndex;

    [SerializeField] private bool debug;

    private void Awake()
    {
        Initialize(initialIndex);
    }

    private void FixedUpdate()
    {
        if (isShimmering)
        {
            Shimmer();
        }
    }

    public void Initialize(int idx)
    {
        for (int i = 0; i < displays.Count; i++)
        {
            if (i == idx)
                SelectImage(i);
            else
                UnSelectImage(i);
        }
    }

    public void SetTarget(int idx, IIdentifiable identifiable=null)
    {
        Print($"Set Target for portrait display {idx} to {identifiable?.Name}", debug);
        displays[idx].SetTarget(identifiable);
    }


    // Selection

    public void Select(int idx)
    {
        Print($"Select portrait display {idx}", debug);
        // Swap Images
        idx %= displays.Count;
        for (int i = 0; i < displays.Count; i++)
        {
            if (i != idx)
                UnSelectImage(i);
        }
        SelectImage(idx);
    }

    public void SelectImage(int idx)
    {
        TargetStatusDisplay image = displays[idx];
        image.portrait.sortingOrder = 3;
        image.portrait.material = shimmerMat;
        portraitAnimator.SetTrigger($"SelectCharacter{idx}");

        // Reset Shimmering
        shimmerMat.SetFloat("_SheenPosition", 0f);
        isShimmering = true;
    }

    public void UnSelectImage(int idx)
    {
        TargetStatusDisplay image = displays[idx];
        image.portrait.sortingOrder = 1;
        image.portrait.material = defaultSpriteMat;
    }


    // Shimmer
    public void Shimmer()
    {
        float endingPos = -.9f;
        float currentPos = shimmerMat.GetFloat("_SheenPosition");
        if (currentPos >= endingPos)
            shimmerMat.SetFloat("_SheenPosition", currentPos - shimmerSpeed);
        else
            isShimmering = false;
    }
}
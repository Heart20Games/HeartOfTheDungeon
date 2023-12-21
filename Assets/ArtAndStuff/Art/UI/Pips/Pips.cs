using MyBox;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Pips : BaseMonoBehaviour
{
    [SerializeField] private Transform pipTarget;
    [SerializeField] private Pip pipPrefab;
    [ReadOnly][SerializeField] private List<Pip> pips = new();
    [SerializeField] private bool lookAtCamera;

    [Header("Pip Counts")]
    [SerializeField] protected int totalPips;
    [SerializeField] protected int filledPips;
    [SerializeField] private bool updatePips = false;
    private int oldPipCount;
    private int oldFilledCount;

    [Foldout("Events")] public UnityEvent<int> onSetTotal;
    [Foldout("Events")] public UnityEvent<int> onSetFilled;
    [Foldout("Events")] public UnityEvent<int> onChanged;

    private void Awake()
    {
        oldPipCount = totalPips;
        oldFilledCount = filledPips;
    }

    private void Start()
    {
        SetPipCount(totalPips);
    }

    private void Update()
    {
        if (updatePips)
        {
            if (totalPips != oldPipCount) SetPipCount(totalPips);
            if (filledPips != oldFilledCount) SetFilled(filledPips);
        }
    }

    private void FixedUpdate()
    {
        if (lookAtCamera)
        {
            transform.TrueLookAt(Camera.main.transform.position);
        }
    }

    public void SetFilled(int filled)
    {
        if (filled > totalPips)
        {
            SetPipCount(filled);
        }
        filledPips = filled;
        for (int i = 0; i < pips.Count; i++)
        {
            pips[i].Filled = i < filled;
        }
        onChanged.Invoke(filledPips - oldFilledCount);
        onSetFilled.Invoke(filledPips);
        oldFilledCount = filledPips;
    }

    protected void ClearPips()
    {
        foreach (Pip pip in pips)
        {
            Destroy(pip.gameObject);
        }
        pips.Clear();
    }

    private void AddPips(int number)
    {
        for (int i = 0; i < number; i++)
        {
            Pip pip = Instantiate(pipPrefab, pipTarget == null ? transform : pipTarget);
            pips.Add(pip);
        }
    }

    private void RemovePips(int number)
    {
        for(int i = pips.Count - number; i < pips.Count; i++)
        {
            if (pips[i] != null)
            {
                if (Application.isEditor)
                    DestroyImmediate(pips[i].gameObject);
                else
                    Destroy(pips[i].gameObject);
            }
        }
        pips.RemoveRange(pips.Count - number, number);
    }

    public void SetPipCount(int total)
    {
        totalPips = total;
        filledPips = Mathf.Min(filledPips, totalPips);
        if (pips.Count > total)
            RemovePips(pips.Count - total);
        else if (pips.Count < total)
            AddPips(total - pips.Count);
        oldPipCount = totalPips;
        onSetTotal.Invoke(totalPips);
    }


}

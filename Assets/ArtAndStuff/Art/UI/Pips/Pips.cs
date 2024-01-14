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
    [SerializeField] private int totalPips = 5;
    [SerializeField] private int filledPips = 5;
    [SerializeField] private bool updatePips = false;
    private int lastPipCount;
    private int lastFilledCount;
    public int TotalPips { get => totalPips; set => SetPipCount(value); }
    public int FilledPips { get => filledPips; set => SetFilled(value); }

    [Foldout("Events")] public UnityEvent<int> onSetTotal;
    [Foldout("Events")] public UnityEvent<int> onSetFilled;
    [Foldout("Events")] public UnityEvent<int> onChanged;

    private void Awake()
    {
        lastPipCount = totalPips;
        lastFilledCount = filledPips;
    }

    private void Start()
    {
        if (updatePips)
        {
            SetPipCount(totalPips);
        }
    }

    private void Update()
    {
        if (updatePips)
        {
            if (totalPips != lastPipCount) SetPipCount(totalPips);
            if (filledPips != lastFilledCount) SetFilled(filledPips);
        }
    }

    private void FixedUpdate()
    {
        if (lookAtCamera)
        {
            transform.TrueLookAt(Camera.main.transform.position);
        }
    }

    public void SetFilled(int filled) { SetFilled(filled, Mool.Maybe); }
    public void SetFilled(int filled, Mool alwaysReport)
    {
        lastFilledCount = filledPips;
        if (filled > totalPips)
        {
            SetPipCount(filled);
        }
        print($"Change: {filledPips}/{lastFilledCount} -> {filled}");
        filledPips = filled;
        for (int i = 0; i < pips.Count; i++)
        {
            pips[i].Filled = i < filled;
        }
        if (filledPips != lastFilledCount || alwaysReport.IsYes && !alwaysReport.IsNo) ReportFill();
        lastFilledCount = filledPips;
    }

    public void ReportFill()
    {
        print($"Report: {lastFilledCount} -> {filledPips} (dif: {filledPips - lastFilledCount})");
        onChanged.Invoke(filledPips - lastFilledCount);
        onSetFilled.Invoke(filledPips);
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
        lastPipCount = totalPips;
        onSetTotal.Invoke(totalPips);
    }


}

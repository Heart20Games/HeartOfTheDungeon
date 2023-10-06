using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class Pips : BaseMonoBehaviour
{
    [SerializeField] private Pip pipPrefab;
    private Canvas canvas;
    [ReadOnly][SerializeField] private List<Pip> pips = new();
    [SerializeField] private bool lookAtCamera;

    [Header("Pip Counts")]
    [SerializeField] protected int totalPips;
    [SerializeField] protected int filledPips;
    [SerializeField] private bool updatePips = false;
    private int oldPipCount;
    private int oldFilledCount;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
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
            Pip pip = Instantiate(pipPrefab, canvas.transform);
            pips.Add(pip);
        }
    }

    private void RemovePips(int number)
    {
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
    }


}

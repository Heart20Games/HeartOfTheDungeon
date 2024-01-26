using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.ProBuilder.Shapes;

namespace UIPips
{
    [ExecuteAlways]
    public class PipGenerator : BaseMonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] public Transform pipTarget;
        [SerializeField] public Pip basePrefab;
        //[ReadOnly][SerializeField] private List<Pip> pips = new();

        [Header("Partitions")]
        public List<PipPartition> partitions;
        //[SerializeField] private int totalPips = 5;
        //[SerializeField] private int filledPips = 5;
        [SerializeField] private bool updatePips = false;
        //public int TotalPips { get => totalPips; set => SetPipCount(value); }
        //public int FilledPips { get => filledPips; set => SetFilled(value); }

        [Header("Features")]
        [SerializeField] private bool lookAtCamera;
        public bool debug = false;
        
        public void SetFilled(int filled, PipType type = PipType.None)
        {
            foreach (var partition in partitions)
            {
                if (partition.Type == type)
                {
                    partition.SetFilled(filled);
                }
            }
        }

        public void SetTotal(int total, PipType type = PipType.None)
        {
            foreach (var partition in partitions)
            {
                if (partition.Type == type)
                {
                    partition.SetTotal(total);
                }
            }
        }

        // For using multiple Pips components on one object.
        //private int PipsIndex { get { return transform.GetComponents<PipGenerator>().IndexOfItem(this); } }
        //private int PipsStartPoint
        //{
        //    get
        //    {
        //        int index = PipsIndex;
        //        if (pips.Count > 0) return pips[0].transform.GetSiblingIndex();
        //        else if (index <= 0) return -1;
        //        else return transform.GetComponents<PipGenerator>()[index - 1].PipsEndPoint;
        //    }
        //}
        //private int PipsEndPoint
        //{
        //    get
        //    {
        //        int index = PipsIndex;
        //        if (pips.Count > 0) return pips[^1].transform.GetSiblingIndex();
        //        else if (index <= 0) return -1;
        //        else return transform.GetComponents<PipGenerator>()[index - 1].PipsEndPoint;
        //    }
        //}

        // Monobehaviour

        private void Awake()
        {
            //lastPipCount = totalPips;
            //lastFilledCount = filledPips;
            //lastGroupCapacity = groupCapacity;
            //lastGroupThreshold = groupThreshold;
            //if (groupPrefab == null) groupPrefab = pipPrefab;
        }

        private void Start()
        {
            if (updatePips)
            {
                //SetPipCount(totalPips);
            }
        }

        private void Update()
        {
            if (updatePips)
            {
                //if (groupCapacity != lastGroupCapacity || groupThreshold != lastGroupThreshold)
                //{
                //    SetPipCount(totalPips);
                //    lastGroupCapacity = groupCapacity;
                //    lastGroupThreshold = groupThreshold;
                //}
                //if (totalPips != lastPipCount) SetPipCount(totalPips);
                //if (filledPips != lastFilledCount)
                //{
                //    SetFilled(filledPips);
                //    SetPipCount(totalPips);
                //}
            }
        }

        private void FixedUpdate()
        {
            if (lookAtCamera)
            {
                transform.TrueLookAt(Camera.main.transform.position);
            }
        }

        // Setters and Fillers

        //public void SetFilled(int filled) { SetFilled(filled, Mool.Maybe); }
        //public void SetFilled(int filled, Mool alwaysReport)
        //{
        //    // Previous Value
        //    lastFilledCount = filledPips;

        //    // Bounds
        //    if (filled > totalPips)
        //    {
        //        if (expandTotalOnFill)
        //            SetPipCount(filled);
        //        else
        //            filled = totalPips;
        //    }
        //    if (filled < 0) filled = 0;

        //    // 
        //    Print($"Change: {filledPips}/{lastFilledCount} -> {filled}", debug);
        //    filledPips = filled;
        //    if (filledPips != lastFilledCount || alwaysReport.IsYes && !alwaysReport.IsNo) ReportFill();
        //    lastFilledCount = filledPips;
        //    for (int i = 0; i < pips.Count; i++)
        //    {
        //        pips[i].Filled = i < filled;
        //    }
        //}

        //public void ShiftFilled()
        //{
        //    int totalFilled = 0;
        //    foreach (var pip in pips)
        //    {

        //    }
        //}

        //public void ReportFill()
        //{
        //    UnHide();
        //    Print($"Report: {lastFilledCount} -> {filledPips} (dif: {filledPips - lastFilledCount})", debug);
        //    int sign = negateFillReports ? -1 : 1;
        //    onChanged.Invoke(sign * (filledPips - lastFilledCount));
        //    onSetFilled.Invoke(filledPips);
        //}

        // Generation

        public void ClearPips(List<Pip> pips)
        {
            foreach (Pip pip in pips)
            {
                if (pip != null)
                {
                    Assert.IsNotNull(pip);
                    if (Application.isEditor)
                        DestroyImmediate(pip.gameObject);
                    else
                        Destroy(pip.gameObject);
                }
                else Debug.LogWarning("Had null Pip in pips list.");
            }
            pips.Clear();
        }
    }
}
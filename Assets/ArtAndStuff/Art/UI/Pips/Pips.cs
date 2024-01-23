using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace UIPips
{
    [ExecuteAlways]
    public class Pips : BaseMonoBehaviour
    {
        [Header("Configuration")]
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

        [Header("Pip Features")]
        public bool negateFillReports = false;
        public bool expandTotalOnFill = false;
        public bool debug = false;

        // Grouping
        public bool useGrouping = false;
        [ConditionalField("useGrouping")][SerializeField] private int groupThreshold = 2;
        [ConditionalField("useGrouping")][SerializeField] private int groupCapacity = 5;
        [ConditionalField("useGrouping")][SerializeField] private Pip groupPrefab;


        // For using multiple Pips components on one object.
        private int PipsIndex { get { return transform.GetComponents<Pips>().IndexOfItem(this); } }
        private int PipsStartPoint
        {
            get
            {
                int index = PipsIndex;
                if (pips.Count > 0) return pips[0].transform.GetSiblingIndex();
                else if (index <= 0) return -1;
                else return transform.GetComponents<Pips>()[index - 1].PipsEndPoint;
            }
        }
        private int PipsEndPoint
        {
            get
            {
                int index = PipsIndex;
                if (pips.Count > 0) return pips[^1].transform.GetSiblingIndex();
                else if (index <= 0) return -1;
                else return transform.GetComponents<Pips>()[index - 1].PipsEndPoint;
            }
        }


        // Monobehaviour

        private void Awake()
        {
            lastPipCount = totalPips;
            lastFilledCount = filledPips;
            if (groupPrefab == null) groupPrefab = pipPrefab;
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

        // Setters and Fillers

        public void SetFilled(int filled) { SetFilled(filled, Mool.Maybe); }
        public void SetFilled(int filled, Mool alwaysReport)
        {
            // Previous Value
            lastFilledCount = filledPips;
            
            // Bounds
            if (filled > totalPips)
            {
                if (expandTotalOnFill)
                    SetPipCount(filled);
                else
                    filled = totalPips;
            }
            if (filled < 0) filled = 0;

            // 
            Print($"Change: {filledPips}/{lastFilledCount} -> {filled}", debug);
            filledPips = filled;
            if (filledPips != lastFilledCount || alwaysReport.IsYes && !alwaysReport.IsNo) ReportFill();
            lastFilledCount = filledPips;
            for (int i = 0; i < pips.Count; i++)
            {
                pips[i].Filled = i < filled;
            }
        }

        public void ShiftFilled()
        {
            int totalFilled = 0;
            foreach (var pip in pips)
            {

            }
        }

        public void ReportFill()
        {
            UnHide();
            Print($"Report: {lastFilledCount} -> {filledPips} (dif: {filledPips - lastFilledCount})", debug);
            int sign = negateFillReports ? -1 : 1;
            onChanged.Invoke(sign * (filledPips - lastFilledCount));
            onSetFilled.Invoke(filledPips);
        }

        protected void ClearPips()
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

        private void AddPips(int number)
        {
            if (useGrouping)
            {
                // Filled
                int fullGroups = number / groupCapacity;
                int remainder = number % groupCapacity;
                for (int i = 0; i < fullGroups; i++)
                {
                    AddPip(groupPrefab, groupCapacity);
                }
                if (remainder > groupThreshold)
                    AddPip(groupPrefab, remainder - groupThreshold);
                for (int i = 0; i < Mathf.Min(remainder, groupThreshold); i++)
                    AddPip(pipPrefab);
            }
            else
            {
                for (int i = 0; i < number; i++)
                {
                    AddPip(pipPrefab);
                }
            }
        }

        private void AddPip(Pip prefab, int amount = 1)
        {
            Pip pip = Instantiate(prefab, pipTarget == null ? transform : pipTarget);
            pip.transform.SetSiblingIndex(PipsEndPoint + 1);
            pips.Add(pip);
            pip.amount = amount;
        }

        private void RemovePips(int number)
        {
            for (int i = pips.Count - number; i < pips.Count; i++)
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
            
            // New Way
            ClearPips();
            AddPips(filledPips);
            AddPips(totalPips - filledPips);

            // Old Way
            //if (pips.Count > total)
            //    RemovePips(pips.Count - total);
            //else if (pips.Count < total)
            //    AddPips(total - pips.Count);
            
            lastPipCount = totalPips;
            onSetTotal.Invoke(totalPips);
        }

        // Hiding

        // Auto-Hide
        public bool autoHide;
        [ConditionalField("autoHide")][SerializeField] private float hideWaitTime;
        [ConditionalField("autoHide")][ReadOnly][SerializeField] private float currentHideTime;
        private Coroutine coroutine;

        public void Hide()
        {
            foreach (var pip in pips)
            {
                pip.gameObject.SetActive(false);
            }
        }

        public void UnHide()
        {
            foreach (var pip in pips)
            {
                pip.gameObject.SetActive(true);
            }

            if (autoHide && gameObject.activeInHierarchy)
            {
                if (coroutine == null)
                    coroutine = StartCoroutine(Deactivate(hideWaitTime));
                else
                    currentHideTime = hideWaitTime;
            }
        }

        public IEnumerator Deactivate(float waitTime)
        {
            currentHideTime = waitTime;
            while (currentHideTime > 0)
            {
                float timeToWait = Mathf.Min(currentHideTime, 1f);
                yield return new WaitForSeconds(timeToWait);
                currentHideTime -= timeToWait;
            }
            Hide();
            coroutine = null;
        }
    }
}
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
    public class PipGenerator : BaseMonoBehaviour
    {
        [Header("Configuration")]
        public Transform pipTarget;
        public AutoPip basePrefab;
        public bool usedInWorldSpace;
        [ConditionalField("usedInWorldSpace")] [SerializeField] private Vector3 worldSpaceOffset = new();
        [ConditionalField("usedInWorldSpace")][SerializeField] private Vector3 worldSpaceScale = new();
        [ConditionalField("usedInWorldSpace")] [SerializeField] private bool lookAtCamera;
        public bool debug = false;
        [SerializeField] private bool updatePips = false;

        [Header("Partitions")]
        public List<PipPartitionSettings> partitionSettings;
        public List<PipPartition> partitions;

        public void SetFilled(int filled, PipType type = PipType.None)
        {
            Print($"Filling {filled} pips on partition {type}.", debug);
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
            Print($"Totaling {total} pips on partition {type}.", debug);
            int childOffset = 0;
            foreach (var partition in partitions)
            {
                if (partition.Type == type)
                {
                    partition.SetTotal(total, childOffset);
                }
                childOffset += partition.pips.Count;
            }
        }

        // Typed Setters
        public void SetHealth(int filled) { SetFilled(filled, PipType.Health); }
        public void SetHealthTotal(int total) { SetTotal(total, PipType.Health); }
        public void SetArmor(int filled) { SetFilled(filled, PipType.Armor); }
        public void SetArmorTotal(int total) { SetTotal(total, PipType.Armor); }

        // Monobehaviour

        private void Awake()
        {
            if (TryGetComponent<Canvas>(out var canvas))
            {
                canvas.renderMode = usedInWorldSpace ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = Camera.current;
                if (usedInWorldSpace)
                {
                    transform.localPosition = worldSpaceOffset;
                    transform.localScale = worldSpaceScale;
                }
            }
            foreach (var settings in partitionSettings)
            {
                partitions.Add(new(settings, this));
            }
        }

        private void Update()
        {
            if (updatePips)
            {
                int childOffset = 0;
                foreach (var partition in partitions)
                {
                    partition.Update(childOffset);
                    childOffset += partition.pips.Count;
                }
            }
        }

        private void FixedUpdate()
        {
            if (lookAtCamera)
            {
                transform.TrueLookAt(Camera.main.transform.position);
            }
        }

        // Generation

        public void ClearPips(List<Pip> pips)
        {
            if (pips != null)
            {
                foreach (Pip pip in pips)
                {
                    if (pip != null)
                    {
                        Assert.IsNotNull(pip);
                        if (Application.isPlaying)
                            Destroy(pip.gameObject);
                        else
                            DestroyImmediate(pip.gameObject);
                    }
                    else Debug.LogWarning("Had null Pip in pips list.");
                }
                pips.Clear();
            }
        }
    }
}
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
        [SerializeField] private bool lookAtCamera;
        public bool debug = false;

        [Header("Partitions")]
        public List<PipPartitionSettings> partitionSettings;
        public List<PipPartition> partitions;
        [SerializeField] private bool updatePips = false;

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

        // Monobehaviour

        private void Awake()
        {
            foreach (var settings in partitionSettings)
            {
                partitions.Add(new(settings, this));
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
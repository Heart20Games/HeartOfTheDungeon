using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIPips
{
    public enum PipType { None, Health, Armor, Skill, Mana }

    [CreateAssetMenu(fileName = "PipPartition", menuName = "PipPartition", order = 1)]
    public class PipPartitionSettings : BaseScriptableObject
    {
        public PipType type = PipType.None;

        [Space]
        public Sprite[] filledSprites;
        public Sprite[] unfilledSprites;

        // Reporting and Changing
        [Space]
        public bool expandTotalOnFill = false;
        public bool negateFillReports = false;

        // Auto-Hide
        [Space]
        public bool autoHide;
        [ConditionalField("autoHide")] public float hideWaitTime;

        // Grouping
        [Space]
        public bool useGrouping = false;
        [ConditionalField("useGrouping")] public int groupThreshold = 2;
        [ConditionalField("useGrouping")] public int groupCapacity = 5;
    }
}
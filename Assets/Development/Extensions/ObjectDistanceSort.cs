using System.Collections.Generic;
using UnityEngine;

namespace Sorting
{
    public class ObjectDistanceSort<T> : Object where T : IBaseMonoBehavior
    {
        // Fields
        private Transform transform;
        private List<T> list;


        // Constructor
        public ObjectDistanceSort() { }
        public ObjectDistanceSort(List<T> list, Transform transform)
        {
            this.transform = transform;
            this.list = list;
        }


        // Start Sorting
        public void Sort(List<T> list, Transform transform)
        {
            this.list = list;
            this.transform = transform;
            Sort();
        }
        public void Sort()
        {
            if (list == null || transform == null)
                Debug.LogError("Object Distance Sort requires both a List and an origin Transform.");
            else
                QuickSort(0, list.Count - 1);
        }


        // Quicksort Algorithm
        private void QuickSort(int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = Partition(left, right);

                if (pivotIndex > 1)
                    QuickSort(left, pivotIndex - 1);

                if (pivotIndex + 1 < right)
                    QuickSort(pivotIndex + 1, right);
            }
        }

        private int Partition(int left, int right)
        {
            float pivotDistance = Vector3.Distance(transform.position, list[right].GetTransform().position);
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                float currentDistance = Vector3.Distance(transform.position, list[j].GetTransform().position);

                if (currentDistance <= pivotDistance)
                {
                    i++;
                    Swap(i, j);
                }
            }

            Swap(i + 1, right);
            return i + 1;
        }


        // Helpers
        private void Swap(int a, int b)
        {
            (list[b], list[a]) = (list[a], list[b]);
        }
    }
}

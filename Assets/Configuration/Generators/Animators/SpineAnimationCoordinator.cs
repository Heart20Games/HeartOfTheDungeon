using CodiceApp.EventTracking.Plastic;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HotD
{
    public class SpineAnimationCoordinator : BaseMonoBehaviour
    {
        [SerializeField] private bool debug;

        public void StartCast()
        {
            Print("OnStartCast (SpineAnimationCoordinator)", debug);
            SendMessageUpwards("OnStartCast");
        }

        public void EndCast()
        {
            Print("OnEndCast (SpineAnimationCoordinator)", debug);
            SendMessageUpwards("OnEndCast");
        }
    }
}
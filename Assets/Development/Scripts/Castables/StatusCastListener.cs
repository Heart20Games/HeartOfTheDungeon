using HotD.Body;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class StatusCastListener : CastListener
    {
        public StatusClass statusClass;

        [SerializeField] private bool debugStatuses = false;

        private void OnEnable()
        {
            ApplyOrRemoveStatuses(true);
        }

        private void OnDisable()
        {
            ApplyOrRemoveStatuses(false);
        }

        private void ApplyOrRemoveStatuses(bool apply)
        {
            if (statusClass.statuses == null) { Print("Status list is null. skipping add or remove statuses.", debugStatuses, this); return; }
            if (Owner == null) { Debug.LogWarning("Owner is Null; can't add or remove statuses.", this); return; }

            if (Owner is Character)
            {
                foreach (var status in statusClass.statuses)
                {
                    if (status.effect != null)
                    {
                        Print($"{(apply ? "Applying" : "Removing")} status {status.name}", debugStatuses, this);
                        if (apply) status.effect.Apply(Owner as Character, status.strength + statusClass.Power);
                        else status.effect.Remove(Owner as Character);
                    }
                }
            }
        }
    }
}
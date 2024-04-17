using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public interface ICastableStateExecutor
    {
        public CastableState State { get; set; }
        public void SetActive(bool active);
        public bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed);
    }

    public class CastableStateExecutor : BaseMonoBehaviour, ICastableStateExecutor
    {
        protected CastableState state;

        public CastableState State { get => state; set => state = value; }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public virtual bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed)
        {
            if (stateAction.state == state)
            {
                onActionPerformed.Invoke(stateAction);
            }
            return true;
        }
    }
}
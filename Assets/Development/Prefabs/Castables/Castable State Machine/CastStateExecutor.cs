using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public interface ICastStateExecutor
    {
        public CastState State { get; set; }
        public void SetActive(bool active);
        public bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed);
    }

    public class CastStateExecutor : BaseMonoBehaviour, ICastStateExecutor
    {
        [SerializeField] protected CastState state;

        public CastState State { get => state; set => state = value; }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public virtual bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed)
        {
            if (stateAction.state == state)
            {
                onActionPerformed.Invoke(stateAction);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
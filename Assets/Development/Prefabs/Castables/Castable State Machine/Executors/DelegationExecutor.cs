using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class DelegatedExecutor : CastableStateExecutor
    {
        public override bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed)
        {
            onActionPerformed.Invoke(stateAction);
            return true;
        }
    }
}
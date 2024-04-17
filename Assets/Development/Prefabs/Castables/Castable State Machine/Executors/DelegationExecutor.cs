using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class DelegatedSingleExecutor : CastStateExecutor
    {
        public UnityAction StartAction;
        public List<CastAction> supportedActions;

        private UnityAction actionPerformed;

        public override bool PerformAction(StateAction stateAction, UnityAction<StateAction> onActionPerformed)
        {
            if (actionPerformed == null && supportedActions.Contains(stateAction.action))
            {
                actionPerformed = () => { onActionPerformed.Invoke(stateAction); };
                return true;
            }
            else
            {
                return false;
            }
        }

        [ButtonMethod]
        public void FinishAction()
        {
            actionPerformed.Invoke();
        }
    }
}
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace HotD.Castables
{
    using static Coordination;

    public class CastCoordinator : MecanimCoordinator
    {
        public int PowerLevel { get => GetInt("ChargeLevel"); set => SetPowerLevel(value); }
        public int ComboLevel { get => GetInt("ComboLevel"); set => SetComboLevel(value); }
        public ActionType ActionIndex { get => (ActionType)(int)GetFloat("Action"); set => SetActionIndex((int)value); }

        public UnityEvent<CastAction> onAction;

        public void RegisterActionListener(UnityAction<CastAction> listener, bool add = true)
        {
            if (add)
                onAction.AddListener(listener);
            else
                onAction.RemoveListener(listener);
        }

        private bool HasTrigger(Triggers triggers, Triggers trigger)
        {
            return (triggers & trigger) == trigger;
        }

        public void Coordinate(Triggers triggers)
        {
            if (triggers == Triggers.None) return;
            if (HasTrigger(triggers, Triggers.StartAction))
            {
                Print($"Triggering StartAction ({triggers})", debugTriggers, this);
                SetTrigger("StartAction");
            }
            if (HasTrigger(triggers, Triggers.StartCast))
            {
                Print($"Triggering StartCast ({triggers})", debugTriggers, this);
                SetTrigger("StartCast");
            }
        }

        public void SetComboLevel(int level)
        {
            Print("Set Combo Level", debugValues, this);
            SetInt("ComboLevel", level);
        }

        public void SetPowerLevel(int level)
        {
            Print("Set Power Level", debugValues, this);
            SetInt("ChargeLevel", level);
        }

        public void SetActionIndex(int idx)
        {
            Print("Set Action Index", debugValues, this);
            SetFloat("Action", idx);
        }

        [ButtonMethod]
        public void OnStartCast()
        {
            Print($"OnStartCast ({onAction.GetPersistentEventCount()} listners)", debugTriggers, this);
            onAction.Invoke(CastAction.Continue);
        }

        [ButtonMethod]
        public void OnEndCast()
        {
            Print($"OnEndCast ({onAction.GetPersistentEventCount()} listeners)", debugTriggers, this);
            onAction.Invoke(CastAction.End);
        }

        [Header("Tests")]
        public bool debug = false;
        [SerializeField] protected bool debugValues = false;
        [SerializeField] protected bool debugTriggers = false;
        public int testIdx = 0;
        [ButtonMethod]
        public void TestSetAction()
        {
            SetActionIndex(testIdx);
        }

        [ButtonMethod]
        public void TestStartAction()
        {
            SetTrigger("StartAction");
        }

        [ButtonMethod]
        public void TestStartCast()
        {
            SetTrigger("StartCast");
        }
    }
}
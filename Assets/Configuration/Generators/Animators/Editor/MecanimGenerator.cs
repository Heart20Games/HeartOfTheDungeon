using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMecanimGenerator", menuName = "Mecanim Generator", order = 1)]
public class MecanimGenerator : Generator
{
    public enum Slot { None, Idle, Hit, Dead, Run }

    [Serializable]
    public struct MotionSlot
    {
        public string name;
        public Slot slot;
        public Motion motion;
    }

    [Serializable]
    public struct Condition
    {
        public Condition(string parameter, AnimatorConditionMode mode, float threshold)
        {
            this.parameter = parameter;
            this.mode = mode;
            this.threshold = threshold;
        }
        public string parameter;
        public AnimatorConditionMode mode;
        public float threshold;
    }

    [Header("Settings")]
    public AnimatorController mecanim;

    public List<MotionSlot> slots = new();
    public List<Charges> chargeActions = new();
    public List<Combos> comboActions = new();

    public void GenerateMecanim()
    {
        if (!Application.isEditor)
        {
            Debug.LogWarning("Cannot generate castable outside the Editor");
            return;
        }

        string oldDirectory = fullDirectory;
        PrepareResultDirectory();
        bool sameDirectory = oldDirectory.Equals(fullDirectory);

        if (!(mecanim == null || overwrite || !sameDirectory))
        {
            Debug.LogWarning("I am not allowed to overwrite stuff, sorry.");
            return;
        }

        // Trim the name so it doesn't look like a series of subdirectories
        outputName = outputName.Trim('/');

        // Get a controller to work with
        string mecanimPath = $"{fullDirectory}/{outputName}.controller";
        if (mecanim == null)
        {
            // Load whatever Animator Controller we find.
            mecanim = AssetDatabase.LoadAssetAtPath<AnimatorController>(mecanimPath);
        }
        
        // Make a new one, or clear the old one.
        if (mecanim == null)
        {
            // Create a new controller.
            mecanim = AnimatorController.CreateAnimatorControllerAtPath(mecanimPath);
        }
        else if (replace)
        {
            foreach(var layer in mecanim.layers)
            {
                RecursiveClearStateMachine(layer.stateMachine);
            }
            for (int i = mecanim.parameters.Length - 1; i >= 0; i--)
            {
                mecanim.RemoveParameter(mecanim.parameters[i]);
            }
            for (int i = mecanim.layers.Length - 1; i >= 0; i--)
            {
                mecanim.RemoveLayer(i);
            }
            mecanim.AddLayer("Base Layer");
        }

        if (emptyTest) return;

        // Add Parameters
        mecanim.AddParameter("ChargeLevel", AnimatorControllerParameterType.Int);
        mecanim.AddParameter("ComboLevel", AnimatorControllerParameterType.Int);
        mecanim.AddParameter("StartCast", AnimatorControllerParameterType.Trigger);
        mecanim.AddParameter("StartAction", AnimatorControllerParameterType.Trigger);
        mecanim.AddParameter("Action", AnimatorControllerParameterType.Int);
        mecanim.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        mecanim.AddParameter("Run", AnimatorControllerParameterType.Bool);
        mecanim.AddParameter("Dead", AnimatorControllerParameterType.Bool);

        // Add StateMachines
        var root = mecanim.layers[0].stateMachine;
        var actions = root.AddStateMachine("Actions", new(-275, -150));

        // Add States
        var idle = root.AddState("Idle", new());
        var hit = root.AddState("Hit", new(0, -75));
        var dead = root.AddState("Dead", new(-275, 0));

        // Add Blend Trees
        var run = mecanim.CreateBlendTreeInController("Run", out var runTree);
        runTree.useAutomaticThresholds = false;

        // Add Motions
        foreach (MotionSlot motionSlot in slots)
        {
            switch (motionSlot.slot)
            {
                case Slot.Idle: idle.motion = motionSlot.motion; break;
                case Slot.Hit: hit.motion = motionSlot.motion; break;
                case Slot.Dead: dead.motion = motionSlot.motion; break;
                case Slot.Run: { runTree.AddChild(motionSlot.motion, 0); break; }
            }
        }

        // Position Nodes
        root.anyStatePosition = new(-275, -75);
        root.entryPosition = new(275, 0);
        root.exitPosition = new(275, 75);
        SetChildPosition(root, "Run", new(0, 75));

        // Add Transitions
        var deathTransition = root.AddAnyStateTransition(dead);
        deathTransition.AddCondition(AnimatorConditionMode.If, 0, "Dead");
        deathTransition.duration = 0;
        deathTransition.canTransitionToSelf = false;
        var unDeathTransition = dead.AddTransition(idle);
        unDeathTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Dead");

        var hitTransition = root.AddAnyStateTransition(hit);
        hitTransition.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        var unHitTransition = hit.AddTransition(idle);
        unHitTransition.duration = 0;
        unHitTransition.hasExitTime = true;

        var runTransition = idle.AddTransition(run);
        runTransition.AddCondition(AnimatorConditionMode.If, 0, "Run");
        runTransition.duration = 0;
        var unRunTransition = run.AddTransition(idle);
        unRunTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Run");
        unRunTransition.duration = 0;

        var actionsTransition = root.AddAnyStateTransition(actions);
        actionsTransition.AddCondition(AnimatorConditionMode.Greater, 0, "Action");
        actionsTransition.AddCondition(AnimatorConditionMode.If, 0, "StartAction");

        int actionIndex = 0;
        foreach (var charges in chargeActions)
        {
            actionIndex++;
            var action = AddChargeStateMachine(actions, charges);
            var actionTransition = actions.AddEntryTransition(action);
            actionTransition.AddCondition(AnimatorConditionMode.Equals, actionIndex, "Action");
            var exitActionTransition = actions.AddStateMachineExitTransition(action);
        }

        foreach (var combos in comboActions)
        {
            actionIndex++;
            var action = AddComboStateMachine(actions, combos);
            var actionTransition = actions.AddEntryTransition(action);
            actionTransition.AddCondition(AnimatorConditionMode.Equals, actionIndex, "Action");
            var exitActionTransition = actions.AddStateMachineExitTransition(action);
        }

        EditorUtility.SetDirty(mecanim);
        EditorUtility.SetDirty(this);
    }

    // Clear

    public void RecursiveClearStateMachine(AnimatorStateMachine root)
    {
        for (int i = root.states.Length - 1; i >= 0; i--)
        {
            root.RemoveState(root.states[i].state);
        }
        for (int i = root.stateMachines.Length - 1; i >= 0; i--)
        {
            RecursiveClearStateMachine(root.stateMachines[i].stateMachine);
            root.RemoveStateMachine(root.stateMachines[i].stateMachine);
        }
    }

    // Charge Machine
    [Serializable]
    public struct Charges
    {
        public string name;
        public List<Charge> charges;
    }

    [Serializable]
    public struct Charge
    {
        public string name;
        public bool shortCircuit;
        public Motion chargeMotion;
        public Motion sustainMotion;
        public Motion castMotion;
        public Motion holdMotion;
    }

    public AnimatorState AddEndCastState(AnimatorStateMachine root, Vector2 position = new())
    {
        AnimatorState endCast = root.AddState("EndCast", position);
        var animEvent = endCast.AddStateMachineBehaviour<AnimatorEvent>();
        animEvent.methodName = "OnEndCast";
        animEvent.eventName = null;
        animEvent.eventType = AnimatorEvent.Event.EnterState;
        animEvent.targets = AnimatorEvent.Target.Parent;
        
        var exitTransition = endCast.AddExitTransition();
        exitTransition.exitTime = 0;
        exitTransition.hasExitTime = true;

        return endCast;
    }

    public AnimatorStateMachine AddChargeStateMachine(AnimatorStateMachine root, Charges blueprints, Vector2 position=new())
    {
        var sub = root.AddStateMachine(blueprints.name);
        sub.entryPosition = new();

        AnimatorState endCast = AddEndCastState(sub, new(300, 0));

        List<AnimatorStateMachine> charges = new();
        int num = 0;
        AnimatorStateMachine prev = null;
        AnimatorState prevCharge = null;
        AnimatorState prevCast = null;
        foreach (Charge blueprint in blueprints.charges)
        {
            num += 1;
            var cur = AddChargeSubMachine(sub, blueprint, num, out var charge, out var cast, new(0, 150 * num));
            charges.Add(cur);

            if (prev != null)
            {
                var chargeTransition = prevCharge.AddTransition(charge);
                chargeTransition.AddCondition(AnimatorConditionMode.Greater, num, "ChargeLevel");

                var backtrackTransition = charge.AddTransition(prevCast);
                backtrackTransition.AddCondition(AnimatorConditionMode.If, 0, "StartCast");
            }
            else
            {
                sub.defaultState = cur.defaultState;
            }

            var endCastTransition = sub.AddStateMachineTransition(cur, endCast);
            
            prev = cur;
            prevCharge = charge;
            prevCast = cast;
        }

        sub.exitPosition = new(300, 150 * (num));

        return sub;
    }

    public AnimatorStateMachine AddChargeSubMachine(AnimatorStateMachine root, Charge blueprint, int level, out AnimatorState charge, out AnimatorState cast, Vector2 position=new())
    {
        var sub = root.AddStateMachine($"Charge {level} ({blueprint.name})", position);

        charge = sub.AddState("Charge", new());
        var sustain = sub.AddState("Sustain", new(0, 150));
        cast = sub.AddState("Cast", new(225, 300));

        sub.parentStateMachinePosition = new(-225, 150);

        charge.motion = blueprint.chargeMotion;
        sustain.motion = blueprint.sustainMotion;
        cast.motion = blueprint.castMotion;

        sub.AddEntryTransition(charge);

        var elevateTransition = charge.AddExitTransition(false);
        elevateTransition.AddCondition(AnimatorConditionMode.Greater, level, "ChargeLevel");

        var sustainTransition = charge.AddTransition(sustain);
        sustainTransition.duration = 0;
        sustainTransition.AddCondition(AnimatorConditionMode.Less, level + 1, "ChargeLevel");

        var castTransition = sustain.AddTransition(cast);
        castTransition.duration = 0;
        castTransition.AddCondition(AnimatorConditionMode.If, 0, "StartCast");

        if (blueprint.shortCircuit)
        {
            var shortTransition = charge.AddTransition(cast);
            shortTransition.duration = 0;
            shortTransition.AddCondition(AnimatorConditionMode.If, 0, "StartCast");
        }

        if (blueprint.holdMotion != null)
        {
            var hold = sub.AddState("Hold", new(0, 225));
            hold.motion = blueprint.holdMotion;

            var holdTransition = cast.AddTransition(hold);
            holdTransition.hasExitTime = true;

            var exitTransition = hold.AddExitTransition();
            exitTransition.hasExitTime = true;

        }
        else
        {
            var exitTransition = cast.AddExitTransition();
            exitTransition.hasExitTime = true;
        }

        return sub;
    }

    // Combo Machine
    [Serializable]
    public struct Combos
    {
        public string name;
        public List<Motion> motions;
    }

    public AnimatorStateMachine AddComboStateMachine(AnimatorStateMachine root, Combos blueprint)
    {
        var sub = root.AddStateMachine($"Combo ({blueprint.name})");

        int num = 0;
        foreach (Motion motion in blueprint.motions)
        {
            num += 1;
            var combo = sub.AddState($"Combo {num}");
            combo.motion = motion;

            var entryTransition = sub.AddEntryTransition(combo);
            entryTransition.AddCondition(AnimatorConditionMode.Equals, num, "ComboLevel");
            
            var exitTransition = combo.AddExitTransition(false);
            exitTransition.hasExitTime = true;
        }

        return sub;
    }


    // Helpers

    public void SetChildPosition(AnimatorStateMachine stateMachine, string name, Vector2 position)
    {
        var states = stateMachine.states;
        for (int i = 0; i < states.Length; i++)
        {
            var child = states[i];
            if (child.state.name == name)
            {
                var newChild = child;
                newChild.position = position;
                newChild.state = child.state;
                states[i] = newChild;
            }
        }
        stateMachine.states = states;
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace HotD.Generators
{
    using HotD.Castables;
    using static YarnRooms;

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

        public bool useBehaviourBasedAnimatorEvents = true;
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
                mecanim.RemoveLayer(0);
            }
            else if (replace)
            {
                foreach (var layer in mecanim.layers)
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
            }

            if (emptyTest) return;

            // Add Parameters
            mecanim.AddParameter("ChargeLevel", AnimatorControllerParameterType.Int);
            mecanim.AddParameter("ComboLevel", AnimatorControllerParameterType.Int);
            mecanim.AddParameter("StartCast", AnimatorControllerParameterType.Trigger);
            mecanim.AddParameter("StartAction", AnimatorControllerParameterType.Trigger);
            mecanim.AddParameter("Action", AnimatorControllerParameterType.Float);
            mecanim.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
            mecanim.AddParameter("Run", AnimatorControllerParameterType.Bool);
            mecanim.AddParameter("Dead", AnimatorControllerParameterType.Bool);
            
            // Generate Layers
            int layerIdx = 0;
            GenerateMoveLayer(layerIdx++);
            GenerateActionLayer(layerIdx++);

            // Dirty it up
            EditorUtility.SetDirty(mecanim);
            EditorUtility.SetDirty(this);
        }

        private void GenerateMoveLayer(int layer)
        {
            // Add Layer
            mecanim.AddLayer("Move");
            var root = mecanim.layers[layer].stateMachine;

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
            // Death
            var deathTransition = root.AddAnyStateTransition(dead);
            deathTransition.AddCondition(AnimatorConditionMode.If, 0, "Dead");
            deathTransition.duration = 0;
            deathTransition.canTransitionToSelf = false;
            var unDeathTransition = dead.AddTransition(idle);
            unDeathTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Dead");

            // Hit
            var hitTransition = root.AddAnyStateTransition(hit);
            hitTransition.AddCondition(AnimatorConditionMode.If, 0, "Hit");
            var unHitTransition = hit.AddTransition(idle);
            unHitTransition.duration = 0;
            unHitTransition.hasExitTime = true;

            // Run
            var runTransition = idle.AddTransition(run);
            runTransition.AddCondition(AnimatorConditionMode.If, 0, "Run");
            runTransition.duration = 0;
            var unRunTransition = run.AddTransition(idle);
            unRunTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Run");
            unRunTransition.duration = 0;
        }

        private void GenerateActionLayer(int layer)
        {
            // Add Layer
            mecanim.AddLayer("Action");
            var actualLayer = mecanim.layers[layer];
            actualLayer.defaultWeight = 1;
            var root = mecanim.layers[layer].stateMachine;

            // Add StateMachines
            var actions = root.AddStateMachine("Actions", new(-275, -150));

            // Add States
            var idle = root.AddState("Idle", new());
            var actionSplit = actions.AddState("Action Split", new(250, 100));

            // Position Nodes
            root.anyStatePosition = new(-275, -75);
            root.entryPosition = new(275, 0);
            root.exitPosition = new(275, 75);

            // Add Transitions
            // Actions
            var actionsTransition = root.AddAnyStateTransition(actions);
            actionsTransition.AddCondition(AnimatorConditionMode.Greater, 0, "Action");
            actionsTransition.AddCondition(AnimatorConditionMode.If, 0, "StartAction");

            // Action Split
            var actionSplitTransition = actions.AddEntryTransition(actionSplit);

            // Charges
            int actionCount = chargeActions.Count + comboActions.Count;
            int actionIndex = 0;
            foreach (var charges in chargeActions)
            {
                var action = AddChargeStateMachine(actions, charges, new Vector2(550, -(actionCount / 2) + (100 * actionIndex)));
                var actionTransition = actionSplit.AddTransition(action);
                actionTransition.duration = 0;
                actionTransition.AddCondition(AnimatorConditionMode.Greater, (int)charges.actionType-1, "Action");
                actionTransition.AddCondition(AnimatorConditionMode.Less, (int)charges.actionType+1, "Action");
                var exitActionTransition = actions.AddStateMachineExitTransition(action);
                actionIndex++;
            }

            // Combos
            foreach (var combos in comboActions)
            {
                var action = AddComboStateMachine(actions, combos, new Vector2(550, -(actionCount / 2) + (100 * actionIndex)));
                var actionTransition = actionSplit.AddTransition(action);
                actionTransition.duration = 0;
                actionTransition.AddCondition(AnimatorConditionMode.Greater, (int)combos.actionType-1, "Action");
                actionTransition.AddCondition(AnimatorConditionMode.Less, (int)combos.actionType+1, "Action");
                var exitActionTransition = actions.AddStateMachineExitTransition(action);
                actionIndex++;
            }
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
            public ActionType actionType;
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

        // AnimatorEvent Behaviours
        public AnimatorState AddAnimatorEventState(AnimatorStateMachine root, string name, bool exit, Vector2 position = new(), string eventName = null)
        {
            AnimatorState state = root.AddState(name, position);

            if (useBehaviourBasedAnimatorEvents)
            {
                AddAnimatorEventBehaviour(state, "On" + (eventName ?? name), null, AnimatorEvent.Event.EnterState, AnimatorEvent.Target.Parent);
            }

            if (exit)
            {
                var exitTransition = state.AddExitTransition();
                exitTransition.exitTime = 0;
                exitTransition.hasExitTime = true;
            }

            return state;
        }

        public void AddAnimatorEventBehaviour(AnimatorState state, string methodName, string eventName, AnimatorEvent.Event eventType, AnimatorEvent.Target targets)
        {
            var animEvent = state.AddStateMachineBehaviour<AnimatorEvent>();
            animEvent.methodName = methodName;
            animEvent.eventName = eventName;
            animEvent.eventType = eventType;
            animEvent.targets = targets;
        }

        // Charge Machine
        public AnimatorStateMachine AddChargeStateMachine(AnimatorStateMachine root, Charges blueprints, Vector2 position = new())
        {
            var sub = root.AddStateMachine(blueprints.name);
            sub.entryPosition = new();

            AnimatorState endCast = AddAnimatorEventState(sub, "EndCast", true, new(300, 0));

            List<AnimatorStateMachine> charges = new();
            int num = 0;
            AnimatorStateMachine prev = null;
            AnimatorState prevCharge = null;
            AnimatorState prevSustain = null;
            AnimatorState prevCast = null;
            foreach (Charge blueprint in blueprints.charges)
            {
                num += 1;
                var cur = AddChargeSubMachine(sub, blueprint, num, out var charge, out var sustain, out var cast, new(0, 150 * num));
                charges.Add(cur);

                if (prev != null)
                {
                    var chargeTransition = prevCharge.AddTransition(charge);
                    chargeTransition.AddCondition(AnimatorConditionMode.Greater, num-1, "ChargeLevel");

                    var chargeFromSustainTransition = prevSustain.AddTransition(charge);
                    chargeFromSustainTransition.AddCondition(AnimatorConditionMode.Greater, num-1, "ChargeLevel");

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
                prevSustain = sustain;
                prevCast = cast;
            }

            sub.exitPosition = new(300, 150 * (num));

            return sub;
        }

        public AnimatorStateMachine AddChargeSubMachine(AnimatorStateMachine root, Charge blueprint, int level, out AnimatorState charge, out AnimatorState sustain, out AnimatorState cast, Vector2 position = new())
        {
            var sub = root.AddStateMachine($"Charge {level} ({blueprint.name})", position);

            charge = sub.AddState("Charge", new());
            sustain = sub.AddState("Sustain", new(0, 150));
            cast = AddAnimatorEventState(sub, "Cast", false, new(225, 300), "StartCast");

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
                holdTransition.exitTime = 1.0f;

                var exitTransition = hold.AddExitTransition();
                exitTransition.hasExitTime = true;
                holdTransition.exitTime = 1.0f;

            }
            else
            {
                var exitTransition = cast.AddExitTransition();
                exitTransition.hasExitTime = true;
                exitTransition.exitTime = 1.0f;
            }

            return sub;
        }

        // Combo Machine
        [Serializable]
        public struct Combos
        {
            public string name;
            public ActionType actionType;
            public List<Motion> motions;
        }

        public AnimatorStateMachine AddComboStateMachine(AnimatorStateMachine root, Combos blueprint)
        {
            return AddComboStateMachine(root, blueprint, new Vector2(400, 0));
        }
        public AnimatorStateMachine AddComboStateMachine(AnimatorStateMachine root, Combos blueprint, Vector2 position)
        {
            var sub = root.AddStateMachine($"Combo ({blueprint.name} - {blueprint.actionType})", position);

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
}
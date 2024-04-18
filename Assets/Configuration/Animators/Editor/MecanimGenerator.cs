using Codice.CM.Common.Tree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

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

    [Header("Settings")]
    public AnimatorController mecanim;

    public List<MotionSlot> slots;

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

        if (replace) // && !sameDirectory)
        {
            if (mecanim != null)
            {
                AssetDatabase.DeleteAsset($"{oldDirectory}/{mecanim.name}.prefab");
                mecanim = null;
            }
        }

        // Trim the name so it doesn't look like a series of subdirectories
        outputName = outputName.Trim('/');

        // Create a new controller
        mecanim = AnimatorController.CreateAnimatorControllerAtPath($"{fullDirectory}/{outputName}.controller");

        // Add Parameters
        mecanim.AddParameter("CastLevel", AnimatorControllerParameterType.Int);
        mecanim.AddParameter("StartCast", AnimatorControllerParameterType.Trigger);
        mecanim.AddParameter("Action", AnimatorControllerParameterType.Int);
        mecanim.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        mecanim.AddParameter("Run", AnimatorControllerParameterType.Bool);
        mecanim.AddParameter("Dead", AnimatorControllerParameterType.Bool);

        // Add StateMachines
        var root = mecanim.layers[0].stateMachine;
        var action = root.AddStateMachine("Actions", new(-275, -150));

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
        var unDeathTransition = dead.AddTransition(idle);
        unDeathTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Dead");

        var hitTransition = root.AddAnyStateTransition(hit);
        hitTransition.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        var unHitTransition = hit.AddTransition(idle);
        unHitTransition.duration = 0;

        var runTransition = idle.AddTransition(run);
        runTransition.AddCondition(AnimatorConditionMode.If, 0, "Run");
        runTransition.duration = 0;
        var unRunTransition = run.AddTransition(idle);
        unRunTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Run");
        unRunTransition.duration = 0;

        var actionTransition = root.AddAnyStateTransition(action);
        actionTransition.AddCondition(AnimatorConditionMode.Greater, 0, "Action");

        EditorUtility.SetDirty(mecanim);

        EditorUtility.SetDirty(this);
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

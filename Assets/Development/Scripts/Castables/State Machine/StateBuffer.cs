using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    // Action Buffer Manager
    public interface IStateBuffer
    {
        public void BufferAction(CastAction action, ActionBuffer requeued);
        public void SetBufferKeepFlags(CastAction action);
        public void SetBufferClearFlags(CastAction action);
        public void SetBufferClearFlags(CastState state);
    }
    
    public class StateBuffer : BaseMonoBehaviour, IStateBuffer
    {
        [SerializeField][ReadOnly] private StateCastable csm;

        public List<ActionBuffer> actionBuffers = new();
        [SerializeField] private List<ActionBuffer> bufferedActions = new();

        [SerializeField][ReadOnly] private float currentTime = -1;
        [SerializeField] private bool debugBuffering = false;
        [SerializeField] private bool debugWithDetail = false;
        [SerializeField] private bool debugWithBreaks = false;

        private void Awake()
        {
            csm = GetComponent<StateCastable>();
        }

        // Keep the buffer running.

        private void Update()
        {
            currentTime = Time.time;

            // Remove actions from the buffer, requeue them if they haven't been in the buffer too long.
            for (int i = bufferedActions.Count - 1; i >= 0; i--)
            {
                ActionBuffer buffered = bufferedActions[i];
                bufferedActions.RemoveAt(i);

                if (!buffered.ShouldClear())
                {
                    if (debugBuffering && debugWithDetail) Debug.LogWarning($"Requeue Buffer: {buffered} {{{csm.state} in {buffered.ClearStatesString()}}}");
                    csm.QueueAction(buffered.action, buffered);
                }
                else if (debugBuffering)
                {
                    Debug.LogWarning($"Dropped Buffer: {buffered} {{{csm.state} in {buffered.ClearStatesString()}}}");
                    if (debugWithBreaks) Debug.Break();
                }
            }
        }


        // Add something to the buffer.

        public void BufferAction(CastAction action, ActionBuffer requeued)
        {
            // If the action is None, we should ignore it.
            if (requeued.action == CastAction.None) return;

            bool isNewBuffer = requeued.timeBuffered < 0;
            float timeBuffered = requeued.timeBuffered == -1 ? Time.time : requeued.timeBuffered;

            ActionBuffer template = requeued;
            ActionBuffer finalBuffer = template;
            bool foundValidBufferTemplate = false;

            // Find a template buffer
            int templateIndex = actionBuffers.FindIndex((ActionBuffer option) => { return option.action == action; });
            if (templateIndex >= 0)
            {
                if (isNewBuffer)
                {
                    template = actionBuffers[templateIndex];
                    finalBuffer = template;
                }
                foundValidBufferTemplate = true;
            }

            // If there isn't a valid template, we should just ignore this action.
            if (!foundValidBufferTemplate) return;

            // Determine whether an action of this kind is already in the buffer.
            int bufferIndex = bufferedActions.FindIndex((ActionBuffer buffered) => { return buffered.action == action; });
            if (bufferIndex < 0)
            {
                // Add a new buffered action
                finalBuffer = new(template, timeBuffered);
                bufferedActions.Add(finalBuffer);
            }
            else
            {
                // Modify the old buffered action
                bufferedActions[bufferIndex] = new(bufferedActions[bufferIndex], template);
                bufferedActions[bufferIndex] = new(bufferedActions[bufferIndex], timeBuffered);
                finalBuffer = bufferedActions[bufferIndex];
            }

            // Some debugging
            if (debugBuffering)
            {
                if (finalBuffer.timeBuffered < 0)
                {
                    Debug.LogWarning($"Not Buffering: {action} ({(isNewBuffer ? "New" : "Old")})");
                }
                else
                {
                    Debug.LogWarning($"Buffering: {finalBuffer.action} ({(isNewBuffer ? "New" : "Old")})");
                    if (debugWithBreaks && isNewBuffer) Debug.Break();
                }
            }
        }


        // Flag Helpers

        public void SetBufferKeepFlags(CastAction action)
        {
            for (int i = 0; i < bufferedActions.Count; i++)
            {
                if (!bufferedActions[i].keepReached)
                    bufferedActions[i] = bufferedActions[i].MarkKeepReached(action);
            }
        }

        public void SetBufferClearFlags(CastAction action)
        {
            for (int i = 0; i < bufferedActions.Count; i++)
            {
                if (!bufferedActions[i].clearReached)
                    bufferedActions[i] = bufferedActions[i].MarkClearReached(action);
            }
        }

        public void SetBufferClearFlags(CastState state)
        {
            for (int i = 0; i < bufferedActions.Count; i++)
            {
                if (!bufferedActions[i].clearReached)
                    bufferedActions[i] = bufferedActions[i].MarkClearReached(state);
            }
        }

        // Quick Setup Methods

        [ButtonMethod]
        public void AddTriggerBuffer()
        {
            AddTriggerBuffer(0.3f);
        }
        public void AddTriggerBuffer(float bufferTime)
        {
            actionBuffers ??= new();
            actionBuffers.Add(new(CastAction.Trigger, bufferTime, CastAction.Release, CastAction.None, new() { CastState.Activating }));
            actionBuffers.Add(new(CastAction.Release, bufferTime, CastAction.None, CastAction.Trigger, new() { CastState.Executing }));
        }
    }


    // Action Buffer
    [Serializable]
    public struct ActionBuffer
    {
        public string name;
        public CastAction action;

        public float timeInBuffer;
        [ReadOnly] public float timeBuffered;
        
        public CastAction keepUntil;
        [ReadOnly] public bool keepReached;
        
        public CastAction clearOn;
        [ReadOnly] public bool clearReached;
        
        [ReadOnly] public bool isRequeued;
        
        public List<CastState> clearStates;

        // Empty Initialization
        public ActionBuffer(CastAction action)
        {
            this.name = action.ToString();
            this.action = action;
            this.timeInBuffer = 0;
            this.keepUntil = CastAction.None;
            this.clearOn = CastAction.None;
            this.clearStates = new();

            this.timeBuffered = -1;
            this.keepReached = keepUntil == CastAction.None;
            this.clearReached = false;
            this.isRequeued = false;
        }

        // Pure Initialization
        public ActionBuffer(CastAction action, float timeInBuffer, CastAction keepUntil, CastAction clearOn, List<CastState> clearStates)
        {
            this.name = action.ToString();
            this.action = action;
            this.timeInBuffer = timeInBuffer;
            this.keepUntil = keepUntil;
            this.clearOn = clearOn;
            this.clearStates = clearStates;

            this.timeBuffered = -1;
            this.keepReached = keepUntil == CastAction.None;
            this.clearReached = false;
            this.isRequeued = false;
        }


        // Copies the buffer with the given buffered time.
        public ActionBuffer(ActionBuffer old, float timeBuffered)
        {
            this.name = old.name;
            this.action = old.action;
            this.timeInBuffer = old.timeInBuffer;
            this.keepUntil = old.keepUntil;
            this.keepReached = this.keepUntil == CastAction.None || old.keepReached;
            this.clearOn = old.clearOn;
            this.clearReached = old.clearReached;
            this.isRequeued = old.isRequeued;
            this.clearStates = old.clearStates;

            this.timeBuffered = timeBuffered;
        }


        // Merges the two given buffers
        public ActionBuffer(ActionBuffer old1, ActionBuffer old2)
        {
            this.name = old1.name;
            this.action = old1.action;
            this.timeInBuffer = Mathf.Max(old1.timeInBuffer, old2.timeInBuffer);
            this.timeBuffered = Mathf.Max(old1.timeBuffered, old2.timeBuffered);
            this.keepUntil = old1.keepUntil | old2.keepUntil;
            this.keepReached = this.keepUntil == CastAction.None || old1.keepReached || old2.keepReached;
            this.clearOn = old1.clearOn | old2.clearOn;
            this.clearReached = old1.clearReached || old2.clearReached;
            this.isRequeued = old1.isRequeued || old2.isRequeued;
            
            this.clearStates = new();
            foreach (var state in old1.clearStates)
                clearStates.Add(state);
            foreach (var state in old2.clearStates)
                if (!clearStates.Contains(state))
                    clearStates.Add(state);
        }


        // Marks whether the KeepUntil CastAction is being passed.
        public readonly ActionBuffer MarkKeepReached(CastAction keepUntil)
        {
            if (this.keepReached)
            {
                Debug.Log($"Marked already; Keep {action}? {(this.keepReached ? "Yes" : "No")}");
                return this;
            }
            else if (this.keepUntil == CastAction.None)
            {
                Debug.Log($"Keep Auto Marked on {this.action}. ({this.keepUntil} vs {keepUntil})");
                return new(this, timeBuffered, true);
            }
            else
            {
                bool keepReached = this.keepUntil.HasFlag(keepUntil);
                Debug.Log($"Keep Marked on {this.action}? {(keepReached ? "Yes" : "No")} ({this.keepUntil} vs {keepUntil})");
                return new(this, keepReached ? Time.time : timeBuffered, keepReached);
            }
        }
        public ActionBuffer(ActionBuffer old, float timeBuffered, bool keepReached)
        {
            this.name = old.name;
            this.action = old.action;
            this.timeInBuffer = old.timeInBuffer;
            this.keepUntil = old.keepUntil;
            this.clearOn = old.clearOn;
            this.clearReached = old.clearReached;
            this.isRequeued = old.isRequeued;
            this.clearStates = old.clearStates;

            this.timeBuffered = timeBuffered;
            this.keepReached = keepReached || this.keepUntil == CastAction.None;
        }


        // Marks whether the ClearOn CastAction is being passed.
        public readonly ActionBuffer MarkClearReached(CastState clearState)
        {
            if (this.clearReached)
            {
                Debug.Log($"Marked already; Clear {action}? {(this.clearReached ? "Yes" : "No")}");
                return this;
            }
            else if (this.clearStates.Count == 0)
            {
                Debug.Log($"Clear Auto Marked on {this.action}. ({clearState} in {ClearStatesString()})");
                return new(this, false);
            }
            else
            {
                bool clearReached = this.clearStates.Contains(clearState);
                Debug.Log($"Clear Marked on {action}? {(clearReached ? "Yes" : "No")} ({clearState} in {ClearStatesString()})");
                return new(this, clearReached);
            }
        }
        public readonly ActionBuffer MarkClearReached(CastAction clearOn)
        {
            if (this.clearReached)
            {
                Debug.Log($"Marked already; Clear {action}? {(this.clearReached ? "Yes" : "No")}");
                return this;
            }
            else if (this.clearOn == CastAction.None)
            {
                Debug.Log($"Clear Auto Marked on {this.action}. ({this.clearOn} vs {clearOn})");
                return new(this, false);
            }
            else
            {
                bool clearReached = this.clearOn.HasFlag(clearOn);
                Debug.Log($"Clear Marked on {action}? {(clearReached ? "Yes" : "No")} ({this.clearOn} vs {clearOn})");
                return new(this, clearReached);
            }
        }
        public ActionBuffer(ActionBuffer old, bool clearReached)
        {
            this.name = old.name;
            this.action = old.action;
            this.timeInBuffer = old.timeInBuffer;
            this.timeBuffered = old.timeBuffered;
            this.keepUntil = old.keepUntil;
            this.keepReached = old.keepReached || this.keepUntil == CastAction.None;
            this.clearOn = old.clearOn;
            this.isRequeued = old.isRequeued;
            this.clearStates = old.clearStates;

            this.clearReached = clearReached;
        }


        // Checks for determining whether this buffer should be dropped.
        public readonly bool ShouldClear()
        {
            return clearReached || (BufferTimeReached() && keepReached);
        }
        public readonly bool BufferTimeReached()
        {
            return Time.time > (timeBuffered + timeInBuffer);
        }


        // ToString helpers
        public override readonly string ToString()
        {
            float endTime = timeBuffered + timeInBuffer;
            return $"{action} ({timeBuffered} -> {Time.time} / {endTime}: {endTime - Time.time}) [c: {clearReached} || t: {BufferTimeReached()} + k: {keepReached}]";
        }

        public readonly string ClearStatesString()
        {
            string result = "[";
            foreach (CastState state in clearStates)
            {
                result += $"{state} ";
            }
            result += "]";
            return result;
        }
    }
}
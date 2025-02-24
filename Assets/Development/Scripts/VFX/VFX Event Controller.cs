using System.Collections.Generic;
using HotD.Body;
using MyBox;
using UnityEngine;

namespace HotD.Body
{
    public class VFXEventController : BaseMonoBehaviour
    {
        [Foldout("Parts", true)]
        [SerializeField] private Animator animator;
        [SerializeField] private Movement movement;
        [SerializeField] private Rigidbody character;
        [Foldout("Parts")]
        [SerializeField] private List<Transform> firepoints;

        public bool debug;

        [Header("Cast Levels")]
        [SerializeField] private int minCastLevel = 2;
        [SerializeField] private int maxCastLevel = 3;

        [Header("Effects")]
        public List<CastedVFX> effects = new();
        public List<CastedVFX> effectsBeingCasted = new();

        private void Start()
        {
            animator = GetComponent<Animator>();
            movement = GetComponentInParent<Movement>();
            character = GetComponentInParent<Rigidbody>();
        }

        public void AddVFX(CastedVFX vfx)
        {
            if (isActiveAndEnabled && vfx != null)
            {
                effects.Add(vfx);
                vfx.AddListenerController(false, false,
                    (int firepointIdx) => { SetFirePoint(vfx, firepointIdx); },
                    (int limit) => { SetCastLimit(vfx, limit); },
                    () => { OnVFXTriggered(vfx); },
                    () => { OnVFXReleased(vfx); },
                    (_) => { OnVFXCasted(vfx); },
                    () => { OnVFXEndCast(vfx); }
                );
            }
        }

        public void SetCastLimit(CastedVFX vfx, int limit)
        {
            for (int i = minCastLevel; i <= maxCastLevel; i++)
            {
                animator.SetBool($"Level{i}", i <= limit);
                SetVFXBool(vfx, $"Level {i} Available", i <= limit);
            }
        }

        // Casting

        public void OnVFXTriggered(CastedVFX vfx)
        {
            Print($"VFX Triggered ({vfx.name})", debug);
            SetVFXEnabled(vfx, true);
            SetVFXBool(vfx, "Sustain", true);
            if (animator.HasParameter(vfx.triggerParameter, this))
                animator.SetTrigger(vfx.triggerParameter);
        }

        public void OnVFXReleased(CastedVFX vfx)
        {
            Print($"VFX Released ({vfx.name})", debug);
            SetVFXBool(vfx, "Sustain", false);
        }

        public void OnVFXCasted(CastedVFX vfx)
        {
            SetVFXTrigger("Cast");
            animator.SetTrigger(vfx.castParameter);
            effectsBeingCasted.Add(vfx);
        }

        public void OnAnimCasted(string castKey)
        {
            if (TryGetEffectBeingCasted(castKey, out var vfx))
                SetVFXTrigger(vfx, vfx.castParameter);
        }

        public void Release(string castKey)
        {
            if (TryGetEffectBeingCasted(castKey, out var vfx))
            {
                effectsBeingCasted.Remove(vfx);
                vfx.Release();
            }
        }

        public void OnVFXEndCast(CastedVFX vfx)
        {
            Print($"VFX End Cast ({vfx.name})", debug);
            SetVFXEnabled(vfx, false);
        }

        public bool TryGetEffectBeingCasted(string castKey, out CastedVFX result)
        {
            foreach (CastedVFX vfx in effectsBeingCasted)
            {
                if (vfx.castKey == castKey)
                {
                    result = vfx;
                    return true;
                }
            }
            result = null;
            return false;
        }

        // Fire Points
        public void SetFirePoint(CastedVFX vfx, int firepointIdx)
        {
            if (firepoints.Count > firepointIdx)
                SetVFXParent(vfx, firepoints[firepointIdx]);
            else
                Debug.LogWarning($"Trying to set vfx ({vfx.name}) parent to firepoint at index {firepointIdx}. Index out of bounds.");
        }

        // Movement
        //public void StopMovement()
        //{
        //    movement.enabled = false;
        //    character.velocity = new Vector3(0,0,0);
        //}

        //public void StartMovement()
        //{
        //    movement.enabled = true;
        //}

        //public void VFXFirePoint2()
        //{
        //    SetVFXParent(firePoint2.transform, firePoint2.transform.position);
        //    //vfx.transform.Rotate(firePoint1.transform.rotation.x, firePoint1.transform.rotation.y, firePoint1.transform.rotation.z + 90);
        //}

        //public void VFXFirePoint1()
        //{
        //    SetVFXParent(firePoint1.transform, new Vector3(0f,0f,0f));
        //}

        // Beam
        //public void FireLevel3Beam()
        //{
        //    if (level3Beam != null)
        //    {
        //        level3Beam.gameObject.SetActive(true);
        //        level3Beam.cast = true;
        //    }
        //    else Debug.LogWarning("No Level 3 Beam to Fire.");
        //}

        //public void EndBeam()
        //{
        //    if (level3Beam != null)
        //    {
        //        level3Beam.gameObject.SetActive(false);
        //    }
        //    else Debug.LogWarning("No Level 3 Beam to End.");
        //}

        // VFX Helpers

        public void SetVFXBool(string property, bool value)
        {
            foreach (var effect in effects)
                SetVFXBool(effect, property, value);
        }
        public void SetVFXBool(CastedVFX vfx, string property, bool value)
        {
            if (vfx.Equipped)
                vfx.animator.SetBool(property, value);
        }

        public void SetVFXTrigger(string property)
        {
            foreach (var effect in effects)
                SetVFXTrigger(effect, property);
        }
        public void SetVFXTrigger(CastedVFX vfx, string property)
        {
            if (vfx.animator != null && vfx.Equipped)
                vfx.animator.SetTrigger(property);
        }

        public void SetVFXEnabled(bool enabled)
        {
            Print("Set all VFX Enabled.", debug);
            foreach (var effect in effects)
                SetVFXEnabled(effect, enabled);
        }
        public void SetVFXEnabled(CastedVFX vfx, bool enabled)
        {
            //vfx.gameObject.SetActive(enabled && vfx.equipped);
            Print($"Set VFX Enabled? {vfx.name}", debug);
            vfx.VisualsEnabled = enabled && vfx.Equipped;
        }

        public void SetVFXParent(Transform parent, Vector3 position = new())
        {
            foreach (var effect in effects)
            {
                SetVFXParent(effect, parent, position);
            }
        }
        public void SetVFXParent(CastedVFX vfx, Transform parent, Vector3 position = new())
        {
            vfx.transform.parent = parent;
            vfx.transform.position = position;
        }
    }
}
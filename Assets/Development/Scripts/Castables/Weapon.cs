using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Body;
using HotD.Body;

namespace HotD.Castables
{
    public class Weapon : Castable
    {
        private Animator animator;
        public bool swinging = false; // toggled in weapon 
        public float swingLength = 1; // Non-animation swing time
        public float speed = 3f; // speed of the animation
        public int damage = 1;

        private readonly List<IDamageReceiver> others = new();
        private readonly List<IDamageReceiver> ignored = new();

        public override bool CanCast { get => !swinging; }

        private void Awake()
        {
            if (TryGetComponent(out animator))
            {
                animator.speed = speed;
            }
            fields.pivot.gameObject.SetActive(false);
        }

        // Castable

        public override void Initialize(Character source)
        {
            base.Initialize(source);
            IDamageReceiver damageable = source.Body.GetComponent<IDamageReceiver>();
            if (damageable != null)
            {
                ignored.Add(damageable);
            }
        }

        public override void Cast()
        {
            Swing();
        }


        // Swinging

        public void Swing()
        {
            if (animator != null)
            {
                SwingForAnimation();
            }
            else
            {
                StartCoroutine(SwingForSeconds(swingLength));
            }
        }

        public IEnumerator SwingForSeconds(float seconds)
        {
            StartSwinging();
            yield return new WaitForSeconds(seconds);
            DoneSwinging();
        }

        public void SwingForAnimation()
        {
            StartSwinging();
            animator.SetTrigger("Swing");
        }

        public void HitDamagable(Impactor impactor)
        {
            IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();
            if (other != null && !ignored.Contains(other) && !others.Contains(other))
            {
                others.Add(other);
                other.SetDamagePosition(impactor.other.ImpactLocation);
                other.TakeDamage(damage, Identity);
            }
        }

        public void LeftDamagable(Impactor impactor)
        {
            IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();
            if (other != null && !ignored.Contains(other) && others.Contains(other))
            {
                others.Remove(other);
            }
        }

        public void StartSwinging()
        {
            swinging = true;
            fields.pivot.gameObject.SetActive(true);
        }

        public void DoneSwinging()
        {
            swinging = false;
            fields.pivot.gameObject.SetActive(false);
            others.Clear();
        }


        // Cleanup

        private void OnDestroy()
        {
            Destroy(fields.pivot.gameObject);
            if (fields.weaponArt != null)
            {
                Destroy(fields.weaponArt.gameObject);
            }
        }
    }
}
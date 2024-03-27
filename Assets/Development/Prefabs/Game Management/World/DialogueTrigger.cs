using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD
{
    public class DialogueTrigger : Talker
    {
        public bool setInactiveOnDialogueComplete = true;
        public bool gameObjectOnSetActive = true;

        public UnityEvent<bool> onSetActive;

        public void SetActive(bool active)
        {
            onSetActive.Invoke(active);
            if (gameObjectOnSetActive)
                gameObject.SetActive(active);
        }

        public override void CompleteTalking()
        {
            base.CompleteTalking();
            if (setInactiveOnDialogueComplete)
                SetActive(false);
        }
    }
}

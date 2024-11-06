using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{

    // This STS Trigger just sets up the manual player-based trigger and exposes a public function to activate and deactive
    // Only works with PlayerBased settings!
    [AddComponentMenu(Strings.COMPONENTMENU_MANUAL_TRIGGER_BY_PARENT)]
    public class ManualTriggerByParent : MonoBehaviour
    {
        TransitionController seeThroughShaderController;

        void Start()
        {
            if (this.isActiveAndEnabled)
            {
                InitializeTrigger();
            }
        }


        private void InitializeTrigger()
        {
            Transform parentTransform = parentTransform = transform;
            
            if (parentTransform != null)
            {
                seeThroughShaderController = new TransitionController(parentTransform);
            }
        }

        public void ActivateTrigger(Collider other)
        {
            if (this.isActiveAndEnabled && other.gameObject.GetComponent<SeeThroughShaderPlayer>() != null)
            {
                seeThroughShaderController.notifyOnTriggerEnter(this.transform, other.transform);
            }
        }

        public void DeactivateTrigger(Collider other)
        {
            if (this.isActiveAndEnabled && other.gameObject.GetComponent<SeeThroughShaderPlayer>() != null)
            {
                seeThroughShaderController.notifyOnTriggerExit(this.transform, other.transform);

            }            
        }

        // works even if the gameobject isn't the player itself and instead a offset proxy parent
        public void ActivateTrigger(GameObject other)
        {
            SeeThroughShaderPlayer seeThroughShaderPlayer = other.GetComponent<SeeThroughShaderPlayer>();
            if(seeThroughShaderPlayer == null)
            {
                seeThroughShaderPlayer = other.GetComponentInChildren<SeeThroughShaderPlayer>();
            }
            if (this.isActiveAndEnabled && seeThroughShaderPlayer != null)
            {
                seeThroughShaderController.notifyOnTriggerEnter(this.transform, seeThroughShaderPlayer.gameObject.transform);
            }
        }


        // works even if the gameobject isn't the player itself and instead a offset proxy parent
        public void DeactivateTrigger(GameObject other)
        {
            SeeThroughShaderPlayer seeThroughShaderPlayer = other.GetComponent<SeeThroughShaderPlayer>();
            if (seeThroughShaderPlayer == null)
            {
                seeThroughShaderPlayer = other.GetComponentInChildren<SeeThroughShaderPlayer>();
            }

            if (this.isActiveAndEnabled && seeThroughShaderPlayer != null)
            {
                seeThroughShaderController.notifyOnTriggerExit(this.transform, seeThroughShaderPlayer.gameObject.transform);

            }
        }

    }
}
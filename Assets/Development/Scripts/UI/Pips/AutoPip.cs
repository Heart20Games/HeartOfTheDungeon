using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIPips
{
    public class AutoPip : Pip
    {
        [Foldout("AutoPip", true)]
        [SerializeField] private Transform pipLabel;
        [SerializeField] private Transform pipImage;

        public PipPartitionSettings settings;
        public bool usedInWorldSpace = false;

        private Sprite[] filledSprites;
        private Sprite[] unfilledSprites;

        public bool initialized;

        public void Initialize(PipPartitionSettings settings, bool usedInWorldSpace, bool invertFilled = false)
        {
            this.usedInWorldSpace = usedInWorldSpace;
            this.invertFilled = invertFilled;

            filledSprites = (Sprite[])settings.filledSprites.Clone();
            unfilledSprites = (Sprite[])settings.unfilledSprites.Clone();

            SetWithChild("PipLabel", ref pipLabel);
            SetWithChild("PipImage", ref pipImage);

            if (pipImage != null)
            {
                pipImage.localScale *= settings.relativeScale;

                if (pipImage.TryGetComponent<PipImageChanger>(out var changer))
                {
                    changer.filled = filledSprites;
                    changer.unfilled = unfilledSprites;

                    if (!initialized)
                    {
                        changer.onColorChange.RemoveAllListeners();
                        changer.onSpriteChange.RemoveAllListeners();
#if UNITY_EDITOR
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(changer.onSpriteChange, changer.SetSprite);
                        UnityEditor.Events.UnityEventTools.AddPersistentListener(changer.onColorChange, changer.SetColor);
#else
                        changer.onSpriteChange.AddListener(changer.SetSprite);
                        changer.onColorChange.AddListener(changer.SetColor);
#endif

                        initialized = true;
                    }

                    pipImage.gameObject.TryGetComponent<Image>(out changer.image);
                    pipImage.gameObject.TryGetComponent<SpriteRenderer>(out changer.renderer);

                    if (usedInWorldSpace)
                    {
                        SwitchTo(changer, ref changer.renderer, ref changer.image, ref initialized);
                        changer.image.enabled = true;
                    }
                    else
                    {
                        SwitchTo(changer, ref changer.image, ref changer.renderer, ref initialized);
                        changer.renderer.enabled = true;
                    }
                }
            }
        }

        private void SwitchTo<Old, New>(PipImageChanger changer, ref Old oldRef, ref New newRef, ref bool initialized) where Old : Component where New : Component
        {
            if (oldRef != null)
            {
                Destroy(oldRef);
            }

            if (newRef == null)
            {
                newRef = (New)pipImage.gameObject.AddComponent(typeof(New));
            }
        }

        private void SetWithChild(string childName, ref Transform tForm)
        {
            if (tForm == null)
            {
                List<Transform> tforms = transform.GetChildsWhere((Transform child) => { return child.name == childName; });
                if (tforms.Count > 0) tForm = tforms[0];
            }
        }
    }
}

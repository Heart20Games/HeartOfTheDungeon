using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
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
        public bool useInWorldSpace = false;

        private Sprite[] filledSprites;
        private Sprite[] unfilledSprites;

        public void Initialize(PipPartitionSettings settings, bool usedInWorldSpace, bool invertFilled = false)
        {
            this.invertFilled = invertFilled;

            filledSprites = (Sprite[])settings.filledSprites.Clone();
            unfilledSprites = (Sprite[])settings.unfilledSprites.Clone();

            SetWithChild("PipLabel", ref pipLabel);
            SetWithChild("PipImage", ref pipImage);

            if (pipImage != null)
            {
                if (pipImage.TryGetComponent<PipImageChanger>(out var changer))
                {
                    changer.filled = filledSprites;
                    changer.unfilled = unfilledSprites;

                    pipImage.gameObject.TryGetComponent<Image>(out changer.image);
                    pipImage.gameObject.TryGetComponent<SpriteRenderer>(out changer.renderer);

                    if (useInWorldSpace)
                    {
                        if (changer.renderer != null)
                            Destroy(changer.renderer);

                        if (changer.image == null)
                            changer.image = (Image)pipImage.gameObject.AddComponent(typeof(Image));

                        UnityEventTools.AddPersistentListener(changer.onSpriteChange, changer.SetImageSprite);
                        UnityEventTools.AddPersistentListener(changer.onColorChange, changer.SetImageColor);
                    }
                    else
                    {
                        if (changer.image != null)
                            Destroy(changer.image);

                        if (changer.renderer == null)
                            changer.renderer = (SpriteRenderer)pipImage.gameObject.AddComponent(typeof(SpriteRenderer));

                        UnityEventTools.AddPersistentListener(changer.onSpriteChange, changer.SetRendererSprite);
                        UnityEventTools.AddPersistentListener(changer.onColorChange, changer.SetRendererColor);
                    }
                }
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

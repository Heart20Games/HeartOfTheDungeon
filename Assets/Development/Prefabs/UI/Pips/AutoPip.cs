using MyBox;
using System.Collections;
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
        public bool useInWorldSpace = false;

        public void Initialize(PipPartitionSettings settings, bool usedInWorldSpace, bool invertFilled = false)
        {
            this.invertFilled = invertFilled;

            SetWithChild("PipLabel", ref pipLabel);
            SetWithChild("PipImage", ref pipImage);

            if (pipImage != null)
            {
                pipImage.gameObject.AddComponent(usedInWorldSpace ? typeof(Image) : typeof(SpriteRenderer));
                if (pipImage.TryGetComponent<PipImageChanger>(out var changer))
                {
                    changer.filled = settings.filledSprites;
                    changer.unfilled = settings.unfilledSprites;

                    if (usedInWorldSpace && TryGetComponent<Image>(out var image))
                    {
                        changer.onSpriteChange.AddListener((Sprite sprite) => { image.sprite = sprite; });
                        changer.onColorChange.AddListener((Color color) => { image.color = color; });
                    }
                    else if (!usedInWorldSpace && TryGetComponent<SpriteRenderer>(out var renderer))
                    {
                        changer.onSpriteChange.AddListener((Sprite sprite) => { renderer.sprite = sprite; });
                        changer.onColorChange.AddListener((Color color) => { renderer.color = color; });
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

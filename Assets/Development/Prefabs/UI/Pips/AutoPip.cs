using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIPips
{
    public class AutoPip : Pip
    {
        [Foldout("AutoPip", true)]
        [SerializeField] private Transform pipLabel;
        [SerializeField] private Transform pipImage;

        public void Initialize(Sprite[] filled, Sprite[] unfilled, bool invertFilled=false)
        {
            SetWithChild("PipLabel", ref pipLabel);
            SetWithChild("PipImage", ref pipImage);

            //if (pipImage)
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

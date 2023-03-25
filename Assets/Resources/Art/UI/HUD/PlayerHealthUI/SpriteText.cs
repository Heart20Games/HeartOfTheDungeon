using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteText : MonoBehaviour
{
     void Start()
     {
         var parent = transform.parent;
 
         var parentRenderer = parent.GetComponent<Renderer>();
         var renderer = GetComponent<Renderer>();
         renderer.sortingLayerID = parentRenderer.sortingLayerID;
         renderer.sortingOrder = parentRenderer.sortingOrder;
 
         var spriteTransform = parent.transform;
         //var text = GetComponent<TMPro.TextMeshPro>();
         var pos = spriteTransform.position;
         //text.text = string.Format("{0}, {1}", pos.x, pos.y);
     }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu(Strings.COMPONENTMENU_SHADER_PROPERTY_SYNC)]
    public class ShaderPropertySync : GroupReplacementAndSyncBaseAbstract
    {


        private Transform[] listNodesLeafs;
        public bool syncContinuus = true;

        private string seeThroughShaderName;



        private Dictionary<Material, string> tempMaterials;

        protected override bool isReplacement => false;   

    }
}
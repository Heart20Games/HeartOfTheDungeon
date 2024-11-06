using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public class STSCustomTimeSetter : MonoBehaviour
    {

        public float STSCustomTime;


        void Awake()    
        {
            SetSTSCustomTime(STSCustomTime);
        }
        private void Update()
        {
            SetSTSCustomTime(STSCustomTime);
        }


        private void SetSTSCustomTime(float customTime)
        {
            Shader.SetGlobalFloat("_STSCustomTime", customTime);
        }

        /// <summary>
        /// Static function, use this function in an update loop if you dont want to use this class as a component
        /// </summary>
        /// <param name="customTime"></param>
        public static void DirectlySetSTSCustomTime(float customTime)
        {
            Shader.SetGlobalFloat("_STSCustomTime", customTime);
        }

    }
}
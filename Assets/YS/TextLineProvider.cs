using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

#if USE_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Yarn.Unity
{
    
    public class TextLineProvider : LineProviderBehaviour
    {
        public string currentID;
        public GameObject tester;
        public EventReference fmodEvent;
        private FMOD.Studio.EventInstance instance;

        StudioEventEmitter emitter;
        

        private void Awake()
        {
            instance = RuntimeManager.CreateInstance(fmodEvent);
            //instance.start();

            emitter = tester.GetComponent<StudioEventEmitter>();
        }


        /// <summary>Specifies the language code to use for text content
        /// for this <see cref="TextLineProvider"/>.
        [Language]
        public string textLanguageCode = System.Globalization.CultureInfo.CurrentCulture.Name;

        public override LocalizedLine GetLocalizedLine(Yarn.Line line)
        {
            var text = YarnProject.GetLocalization(textLanguageCode).GetLocalizedString(line.ID);
            currentID = line.ID;
            //emitter.Params.SetValue("B001 Rotta");
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            string clean = line.ID.Remove(4, 1);
            currentID = clean;
            instance.setParameterByNameWithLabel("Parameter 2", clean);
            instance.start();
            
            //tester.GetComponent<SoundScript>().coolfunctionthatfindsclips(currentID);
            return new LocalizedLine()
            {
                TextID = line.ID,
                RawText = text,
                Substitutions = line.Substitutions,
                Metadata = YarnProject.lineMetadata.GetMetadata(line.ID),
            };
        }

        public override void PrepareForLines(IEnumerable<string> lineIDs)
        {
            // No-op; text lines are always available
        }

        public override bool LinesAvailable => true;

        public override string LocaleCode => textLanguageCode;
    }
}

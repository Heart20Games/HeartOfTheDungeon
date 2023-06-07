using System;
using UnityEngine;

namespace FMODUnity
{
#if UNITY_EDITOR
    [Serializable]
    public struct EventReference
    {
        public FMOD.GUID Guid;

        public string Path;

        public static Func<string, FMOD.GUID> GuidLookupDelegate;
        public override string ToString()
        {
            return string.Format("{0} ({1})", Guid, Path);
        }

        public bool IsNull
        {
            get
            {
                return string.IsNullOrEmpty(Path) && Guid.IsNull;
            }
        }
        //#endif
        public static EventReference Find(string path)
        {
            if (GuidLookupDelegate == null)
            {
                throw new InvalidOperationException("EventReference.Find called before EventManager was initialized");
            }

            return new EventReference { Path = path, Guid = GuidLookupDelegate(path) };
        }

    }
}
#else 
    public struct EventReference { 

        public override string ToString()
        {
            return Guid.ToString();
        }

        public bool IsNull
        {
            get
            {
                return Guid.IsNull;
            }
        }
        public static EventReference Find(string path)
        {
            if (GuidLookupDelegate == null)
            {
                throw new InvalidOperationException("EventReference.Find called before EventManager was initialized");
            }

            return new EventReference { Path = path, Guid = GuidLookupDelegate(path) };
        }
    }
}
#endif

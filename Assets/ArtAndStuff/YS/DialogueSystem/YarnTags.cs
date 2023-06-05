using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "YarnTags", menuName = "Yarn Spinner/Yarn Tags", order = 1)]
public class YarnTags : ScriptableObject
{
    // General Tags
    static public string exclusionTag = "exclude";
    static public string inclusionTag = "include";

    // View Tags
    public enum ViewType { Portrait, Line, OptionList, Audio }
    [Serializable]
    public struct ViewTag
    {
        public string name;
        public ViewType type;
    }
    public List<ViewTag> viewTags = new();
    static private Dictionary<ViewType, ViewTag> tags = null;
    static public Dictionary<ViewType, ViewTag> ViewTags { get { return tags ?? InitializeViewTags(null); } }
    static public Dictionary<ViewType, ViewTag> InitializeViewTags(List<ViewTag> tagList)
    {
        tags ??= new();
        tags.Clear();
        if (tagList != null)
        {
            for (int i = 0; i < tagList.Count; i++)
            {
                ViewTag viewTag = tagList[i];
                tags[viewTag.type] = viewTag;
            }
        }
        return tags;
    }

    // Awake
    public void Initialize()
    {
        InitializeViewTags(viewTags);
    }

    // Tag Checks
    static public bool Included(string[] metaData, ViewType viewType)
    {
        if (metaData != null && ViewTags.TryGetValue(viewType, out ViewTag viewTag))
        {
            string tag = viewTag.name;
            for (int i = 0; i < metaData.Length; i++)
            {
                string meta = metaData[i];
                if (meta.StartsWith(inclusionTag) && meta.Contains(tag)) return true;
                else if (meta.StartsWith(exclusionTag) && meta.Contains(tag)) return false;
            }
            return true;
        }
        else return true;
    }

    static public bool HasTag(string[] metaData, string tag)
    {
        if (metaData != null)
        {
            foreach (string meta in metaData)
            {
                if (meta == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

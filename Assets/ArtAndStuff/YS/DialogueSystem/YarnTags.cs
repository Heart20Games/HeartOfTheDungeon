using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "YarnTags", menuName = "Yarn Spinner/Yarn Tags", order = 1)]
public class YarnTags : ScriptableObject
{
    // General Tags
    static public string exclusionTag = "exclude";
    static public string inclusionTag = "include";

    // View Tags
    public enum ViewType { Portrait, Line, OptionList, Audio, Bubble }
    [Flags] public enum Inclusion { NA=0, Included=1<<0, Excluded=1<<1 }
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

    // Include/Exclude Tag Checks

    static public void SetNodeInclusion(IEnumerable<string> metaData, List<IViewable> viewables)
    {
        IEnumerator<string> tags = metaData.GetEnumerator();
        while (tags.MoveNext())
        {
            for (int i = 0; i < viewables.Count; i++)
            {
                Inclusion includes = Includes(tags.Current, viewables[i].GetViewType());
                viewables[i].SetViewable(includes);
            }
        }
    }

    static public Inclusion Included(string[] metaData, ViewType viewType)
    {
        if (metaData != null && ViewTags.TryGetValue(viewType, out ViewTag viewTag))
        {
            for (int i = 0; i < metaData.Length; i++)
            {
                Inclusion inclusion = WhichInclusionTag(metaData[i], viewTag.name);
                if ((inclusion & Inclusion.NA) != 0) return inclusion;
            }
            return Inclusion.NA;
        }
        else return Inclusion.NA;
    }

    static public Inclusion Includes(string meta, ViewType viewType)
    {
        if (ViewTags.TryGetValue(viewType, out ViewTag viewTag))
        {
            return WhichInclusionTag(meta, viewTag.name);
        }
        else return Inclusion.NA;
    }

    static public Inclusion WhichInclusionTag(string meta, string tag)
    {
        if (IsPairTag(meta, inclusionTag, tag)) return Inclusion.Included;
        else if (IsPairTag(meta, exclusionTag, tag)) return Inclusion.Excluded;
        else return Inclusion.NA;
    }

    // Tag Checks

    static public bool HasTag(string[] metaData, string tag)
    {
        if (metaData == null) return false;
        foreach (string meta in metaData)
        {
            if (IsTag(meta, tag)) return true;
        }
        return false;
    }

    static public bool HasPairTag(string[] metaData, string key, string value)
    {
        if (metaData == null) return false;
        foreach (string meta in metaData)
        {
            if (IsPairTag(meta, key, value)) return true;
        }
        return false;
    }

    static public bool HasPairTag(string[] metaData, string key, out string value, string defaultValue="")
    {
        value = defaultValue;
        if (metaData == null) return false;
        foreach (string meta in metaData)
        {
            if (IsPairTag(meta, key, out value, defaultValue)) return true;
        }
        return false;
    }

    static public bool IsTag(string meta, string value)
    {
        return meta == value;
    }

    static public bool IsPairTag(string meta, string key, string value)
    {
        return meta.StartsWith(key) && meta.Contains(value);
    }
    
    static public bool IsPairTag(string meta, string key, out string value, string defaultValue="")
    {
        value = defaultValue;
        if (meta.StartsWith(key))
        {
            value = meta.Substring(key.Length+1, meta.Length-(key.Length+1));
            return true;
        }
        return false;
    }

    static public bool ShouldIncludeView(string[] metadata, ViewType viewType, Inclusion viewable, bool includeByDefault=true)
    {
        Inclusion inclusion = Included(metadata, viewType);
        bool exclude = (inclusion & Inclusion.Excluded) != 0;
        bool include = (inclusion & Inclusion.Included) != 0;
        bool nodeViewable = (viewable & Inclusion.Excluded) == 0 && (includeByDefault || (viewable & Inclusion.NA) == 0);
        return !(exclude || (!nodeViewable && !include));
    }
}

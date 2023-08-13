using System;
using UnityEditor;
using UnityEngine;


// Credited to James Lafritz via Dev Genius: https://blog.devgenius.io/making-the-inspector-look-better-175baf39ada0


[AttributeUsage(AttributeTargets.Field)]
public class TagAttribute : PropertyAttribute { }

[AttributeUsage(AttributeTargets.Field)]
public class LayerAttribute : PropertyAttribute { }

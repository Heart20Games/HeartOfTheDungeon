using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//[CustomEditor(typeof(Impact))]
public class ImpactInspector : Editor
{
    public VisualTreeAsset inspectorUXML;

    public override VisualElement CreateInspectorGUI()
    {
        // VisualElement as root of inspector UI.
        VisualElement inspector = new VisualElement();

        // Simple Label
        inspector.Add(new Label("Impact"));

        // Clone a visual tree from UXML
        inspectorUXML.CloneTree(inspector);

        // Return finished Inspector UI
        return inspector;
    }

}

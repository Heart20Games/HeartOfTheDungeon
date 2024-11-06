using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
//using UnityEngine.UI;
using static ShaderCrew.SeeThroughShader.GeneralUtils;

public class SeeThroughShaderQuickSetup : EditorWindow
{

    private Vector2 scrollPosition;
    private Texture2D stslogo;
    private Texture2D circlemodepic;
    private Texture2D anglemodepic;
    private Texture2D nonemodepic;
    private Texture2D curvemodepic;
    private Texture2D dissolvemaskmodepic;
    private Texture2D raycastmodepic;
    private Texture2D dropfieldpic;
    private Texture2D overviewpic;
    private Texture2D setuppic;
    private Texture2D samplespic;
    private Texture2D resetpic;
    private Texture2D gopic;
    private Texture2D documentationpic;
    private Texture2D notreadypic;
    private Texture2D readypic;


    private GameObject prefab;
    private GameObject scenePlayer;
    private GameObject raycastElement;
    private LayerMask layerMasks;
    private Material refMaterial;

    List<GameObject> listScenePlayers = new List<GameObject>();
    List<GameObject> listPrefabs = new List<GameObject>();
    private GUIStyle replacementStyle = new GUIStyle();
    private Material materialField;

    private Material refMaterialCircle;
    private Material refMaterialAngle;
    private Material refMaterialNone;
    private Material refMaterialCurve;
    private Material refMaterialDissolveMask;
    private Material refMaterialRaycast;
    private Texture2D urppic;
    private PlayersPositionManager posManager;
    private GlobalShaderReplacement gloRepl;
    private UnityVersionRenderPipelineShaderInfo unityVersionRenderPipelineShader;
    Dictionary<string, string> UnityToSTSShaderNameMapping;

    Dictionary<string, Shader> UnityToSTSShaderMapping;


    GUIStyle stepHeaderStyle;
    GUIStyle centerLabelStyle;
    GUIStyle textAreaStyle;
    GUIStyle m_LinkStyle;
    GUIStyle headerStyle;
    GUIStyle grayHeaderStyle;
    GUIStyle bigTextStyle;
    GUIStyle bigErrorStyle;
    GUIStyle textAreaBoldStyle;
    GUIStyle step2TextStyle;
    GUIStyle selectedPresetStyle;
    GUIStyle replacementStyleOv;
    GUIStyle descriptiontextstyle;
    GUIStyle redTextStyle;
    GUIStyle greenTextStyle;

    //string richColor;

    Color buttonColor;

    bool m_FirstTimeApply = true;

    bool LinkLabel(GUIContent label)
    {
        GUIStyle m_LinkStyle = new GUIStyle(EditorStyles.label);
        m_LinkStyle.wordWrap = false;
        m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
        m_LinkStyle.stretchWidth = false;
        m_LinkStyle.contentOffset = new Vector2(15, 0);
        m_LinkStyle.alignment = TextAnchor.MiddleCenter;
        return GUILayout.Button(label, m_LinkStyle);
    }

    Material[] mats;
    //private bool isCorrectShaderUsed;
    private GUIStyle imgstyle = new GUIStyle();
    private bool justSavedGroup;
    private bool justSavedPlayers;
    private AnimBool globalGroupSaveAnim;
    private AnimBool globalGroupSaveAnimAddedPlayers;
    private float fadeGroupValuePlayers;
    //private float fadeGroupValueAddedPlayers;
    private bool isReadyToGo = false;
    private Vector2 scrollPosition2;
    private Vector2 scrollPosition3;
    private Vector2 scrollPosition4;
    private Vector2 scrollPosition5;
    private GUIStyle scrollViewStyle;
    private Dictionary<Button, GameObject> dictButtonGameObject;
    private GameObject sceneParent;
    private List<GameObject> listSceneParents = new List<GameObject>();
    private List<GameObject> listRaycastElements = new List<GameObject>();

    int tab = 0;
    // Start is called before the first frame update    
    private Rect dropAreaGroup;
    [MenuItem("Window/See-through Shader/SeeThroughShader QuickSetup")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SeeThroughShaderQuickSetup));
        var window = GetWindow<SeeThroughShaderQuickSetup>("See-through Shader/SeeThroughShader QuickSetup");
        //var window = GetWindow<MyWindow>("SeeThroughShaderSetup");

        // Set the minimum size
        window.maxSize = new Vector2(2600, 1820);
        window.minSize = new Vector2(1290, 910);

    }

    void OnGUI()
    {


        if (m_FirstTimeApply)
        {
            m_FirstTimeApply = false;
            DoSetup();
            m_FirstTimeApply = false;
        }
        //DrawHeaderArea();
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f)); // Dark grey color
        backgroundTexture.Apply();

        // Set up the GUIStyle
        scrollViewStyle = new GUIStyle();
        scrollViewStyle.normal.background = backgroundTexture;
        scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, scrollViewStyle);
        DrawOverview();
        EditorGUILayout.BeginHorizontal();

        DrawDropFields();
        DrawDropFields2();

        EditorGUILayout.EndVertical();
        DrawSetupField2();
        //DrawSetupButtons();
        GUILayout.EndScrollView();


    }

    void DoSetup()
    {
        m_LinkStyle = new GUIStyle(EditorStyles.label);
        m_LinkStyle.wordWrap = false;
        m_LinkStyle.stretchWidth = false;

        headerStyle = new GUIStyle();
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 15;

        grayHeaderStyle = new GUIStyle();
        grayHeaderStyle.fontSize = 14;
        grayHeaderStyle.alignment = TextAnchor.MiddleCenter;
        grayHeaderStyle.wordWrap = true;


        bigTextStyle = new GUIStyle();

        bigTextStyle.fontStyle = FontStyle.Bold;
        bigTextStyle.fontSize = 15;
        bigTextStyle.alignment = TextAnchor.MiddleCenter;

        bigErrorStyle = new GUIStyle();
        bigErrorStyle.fontStyle = FontStyle.Bold;
        bigErrorStyle.fontSize = 13;
        bigErrorStyle.alignment = TextAnchor.MiddleCenter;
        bigErrorStyle.wordWrap = true;

        textAreaStyle = new GUIStyle();

        textAreaStyle.padding.left = 20;
        textAreaStyle.padding.right = 20;
        textAreaStyle.normal.textColor = Color.white;
        textAreaStyle.wordWrap = true;
        textAreaStyle.fontSize = 12;
        textAreaStyle.richText = false;

        descriptiontextstyle = textAreaStyle;


        textAreaBoldStyle = new GUIStyle();
        textAreaBoldStyle.normal.textColor = Color.white;
        textAreaBoldStyle.padding.left = 20;
        textAreaBoldStyle.padding.right = 20;
        textAreaBoldStyle.fontStyle = FontStyle.Bold;
        textAreaBoldStyle.wordWrap = true;
        textAreaBoldStyle.fontSize = 15;

        stepHeaderStyle = new GUIStyle();
        stepHeaderStyle.fontStyle = FontStyle.Bold;
        stepHeaderStyle.alignment = TextAnchor.MiddleCenter;
        //bigTextStyle2.fontSize = 16;
        centerLabelStyle = new GUIStyle();
        centerLabelStyle.normal.textColor = Color.gray;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        //textColor = Color.white;
        //oriCol = EditorStyles.label.normal.textColor;

        step2TextStyle = new GUIStyle();

        step2TextStyle.fontSize = 15;
        step2TextStyle.padding.left = 20;
        step2TextStyle.padding.right = 20;
        step2TextStyle.alignment = TextAnchor.MiddleCenter;
        step2TextStyle.wordWrap = true;

        selectedPresetStyle = new GUIStyle();
        selectedPresetStyle.border = new RectOffset(10, 10, 10, 10);
        redTextStyle = new GUIStyle(GUI.skin.label);
        redTextStyle.normal.textColor = Color.white;
        redTextStyle.fontSize = 15;
        redTextStyle.fontStyle = FontStyle.Bold;

        greenTextStyle = new GUIStyle(GUI.skin.label);
        greenTextStyle.normal.textColor = Color.white;
        greenTextStyle.fontSize = 15;
        greenTextStyle.fontStyle = FontStyle.Bold;

        textAreaBoldStyle = new GUIStyle(GUI.skin.button);
        textAreaBoldStyle.normal.textColor = Color.white;
        textAreaBoldStyle.fontSize = 15;
        textAreaBoldStyle.fontStyle = FontStyle.Bold;
        textAreaBoldStyle.padding = new RectOffset(20, 20, 20, 20);


        if (EditorGUIUtility.isProSkin)
        {

            m_LinkStyle.normal.textColor = new Color(1f, 0.3f, 0.3f, 1);
            headerStyle.normal.textColor = Color.white;
            grayHeaderStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
            textAreaStyle.normal.textColor = Color.white;
            bigErrorStyle.normal.textColor = new Color(1f, 0.3f, 0.3f, 1);
           // richColor = "silver";
            stepHeaderStyle.normal.textColor = Color.white;
            step2TextStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
            bigTextStyle.normal.textColor = Color.white;
            buttonColor = new Color(0.6f, 0.6f, 0.6f, 1);
        }
        else
        {
            m_LinkStyle.normal.textColor = new Color(1f, 0.3f, 0.3f, 1);
            headerStyle.normal.textColor = Color.black;
            grayHeaderStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f, 1);
            textAreaStyle.normal.textColor = Color.black;
            bigErrorStyle.normal.textColor = new Color(0.6f, 0.0f, 0.0f, 1);
            //richColor = "#161616";
            stepHeaderStyle.normal.textColor = Color.black;
            step2TextStyle.normal.textColor = Color.black;
            bigTextStyle.normal.textColor = Color.black;
            buttonColor = new Color(0.8f, 0.8f, 0.8f, 1);
        }
        stslogo = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/logo-with-outlineBig.png") as Texture2D;
        circlemodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/circlemode.PNG") as Texture2D;
        anglemodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/anglemode.PNG") as Texture2D;
        nonemodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/nonemode.PNG") as Texture2D;
        curvemodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/curvemodepic.PNG") as Texture2D;
        dissolvemaskmodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/dissolvemaskpic.PNG") as Texture2D;
        raycastmodepic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/raycastmode.PNG") as Texture2D;
        //dropfieldpic = EditorGUIUtility.Load("Hell Tap Entertainment/Meshkit/Icons/rebuild.png") as Texture2D;
        overviewpic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Editor/Resources/logo-with-outline - Kopie.png") as Texture2D;
        setuppic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Editor/Resources/logo-with-outline - Kopie.png") as Texture2D;
        samplespic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/samples.PNG") as Texture2D;
        resetpic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/reset.png") as Texture2D;
        documentationpic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/documentation.png") as Texture2D;
        notreadypic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/notready.png") as Texture2D;
        readypic = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/Logo/ready.png") as Texture2D;
        //gopic = EditorGUIUtility.Load("Assets/Editor/gobtn.png") as Texture2D;
        refMaterialCircle = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerCircle.mat") as Material;
        refMaterialAngle = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerAngle.mat") as Material;
        refMaterialCurve = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerCurve.mat") as Material;
        refMaterialNone = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerNone.mat") as Material;
        refMaterialDissolveMask = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerDissolveMask.mat") as Material;
        refMaterialRaycast = EditorGUIUtility.Load("Packages/com.shadercrew.seethroughshader.core/Scripts/Resources/STSReferenceMaterials/refMaterialPlayerRaycast.mat") as Material;


        refMaterial = refMaterialCircle;
        posManager = FindObjectOfType<PlayersPositionManager>();
        gloRepl = FindObjectOfType<GlobalShaderReplacement>();

        Color origColor = EditorStyles.label.normal.textColor;
        EditorStyles.label.normal.textColor = Color.white;

        replacementStyle.normal.textColor = Color.white;
        replacementStyle.alignment = TextAnchor.MiddleCenter;
        replacementStyle.fontStyle = FontStyle.Bold;
        //replacementStyle.fontSize = 10;

        replacementStyleOv = new GUIStyle();
        replacementStyleOv.normal.textColor = Color.white;
        replacementStyleOv.alignment = TextAnchor.MiddleCenter;
        replacementStyleOv.fontStyle = FontStyle.Bold;
        //replacementStyleOv.fontSize = 10;
        globalGroupSaveAnim = new AnimBool(false);
        globalGroupSaveAnimAddedPlayers = new AnimBool(false);

        globalGroupSaveAnim.valueChanged.AddListener(Repaint);
        globalGroupSaveAnimAddedPlayers.valueChanged.AddListener(Repaint);

        unityVersionRenderPipelineShader = getUnityVersionAndRenderPipelineCorrectedShaderString();
        mats = Resources.LoadAll("STSReferenceMaterials", typeof(Material)).Cast<Material>().ToArray();
        UpgradeMaterialsToCurrentRP();
        List<Material> matListNotCorrectShader = new List<Material>();
        if (mats != null)
        {
            foreach (Material mat in mats)
            {
                //if (mat.shader.name != unityVersionRenderPipelineShader.versionAndRPCorrectedShader)
                if (!UnityToSTSShaderNameMapping.Values.Contains(mat.shader.name))
                {
                    //isCorrectShaderUsed = false;
                    matListNotCorrectShader.Add(mat);
                }
            }
        }

        if (matListNotCorrectShader.Count > 0)
        {

            OpenRPConverter();
        }
    }


    void DrawDropFields()
    {
        GUIStyle resetStyle = new GUIStyle(GUI.skin.box);
        GUI.skin.box = resetStyle;
        Rect rectt;
        rectt = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.Box(rectt, GUIContent.none);
        EditorUtils.Header("1", replacementStyle);
        EditorUtils.Header("Add Scene Players / Prefabs", replacementStyle);        
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop Prefab Here");

        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
                        
                        if (draggedObject is GameObject && PrefabUtility.GetPrefabAssetType(draggedObject) != PrefabAssetType.NotAPrefab)
                        {
                            prefab = (GameObject)draggedObject;
                            if (listPrefabs.Contains(prefab) == false)
                            {

                                listPrefabs.Add(prefab);
                                justSavedGroup = true;
                            }
                            
                        }
                    }
                }
                break;
        }        
        GUI.Box(rectt, GUIContent.none);


        scenePlayer = EditorGUILayout.ObjectField("ScenePlayer", scenePlayer, typeof(GameObject), false) as GameObject;
        dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop Players in Scene Here");

        Event evt2 = Event.current;
        switch (evt2.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt2.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt2.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
                        // Handle the prefab object
                        if (draggedObject is GameObject)
                        {
                            scenePlayer = (GameObject)draggedObject;
                            if (listScenePlayers.Contains(scenePlayer) == false)
                            {
                                listScenePlayers.Add(scenePlayer);
                                justSavedGroup = true;
                            }


                            // Do something with the prefab
                        }
                    }
                }
                break;
        }
        GUILayout.Space(7);
        GUI.Box(rectt, GUIContent.none);
        EditorUtils.DrawUILineGray(2, 0);

        // Create a texture and set its color
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f)); // Dark grey color
        backgroundTexture.Apply();

        // Set up the GUIStyle
        scrollViewStyle = new GUIStyle();
        scrollViewStyle.normal.background = backgroundTexture;
        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, scrollViewStyle, GUILayout.MaxWidth(800), GUILayout.Height(100));
        
        if (listScenePlayers.Count > 0)
        {
            
            foreach (GameObject item in listScenePlayers)
            {
                //
                GUILayout.BeginHorizontal();
                GUILayout.Label("> " + item.name);
                if (GUILayout.Button(new GUIContent("Remove"), GUILayout.MaxWidth(80), GUILayout.MaxHeight(20)))
                {

                    try
                    {

                        listScenePlayers.Remove(item);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Player removed " + e.Message);



                    }

                }
                //GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();
            }
            
        }
        else
        {
            GUILayout.Label("No scene players added");
        }

        if (listPrefabs.Count > 0)
        {
            
            foreach (GameObject item in listPrefabs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("> " + item.name);
                if (GUILayout.Button(new GUIContent("Remove"), GUILayout.MaxWidth(80), GUILayout.MaxHeight(20)))
                {

                    try
                    {

                        listPrefabs.Remove(item);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Player removed " + e.Message);



                    }

                }
                GUILayout.EndHorizontal();
            }
            
        }
        else
        {
            GUILayout.Label("No prefab players added");
        }
        

        if (justSavedGroup)
        {
            Thread myThread = new Thread(ThreadFunction1);
            myThread.Start();
            // ShowSuccessMessage();
            Repaint();

        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();



    }

    void DrawDropFields2()
    {
        Rect rectt;
        rectt = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(800));
        GUI.Box(rectt, GUIContent.none);
        EditorUtils.Header("2", replacementStyle);
        EditorUtils.Header("Set Layer(s) of Building", replacementStyle);
        layerMasks = EditorGUILayout.MaskField("Layer Mask", layerMasks, UnityEditorInternal.InternalEditorUtility.layers, GUILayout.MaxWidth(300));

        tab = GUILayout.Toolbar(tab, new string[] { "Global", "Group" });
        switch (tab)
        {
            case 0:
                if (refMaterial != null && refMaterial == refMaterialRaycast)
                {
                    EditorUtils.Header("Add Buildings/Elements/Parents to be Raycasted", replacementStyle);
                    //GUILayout.Label("Drag and Drop Player Prefabs");
                    //EditorGUILayout.LabelField("Material Field");
                    raycastElement = EditorGUILayout.ObjectField("Buildings/Elements/Parents", raycastElement, typeof(GameObject), false) as GameObject;
                    Rect dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
                    GUI.Box(dropArea, "Drag and Drop Elements to be Raycasted here");

                    Event evt = Event.current;
                    switch (evt.type)
                    {
                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            if (!dropArea.Contains(evt.mousePosition))
                                break;

                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                Event.current.Use();
                                foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                                {
                                    // Handle the prefab object
                                    if (draggedObject is GameObject)
                                    {
                                        raycastElement = (GameObject)draggedObject;
                                        if (listRaycastElements.Contains(raycastElement) == false)
                                        {

                                            listRaycastElements.Add(raycastElement);
                                            justSavedGroup = true;
                                        }
                                        // Do something with the prefab
                                    }
                                }
                            }
                            break;
                    }
                }
                GUILayout.Label("> Global Option: " +
                    "The Shader is applied over the whole scene on the Layer you set in the Layermask. \n" +                    
                    "Group Option: The Shader is applied on specific GameObjects and their childObjects on the Layer you set in the Layermask", EditorStyles.boldLabel);
                GUI.Box(rectt, GUIContent.none);
                EditorUtils.DrawUILineGray(2, 0);
                // Create a texture and set its color
                Texture2D backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f)); // Dark grey color
                backgroundTexture.Apply();
                scrollViewStyle = new GUIStyle();
                scrollViewStyle.normal.background = backgroundTexture;
                scrollPosition5 = GUILayout.BeginScrollView(scrollPosition5, scrollViewStyle, GUILayout.MaxWidth(800), GUILayout.Height(100));
                if (listRaycastElements.Count > 0)
                {
                    
                    
                    foreach (GameObject item in listRaycastElements)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("> " + item.name);
                        if (GUILayout.Button(new GUIContent("Remove"), GUILayout.MaxWidth(80), GUILayout.MaxHeight(20)))
                        {

                            try
                            {

                                listRaycastElements.Remove(item);
                                GUILayout.EndHorizontal();
                                break;
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log("scene raycastelement got removed" + e.Message);
                            }

                        }
                        GUILayout.EndHorizontal();
                    }
                    
                    
                }
                else
                {
                    GUILayout.Label("No raycast gameobjects/elements/parents added");
                }
                GUILayout.EndScrollView();
                break;
            case 1:
                
                EditorUtils.Header("Add Building / Parent GameObject(s)", replacementStyle);
                sceneParent = EditorGUILayout.ObjectField("Scene Buildings/Parents", sceneParent, typeof(GameObject), false) as GameObject;
                GUILayout.Space(7);
                dropAreaGroup = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
                GUI.Box(dropAreaGroup, "Drag and Drop Building(s) or their parent GameObjects in Scene Here");
                Event evt2 = Event.current;
                Event evt3 = Event.current;
                switch (evt2.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dropAreaGroup.Contains(evt2.mousePosition))
                            break;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt2.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            Event.current.Use();

                            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                            {
                                // Handle the prefab object
                                if (draggedObject is GameObject)
                                {
                                    sceneParent = (GameObject)draggedObject;
                                    if (listSceneParents.Contains(sceneParent) == false)
                                    {
                                        listSceneParents.Add(sceneParent);
                                        justSavedGroup = true;
                                    }


                                    // Do something with the prefab
                                }
                            }
                        }
                        break;
                }
                GUILayout.Label("Global Option: " +
                    "The Shader is applied over the whole scene on the Layer you set in the Layermask. \n" +                    
                    "> Group Option: The Shader is applied on specific GameObjects and their childObjects on the Layer you set in the Layermask", EditorStyles.boldLabel);
                GUI.Box(rectt, GUIContent.none);
                EditorUtils.DrawUILineGray(2, 0);
                // Create a texture and set its color
                backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f)); // Dark grey color
                backgroundTexture.Apply();
                scrollViewStyle = new GUIStyle();
                scrollViewStyle.normal.background = backgroundTexture;
                scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, scrollViewStyle, GUILayout.MaxWidth(800), GUILayout.Height(100));
                if (listSceneParents.Count > 0)
                {
                    
                    
                    foreach (GameObject item in listSceneParents)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("> " + item.name);
                        if (GUILayout.Button(new GUIContent("Remove"), GUILayout.MaxWidth(80), GUILayout.MaxHeight(20)))
                        {

                            try
                            {

                                listSceneParents.Remove(item);
                                GUILayout.EndHorizontal();
                                break;
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log("scene parent go removed " + e.Message);



                            }

                        }
                        GUILayout.EndHorizontal();
                    }
                    
                    
                }
                else
                {
                    GUILayout.Label("No scene gameobjects/parents added");
                }
                GUILayout.EndScrollView();
                break;

        }
                      

        GUIStyle greenTextStyle = new GUIStyle(GUI.skin.label);
        greenTextStyle.normal.textColor = Color.green;
        globalGroupSaveAnimAddedPlayers.target = justSavedGroup;


        
        EditorGUILayout.LabelField((justSavedGroup ? "Added!" : ""), greenTextStyle);

        
        GUILayout.EndVertical();
        if (layerMasks.value != 0 && tab == 0 && refMaterial != refMaterialRaycast && (listScenePlayers.Count > 0 || listPrefabs.Count > 0))
        {
            isReadyToGo = true;

        }
        else if (layerMasks.value != 0 && tab == 0 && refMaterial == refMaterialRaycast && listRaycastElements.Count > 0 && (listScenePlayers.Count > 0 || listPrefabs.Count > 0))
        {
            isReadyToGo = true;
        }
        else if (layerMasks.value != 0 && tab == 1 && listSceneParents.Count > 0 && (listScenePlayers.Count > 0 || listPrefabs.Count > 0))
        {
            isReadyToGo = true;
        }
        else
        {
            isReadyToGo = false;
        }


    }

    void DrawOverview()
    {
        Rect rectt = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.Box(rectt, GUIContent.none);
        EditorUtils.Header("Setup Overview", replacementStyle);
        descriptiontextstyle.fontSize = 13;
        GUILayout.TextArea(
            "1 Drag and Drop your Scene Players or prefabs onto the Fields & " +
            "2 Select Layer(s) of the Buildings or Elements you want to Dissolve. \n" +
            "    Select Global to cover the whole scene or Group for a per GameObject and beneath ChildObjects approach. " +
            "For Group drag and drop your GameObjects/parent GameObjects onto the dropfield. \n" +
            "3 - Select a preset or leave option 1 and press the Go! Button. \n" +            
            "Note:This is the most simple Setup for SeeThroughShader , no Triggers or Zones are used. \n" +
            "please consult the Documentation / Support / Sample scenes for other options. \n" +            
            "Import Button: Show the Window for Importing the Sample scenes. " +            
            "Reset Button: Resets this setup window BUT not already created scene Objects"

            , descriptiontextstyle);



        EditorUtils.DrawUILineGray(2, 0);

        EditorGUILayout.Space(10);
        

        List<Material> matListNotCorrectShader = new List<Material>();
        if (mats != null)
        {
            foreach (Material mat in mats)
            {
                
                if (!UnityToSTSShaderNameMapping.Values.Contains(mat.shader.name))
                {
                    //isCorrectShaderUsed = false;
                    matListNotCorrectShader.Add(mat);
                }
            }
        }

        if (matListNotCorrectShader.Count > 0)
        {
            GUILayout.Label("Please convert your Materials to URP/HDRP with the Rendering Pipeline Converter and Press Go", EditorStyles.largeLabel);
        }

        if (unityVersionRenderPipelineShader.renderPipeline.Contains("URP") || unityVersionRenderPipelineShader.renderPipeline.Contains("HDRP"))
        {

            if (GUILayout.Button(new GUIContent("Show Converter"), EditorStyles.miniButton)) // Replace "Icon1" with your image
            {
                OpenRPConverter();

            }

        }

        EditorUtils.Header("Status", replacementStyleOv);
        GUILayout.BeginVertical();
        GUILayout.Label(unityVersionRenderPipelineShader.renderPipeline + " " + unityVersionRenderPipelineShader.unityVersion);
        if (layerMasks == 0)
        {
            EditorGUILayout.HelpBox("You didn't select any layer! Shader replacement won't happen!", MessageType.Warning);
        }
        else if (listPrefabs.Count == 0 && listScenePlayers.Count == 0)
        {
            EditorGUILayout.HelpBox("You didn't select any player! Drop your scene player(s) or prefab(s) onto the drop field", MessageType.Warning);
        }
        else if (tab == 0 && refMaterial == refMaterialRaycast && listRaycastElements.Count == 0)
        {
            EditorGUILayout.HelpBox("You didn't select any scene GameObject to Raycast! Drop your scene Buildings or GameObject Parents onto the drop field", MessageType.Warning);
        }
        else if (tab == 1 && listSceneParents.Count == 0)
        {
            EditorGUILayout.HelpBox("You didn't select any scene parent GameObject! Drop your scene Buildings or GameObject Parents onto the drop field", MessageType.Warning);
        }
        else
        
        {

            EditorGUILayout.HelpBox("Ready to Go!", MessageType.Info);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        
        if (!isReadyToGo)
        {
            if (GUILayout.Button(new GUIContent("Not Ready!"), textAreaBoldStyle, GUILayout.MinWidth(150), GUILayout.MaxHeight(55))) // Replace "Icon1" with your image
            {
                if (isReadyToGo)
                {

                    //CreateSTSObjects();
                }

            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Go!"), textAreaBoldStyle, GUILayout.MinWidth(150), GUILayout.MaxHeight(55))) // Replace "Icon1" with your image
            {
                if (isReadyToGo)
                {

                    CreateSTSObjects();
                }

            }
        }
        
        if (GUILayout.Button(new GUIContent("Samples"), textAreaBoldStyle, GUILayout.MinWidth(150), GUILayout.MaxHeight(55))) // Replace "Icon1" with your image
        {
            ShowSamplesImportUsage();

        }
        
        if (GUILayout.Button(new GUIContent("Documentation"), textAreaBoldStyle, GUILayout.MinWidth(150), GUILayout.MaxHeight(55))) // Replace "Icon1" with your image
        {
            
            string path = "Packages/com.shadercrew.seethroughshader.core/Documentation/See_throughShaderDocumentation_v1_8_5.pdf";

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            Selection.activeObject = obj;

            EditorGUIUtility.PingObject(obj);

        }
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Reset"), textAreaBoldStyle, GUILayout.MinWidth(150), GUILayout.MaxHeight(55))) // Replace "Icon1" with your image
        {
            ResetSetup();

        }
        //GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndVertical();
    }



    void DrawSetupField2()
    {


        
        EditorUtils.Header("3", replacementStyle);
        EditorUtils.Header("Optional Preset selection", replacementStyleOv);

        EditorGUILayout.BeginHorizontal(); // Begin main horizontal group
        {
            
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialCircle, circlemodepic, "Circle", ref refMaterial);
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialNone, nonemodepic, "None", ref refMaterial);
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialAngle, anglemodepic, "Angle", ref refMaterial);
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialCurve, curvemodepic, "Curve", ref refMaterial);
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialDissolveMask, dissolvemaskmodepic, "Dissolve Mask", ref refMaterial);
            GUILayout.FlexibleSpace();
            DrawMaterialButton(refMaterialRaycast, raycastmodepic, "Raycast", ref refMaterial);
            GUILayout.FlexibleSpace();

        }
        EditorGUILayout.EndHorizontal(); // End main horizontal group


    }

    void DrawMaterialButton(Material currentMaterial, Texture buttonIcon, string buttonTooltip, ref Material selectedMaterial)
    {

        GUI.color = (selectedMaterial == currentMaterial) ? Color.green : Color.white;
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(75), GUILayout.MaxHeight(75)); // Begin inner horizontal
        {
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(buttonIcon, buttonTooltip), GUILayout.MinWidth(200), GUILayout.MaxHeight(150)))
            {
                selectedMaterial = currentMaterial;
            }
            GUILayout.FlexibleSpace();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal(); // End inner horizontal
    }

    void DrawSetupButtons()
    {

        
        Rect rectt = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUI.Box(rectt, GUIContent.none);

        
        if (justSavedPlayers)
        {
            fadeGroupValuePlayers = 1;
        }
        else
        {
            fadeGroupValuePlayers = 0;
        }

       
        globalGroupSaveAnim.target = justSavedPlayers;
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
      
        if (isReadyToGo)
        {

            EditorGUILayout.LabelField((isReadyToGo ? "Ready!" : "Not Ready"), greenTextStyle);
        }
        else
        {

            EditorGUILayout.LabelField((isReadyToGo ? "Ready!" : "Not Ready"), redTextStyle);
        }
        if (EditorGUILayout.BeginFadeGroup(fadeGroupValuePlayers))
        {
            
            EditorGUILayout.LabelField("Status: " + (justSavedPlayers ? "Saved!" : "Not Saved"), greenTextStyle);
        }
        EditorGUILayout.EndFadeGroup();


        
        rectt = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.BeginVertical();
        GUI.Box(rectt, GUIContent.none);


        if (GUILayout.Button(new GUIContent("GO!"), GUILayout.MinWidth(100), GUILayout.MaxHeight(100))) // Replace "Icon1" with your image
        {
            if (isReadyToGo)
            {

                CreateSTSObjects();
            }

        }


        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(samplespic, textAreaBoldStyle, GUILayout.MinWidth(100), GUILayout.MaxHeight(85))) // Replace "Icon1" with your image
        {
            ShowSamplesImportUsage();

        }
        if (GUILayout.Button(documentationpic, textAreaBoldStyle, GUILayout.MinWidth(100), GUILayout.MaxHeight(85))) // Replace "Icon1" with your image
        {
            
            string path = "Packages/com.shadercrew.seethroughshader.core/Documentation/See_throughShaderDocumentation_v1_8_5.pdf";

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            Selection.activeObject = obj;

            EditorGUIUtility.PingObject(obj);

        }
        if (GUILayout.Button(resetpic, textAreaBoldStyle, GUILayout.MinWidth(100), GUILayout.MaxHeight(85))) // Replace "Icon1" with your image
        {
            ResetSetup();

        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }


    private bool CreateSTSObjects()
    {
        GameObject newGameObject = null;
        posManager = FindObjectOfType<PlayersPositionManager>();        
        if (posManager != null && posManager.playableCharacters == null)
        {
            posManager.playableCharacters = new List<GameObject>();
        }
        if ((listPrefabs.Count > 0 || listScenePlayers.Count > 0) && layerMasks != 0)
        {
            if (mats != null)
            {
                foreach (Material mat in mats)
                {
                    //if (mat.shader.name != unityVersionRenderPipelineShader.versionAndRPCorrectedShader)
                    if (!UnityToSTSShaderMapping.Values.Contains(mat.shader))
                    {
                        mat.shader = UnityToSTSShaderMapping.TryGetValue(mat.shader.name, out Shader value) ? value : UnityToSTSShaderMapping[SeeThroughShaderConstants.STS_SHADER_DEFAULT_KEY];
                    }
                }
            }

            if (posManager != null)
            {
                newGameObject = posManager.gameObject;
            }
            else if (posManager == null && gloRepl != null)
            {
                newGameObject = gloRepl.gameObject;
            }
            else
            {
                newGameObject = new GameObject("See-through Shader Manager");

            }

            if (gloRepl == null && tab == 0)
            {
                
                newGameObject.AddComponent<GlobalShaderReplacement>();
                gloRepl = newGameObject.GetComponent<GlobalShaderReplacement>();
            }
            

            if (posManager == null)
            {
                if (newGameObject != null)
                {
                    newGameObject.AddComponent<PlayersPositionManager>();
                    newGameObject.GetComponent<PlayersPositionManager>().playableCharacters = new List<GameObject>();
                    posManager = newGameObject.GetComponent<PlayersPositionManager>();
                }
            }

            if (gloRepl != null && posManager != null)
            {
                gloRepl.layerMasksWithReplacement = layerMasks.value;
                gloRepl.referenceMaterial = refMaterial;

                foreach (GameObject item in listScenePlayers)
                {

                    if (!posManager.playableCharacters.Contains(item))
                    {

                        posManager.playableCharacters.Add(item);
                    }
                }



            }
            else if (posManager != null && tab == 1 && listSceneParents.Count > 0)
            {
                
                foreach (GameObject item in listScenePlayers)
                {

                    if (!posManager.playableCharacters.Contains(item))
                    {

                        posManager.playableCharacters.Add(item);
                    }
                }
            }

            if (listPrefabs.Count > 0)
            {
                foreach (GameObject item in listPrefabs)
                {
                    if (item.GetComponent<PrefabInstance>() == null)
                    {

                        item.AddComponent<PrefabInstance>();
                    }
                }
            }
            if (tab == 1 && listSceneParents.Count > 0)
            {
                CreateGroupSTSObjects();
                if (refMaterial == refMaterialRaycast)
                {
                    foreach (GameObject item in listSceneParents)
                    {
                        if (item.GetComponent<ManualTriggerByParent>() == null)
                        {
                            item.AddComponent<ManualTriggerByParent>();
                        }
                    }
                }
                

            }
            if (tab == 0 && refMaterial == refMaterialRaycast)
            {
                foreach (GameObject item in listRaycastElements)
                {
                    if (item.GetComponent<ManualTriggerByParent>() == null)
                    {
                        item.AddComponent<ManualTriggerByParent>();
                    }
                }
            }
            if (refMaterial != null && refMaterial == refMaterialRaycast && listScenePlayers.Count > 0)
            {
                if (posManager != null)
                {
                    PlayerToCameraRaycastTriggerManager stsCamRaycast = null;
                    if (posManager.gameObject.GetComponent<PlayerToCameraRaycastTriggerManager>() == null)
                    {
                        posManager.gameObject.AddComponent<PlayerToCameraRaycastTriggerManager>();
                        stsCamRaycast = posManager.gameObject.GetComponent<PlayerToCameraRaycastTriggerManager>();
                    }
                    else
                    {
                        stsCamRaycast = posManager.gameObject.GetComponent<PlayerToCameraRaycastTriggerManager>();
                    }
                    if (stsCamRaycast != null)
                    {
                        if (stsCamRaycast != null && stsCamRaycast.playerList == null)
                        {
                            stsCamRaycast.playerList = new List<GameObject>();
                        }
                        foreach (GameObject item in listScenePlayers)
                        {
                            if (!stsCamRaycast.playerList.Contains(item))
                            {
                                stsCamRaycast.playerList.Add(item);
                            }
                            
                        }
                    }
                }
                
            }
            Selection.activeObject = newGameObject;
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + newGameObject.name);
            justSavedPlayers = true;
            Thread myThread = new Thread(ThreadFunction2);
            myThread.Start();
            ShowSuccessMessage();
            if (tab == 0)
            {
                ResetSetup();
            }
            //
            Repaint();
            return true;
        }


        return false;
    }    

    private bool CreateGroupSTSObjects()
    {
        
        posManager = FindObjectOfType<PlayersPositionManager>();

        if (posManager != null)
        {
           

            foreach (GameObject item in listSceneParents)
            {
                if (item.GetComponent<GroupShaderReplacement>() == null)
                {

                    item.AddComponent<GroupShaderReplacement>();
                }
                if (item.GetComponent<GroupShaderReplacement>() != null)
                {
                    item.GetComponent<GroupShaderReplacement>().layerMaskToAdd.value = layerMasks.value;
                    item.GetComponent<GroupShaderReplacement>().referenceMaterial = refMaterial;
                }
                Undo.RegisterCreatedObjectUndo(item, "Create " + item.name);
            }

            foreach (GameObject item in listScenePlayers)
            {

                if (!posManager.playableCharacters.Contains(item))
                {

                    posManager.playableCharacters.Add(item);
                }
            }

            Selection.activeObject = listSceneParents[0];
            
            //ResetSetup();
            Repaint();
            return true;
        }

        
        
        return false;
    }     
    

    public void UpgradeMaterialsToCurrentRP()
    {


        if (UnityToSTSShaderNameMapping == null)
        {
            UnityToSTSShaderNameMapping = GeneralUtils.getUnityToSTSShaderMapping();
        }

        if (UnityToSTSShaderMapping == null)
        {
            UnityToSTSShaderMapping = new Dictionary<string, Shader>();
        }
        foreach (string key in UnityToSTSShaderNameMapping.Keys.ToList())
        {
            Shader shader = Shader.Find(UnityToSTSShaderNameMapping[key]);
            UnityToSTSShaderMapping[key] = shader ?? Shader.Find(UnityToSTSShaderNameMapping[SeeThroughShaderConstants.STS_SHADER_DEFAULT_KEY]);
        }
        if (mats != null)
        {
            foreach (Material mat in mats)
            {
                //if (mat.shader.name != unityVersionRenderPipelineShader.versionAndRPCorrectedShader)
                if (!UnityToSTSShaderMapping.Values.Contains(mat.shader))
                {
                    mat.shader = UnityToSTSShaderMapping.TryGetValue(mat.shader.name, out Shader value) ? value : UnityToSTSShaderMapping[SeeThroughShaderConstants.STS_SHADER_DEFAULT_KEY];
                }
            }
        }


    }
    
    void OpenRPConverter()
    {

        var currentRP = GraphicsSettings.currentRenderPipeline;

        if (currentRP != null && currentRP.GetType().Name.Contains("UniversalRenderPipelineAsset"))
        {
            if (unityVersionRenderPipelineShader.unityVersion.Contains("2019") || unityVersionRenderPipelineShader.unityVersion.Contains("2020"))
            {
                GetWindow<STSDialog2019MatUpgrade>("Sts 2019/2020 Dialogue");
            }
            else
            {
               // Debug.Log("Upgrading Materials to Universal Render Pipeline.");
                EditorApplication.ExecuteMenuItem("Window/Rendering/Render Pipeline Converter");
            }
        }
        else if (currentRP != null && (currentRP.GetType().Name.Contains("High") || currentRP.GetType().Name.Contains("HD")))
        {

            ///Debug.Log("Upgrading Materials to hd Render Pipeline.");

            if (unityVersionRenderPipelineShader.unityVersion.Contains("2019") || unityVersionRenderPipelineShader.unityVersion.Contains("2020"))
            {
               // Debug.Log("Upgrading Materials to Universal Render Pipeline.");
                EditorApplication.ExecuteMenuItem("Window/Render Pipeline/HD Render Pipeline Wizard");
            }
            else
            {
               // Debug.Log("Upgrading Materials to Universal Render Pipeline.");
                EditorApplication.ExecuteMenuItem("Window/Rendering/HDRP Wizard");
            }
        }



    }
    
    void ThreadFunction1()
    {

        
        Thread.Sleep(500);
        justSavedGroup = false;


       
    }

    void ThreadFunction2()
    {


       
        Thread.Sleep(2000);
        justSavedPlayers = false;



       
    }

    void focusOnGameObject(GameObject go)
    {

        Selection.activeObject = go;
    }

    IEnumerator RemoveHighlight()
    {
        yield return new WaitForSeconds(20);

    }

    void ShowSTSConcepts()
    {

    }

    void ShowSuccessMessage()
    {

        bool userClickedOK = EditorUtility.DisplayDialog(
            "Operation Completed",
            "The operation was successful!",
            "OK"

        );
        
        
    }

    void ShowSamplesImportUsage()
    {
        var window = GetWindow<SampleImporterWindow>("See-through Shader Samples Import");
        
    }

    void ResetSetup()
    {
        listScenePlayers = new List<GameObject>();
        listPrefabs = new List<GameObject>();
        listRaycastElements = new List<GameObject>();
        listSceneParents = new List<GameObject>();
        layerMasks.value = 0;
        refMaterial = refMaterialCircle;
    }
}
public static class EditorCoroutineQuick
{
    public static void Start(IEnumerator routine)
    {
        EditorApplication.CallbackFunction callback = null;
        callback = () =>
        {
            if (routine.MoveNext())
            {
                return;
            }

            EditorApplication.update -= callback;
        };

        EditorApplication.update += callback;
    }
}
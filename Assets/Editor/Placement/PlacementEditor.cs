#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[CustomEditor(typeof(PlacementEditor))]
public class PlacementEditor : EditorWindow
{

    // Lists and Arrays
    private List<Texture2D> _previews = new List<Texture2D>();
    private List<ObjectPreset> _objectPresets = new List<ObjectPreset>();

    private List<string> _axisStrings = new List<string> { "X Axis", "Y Axis", "Z Axis" };
    private List<string> _menuLabels = new List<string> { "Paint", "Object Options" };
    private ObjectPreset _currentPreset;

    //Selection indexes
    private int _menuSelection = 0;
    private int _objectSelection = 0;



    //Collapse booleans
    bool _paintOptions = true;
    bool _objectOpacity = true;


    private float _previewSize = 2;

    //Scrollbar floats
    private Vector2 gameObjectscrollPosition = Vector2.zero;
    private Vector2 opacityScrollPosition = Vector2.zero;


    //Remove this later on
    bool _selecting = false;
    private RaycastHit _raycastBrushHit;


    public string presetName;
    public mouseEvent eventType;
    [HideInInspector]
    public EventType selectedEvent;
    public float _brushOpacity = 1f;
    public float _brushSize = 15f;
    public Color _brushColor = new Color(0.028f, 0.991f, 0.934f, 0.116f);
    public bool _brushSolid;

    private GameObject groupObject;

    public enum mouseEvent
    {
        MouseDown,
        MouseHold,
    }

    [MenuItem("ProjectExposure/Placement")]
    static void Init()
    {

        PlacementEditor window = (PlacementEditor)GetWindow(typeof(PlacementEditor));
        Directory.CreateDirectory("Assets/Resources/ObjectPresets");
        window.Show();
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    private void OnEnable()
    {
        loadPresets();
    }

    void loadPresets()
    {
        Object[] presetObject = Resources.LoadAll("ObjectPresets/");
        for (int i = 0; i < presetObject.Length; i++)
        {
            if (presetObject[i] is ObjectPreset)
            {
                ObjectPreset preset = presetObject[i] as ObjectPreset;
                _objectPresets.Add(preset);
                _previews.Add(getTexture(preset.gameObject));
            }
        }
    }

    private void ImportFolder(string selectedPath, bool startup = false)
    {
        List<string> filePath = Directory.GetFiles(selectedPath, "*").Where(s => s.EndsWith(".prefab")).ToList();
        if (filePath.Count == 0)
        {
            EditorUtility.DisplayDialog("No prefabs found", "This folder doesn't seem to contain any prefab", "OK");
            return;
        }
        else
        {
            for (int i = 0; i < filePath.Count; i++)
            {
                filePath[i] = filePath[i].Replace(Application.dataPath, "Assets");
                GameObject gameObject = (GameObject)AssetDatabase.LoadAssetAtPath(filePath[i], typeof(GameObject));
                if (gameObject != null)
                    addPreset(gameObject);
            }
        }
    }

    void addPreset(GameObject gameobject)
    {
        ObjectPreset objectPreset = CreateInstance<ObjectPreset>();
        objectPreset.gameObject = gameobject;
        objectPreset.name = gameobject.name + "ObjectPreset";
        objectPreset.presetName = objectPreset.name;
        AssetDatabase.CreateAsset(objectPreset, "Assets/Resources/ObjectPresets/" + objectPreset.presetName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _objectPresets.Add(objectPreset);
        _previews.Add(getTexture(objectPreset.gameObject));
    }

    Texture2D getTexture(GameObject gameObject)
    {
        Texture2D tx = AssetPreview.GetAssetPreview(gameObject);
        if (tx == null)
            return null;
        else
        {
            Texture2D _tex = new Texture2D(tx.width, tx.height);
            _tex.SetPixels(tx.GetPixels());
            _tex.Apply();
            return _tex;
        }
    }

    List<string> getClassStrings()
    {
        List<string> presetNames = new List<string>();
        if (_objectPresets.Count > 0)
        {
            for (int i = 0; i < _objectPresets.Count; i++)
            {
                presetNames.Add(_objectPresets[i].presetName);
            }
        }
        return presetNames;
    }

    void OnGUI()
    {
        // Styling
        GUIStyle centeredTextStyle = new GUIStyle("label");
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;
        centeredTextStyle.fontStyle = FontStyle.Bold;


        if (Event.current.commandName == "ObjectSelectorClosed" && _selecting)
        {
            GameObject selectedObject = (GameObject)EditorGUIUtility.GetObjectPickerObject();

            if (selectedObject != null)
            {
                addPreset(selectedObject);
            }
            _selecting = false;
        }

        _menuSelection = GUILayout.Toolbar(_menuSelection, _menuLabels.ToArray());
        if (_menuSelection == 0)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Preview Size", centeredTextStyle);

            _previewSize = GUILayout.HorizontalSlider(_previewSize, 1f, 8f, GUILayout.MaxWidth((position.width / 2)));
            GUILayout.EndHorizontal();

            gameObjectscrollPosition = GUILayout.BeginScrollView(gameObjectscrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height / 2.5f));
            _objectSelection = GUILayout.SelectionGrid(_objectSelection, _previews.ToArray(), Mathf.RoundToInt(8 / _previewSize), GUILayout.MaxWidth((position.width - 25f)), GUILayout.MaxHeight(position.height * _previewSize));
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button("Add GameObject", EditorStyles.toolbarButton))
            {
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", EditorGUIUtility.GetControlID(FocusType.Passive));
                _selecting = true;
            }
            if (GUILayout.Button("Import from folder", EditorStyles.toolbarButton))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Load from folder", EditorApplication.applicationPath, "");
                if (selectedPath != "" && selectedPath != null)
                {
                    ImportFolder(selectedPath);
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _paintOptions = EditorGUILayout.Foldout(_paintOptions, "Paint Options");
            if (_paintOptions)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Group Object", GUILayout.Width(Screen.width / 4));
                groupObject = EditorGUILayout.ObjectField(groupObject, typeof(GameObject), true) as GameObject;
                EditorGUILayout.EndHorizontal();
                
                
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Brush size",GUILayout.Width(Screen.width / 4));
                _brushSize = EditorGUILayout.Slider(_brushSize, 0, 250);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Brush opacity", GUILayout.Width(Screen.width / 4));
                _brushOpacity = EditorGUILayout.Slider(_brushOpacity, 0, 1);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Brush color", GUILayout.Width(Screen.width / 4));
                _brushColor = EditorGUILayout.ColorField(_brushColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Event", GUILayout.Width(Screen.width / 4));
                eventType = (mouseEvent)EditorGUILayout.EnumPopup(eventType);
                if (eventType == mouseEvent.MouseDown)
                {
                    selectedEvent = EventType.MouseDown;
                }
                else
                    selectedEvent = EventType.MouseMove;
                EditorGUILayout.EndHorizontal();
            }

            if (_objectPresets.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _objectOpacity = EditorGUILayout.Foldout(_objectOpacity, "Individual Opacity");

                opacityScrollPosition = EditorGUILayout.BeginScrollView(opacityScrollPosition, EditorStyles.helpBox);
                for (int i = 0; i < _objectPresets.Count; i++)
                {
                    
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUILayout.Label(_objectPresets[i].gameObject.name, GUILayout.Width(Screen.width / 4));
                    _objectPresets[i].opacity = EditorGUILayout.Slider(_objectPresets[i].opacity, 0, 1);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        if (_menuSelection == 1)
        {
            if (_objectPresets.Count > 0)
            {

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(_objectPresets[_objectSelection].gameObject.name, centeredTextStyle);
                GUILayout.Label(AssetDatabase.GetAssetPath(_objectPresets[_objectSelection].gameObject));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Scaling Options");
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _currentPreset = _objectPresets[_objectSelection];

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                _currentPreset.uniformScaling = GUILayout.Toggle(_currentPreset.uniformScaling, "Uniform X/Y/Z  scaling");
                GUI.enabled = _currentPreset.uniformScaling;
                _currentPreset.uniformScaleMin = EditorGUILayout.FloatField(_currentPreset.uniformScaleMin);
                EditorGUILayout.MinMaxSlider(ref _currentPreset.uniformScaleMin, ref _currentPreset.uniformScaleMax, _currentPreset.uniformScaleMin, _currentPreset.uniformScaleMax);
                _currentPreset.uniformScaleMax = EditorGUILayout.FloatField(_currentPreset.uniformScaleMax);
                if (_currentPreset.uniformScaling && GUI.changed)
                {
                    _currentPreset.minScaling.x = _currentPreset.minScaling.y = _currentPreset.minScaling.z = _currentPreset.uniformScaleMin;
                    _currentPreset.maxScaling.x = _currentPreset.maxScaling.y = _currentPreset.maxScaling.z = _currentPreset.uniformScaleMax;

                }
                GUILayout.EndHorizontal();
                {
                    for (int i = 0; i < _axisStrings.Count; i++)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        GUI.enabled = !_currentPreset.uniformScaling;
                        _currentPreset.scalingBooleans[i] = GUILayout.Toggle(_currentPreset.scalingBooleans[i], "Random " + _axisStrings[i] + " Scaling");
                        GUI.enabled = _currentPreset.scalingBooleans[i];
                        _currentPreset.minScaling[i] = EditorGUILayout.FloatField(_currentPreset.minScaling[i]);
                        float minScale = _currentPreset.minScaling[i];
                        float maxScale = _currentPreset.maxScaling[i];
                        EditorGUILayout.MinMaxSlider(ref minScale, ref maxScale, minScale, maxScale);
                        _currentPreset.maxScaling[i] = EditorGUILayout.FloatField(_currentPreset.maxScaling[i]);
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
                //Reset the GUI
                GUI.enabled = true;
                ///////////////////////////////////////////////
                // Rotations
                ///////////////////////////////////////////////
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Rotation Options");
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(EditorStyles.helpBox);
                for (int i = 0; i < _axisStrings.Count; i++)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    _currentPreset.rotationBooleans[i] = GUILayout.Toggle(_currentPreset.rotationBooleans[i], "Random " + _axisStrings[i] + " Rotation");
                    GUI.enabled = _currentPreset.rotationBooleans[i];
                    float minRotation = _currentPreset.minRotations[i];
                    float maxRotation = _currentPreset.maxRotations[i];
                    _currentPreset.minRotations[i] = EditorGUILayout.FloatField(minRotation);
                    EditorGUILayout.MinMaxSlider(ref minRotation, ref maxRotation, minRotation, maxRotation);
                    _currentPreset.maxRotations[i] = EditorGUILayout.FloatField(maxRotation);
                    GUILayout.EndHorizontal();
                    //Reset the GUI
                    GUI.enabled = true;
                }
                EditorGUILayout.EndVertical();
                //Reset the GUI
                GUI.enabled = true;
                ///////////////////////////////////////////////
                // Extra options
                ///////////////////////////////////////////////
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("Extra Options");
                GUILayout.EndHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                _currentPreset.tag = EditorGUILayout.TagField("Spawn tag", _currentPreset.tag);
                GUILayout.Label("Static");
                _currentPreset.isStatic = EditorGUILayout.Toggle(_currentPreset.isStatic);
                GUILayout.Label("Layer");
                _currentPreset.layer = EditorGUILayout.LayerField(_currentPreset.layer);
                EditorGUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        Event e = Event.current;
        if (_objectPresets.Count > 0)
        {
            Ray _raycastBrush = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(_raycastBrush, out _raycastBrushHit))
            {
                Handles.color = _brushColor;
                Handles.DrawSolidDisc(_raycastBrushHit.point, _raycastBrushHit.normal, _brushSize / 10);
            }
        }
        if (_objectPresets.Count > 0 && e.shift && e.button == 0 && Event.current.type == selectedEvent && groupObject != null)
        {
                Ray _raycast = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit _raycastHit;
                if (Physics.Raycast(_raycast, out _raycastHit))
                {
                    for (int i = 0; i < _objectPresets.Count; i++)
                    {
                        ObjectPreset objectToSpawn = _objectPresets[i];
                        if (Random.Range(0f, 1f) < objectToSpawn.opacity)
                        {
                            Vector3 positionOffset = Random.insideUnitSphere * _brushSize / 10;
                            positionOffset.y = 0f;
                            GameObject instantiatedObject = Instantiate(objectToSpawn.gameObject, _raycastHit.point + positionOffset, Quaternion.identity);
                            var newEulerAngels = instantiatedObject.transform.eulerAngles;
                            newEulerAngels.x = Random.Range(objectToSpawn.minRotations.x, objectToSpawn.maxRotations.x);
                            newEulerAngels.y = Random.Range(objectToSpawn.minRotations.y, objectToSpawn.maxRotations.y);
                            newEulerAngels.z = Random.Range(objectToSpawn.minRotations.z, objectToSpawn.maxRotations.z);
                            instantiatedObject.transform.eulerAngles = newEulerAngels;
                            instantiatedObject.transform.localScale = new Vector3(Random.Range(objectToSpawn.minScaling.x, objectToSpawn.maxScaling.x), Random.Range(objectToSpawn.minScaling.y, objectToSpawn.maxScaling.y), Random.Range(objectToSpawn.minScaling.z, objectToSpawn.maxScaling.z));
                            instantiatedObject.transform.tag = objectToSpawn.tag;
                            instantiatedObject.transform.parent = groupObject.transform;
                            instantiatedObject.isStatic = objectToSpawn.isStatic;
                            instantiatedObject.layer = objectToSpawn.layer;
                            instantiatedObject.transform.name = objectToSpawn.gameObject.name;
                           
                        }
                    }
            }
            Handles.EndGUI();
        }
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Forester
{
    public class ForesterPresetEditor : EditorWindow
    {
        string assetPath;
        [SerializeField]
        private ForestPreset _ForestPreset = null;
        IconPicker IconPicker;

        int assetId;
        string _PresetName;

        int _SelectionIndex = -1;
        GUIStyle[] _GroupSelectStyle;

        Vector2 scrollPosition1 = Vector2.zero;
        Vector2 scrollPosition2 = Vector2.zero;
        Vector2 scrollPosition3 = Vector2.zero;
        Vector2 scrollPosition4 = Vector2.zero;

        Texture2D _NormalButton;
        Texture2D _ActiveButton;

        private bool _FilterSelectionMode = false;

        Color _ButtonFontColour;

        //Temp Holder
        [SerializeField]
        public List<ForestGroup> _ForestGroup = new List<ForestGroup>();
        public List<bool> _ForestGroupSelection = new List<bool>();

        bool _PresetLoaded = false; 

        [MenuItem("Tools/Forester/Tools/Preset Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ForesterPresetEditor window = GetWindow<ForesterPresetEditor>("Forester Preset Editor");
            if (window.assetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        window.assetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (window.assetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                window.assetPath = dir;
                            }
                        }
                    }
                }
            }
            GUIContent titleContent = new GUIContent("Preset Editor", AssetDatabase.LoadAssetAtPath<Texture2D>(window.assetPath + "/InternalResources/Sprites/Fr_WindowIcon.png"));
            window.titleContent = titleContent;
            window.Show();
            window.minSize = new Vector2(400, 590);
            window.maxSize = new Vector2(400, 590);
        }

        void OnDestroy()
        {
            //GarbageCleanup();
        }

        void OnEnable()
        {
            _NormalButton = null;
            _ActiveButton = null;
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
        }

        void OnScene(SceneView sceneview)
        {
            
        }

        public void StartEditor(ForestPreset forestPreset)
        {
            _ForestPreset = forestPreset;

            for (int i = 0; i < _ForestPreset._ForestGroup.Count; i++)
            {
                if(_ForestPreset._ForestGroup[i]._UniqueID == 0)
                {
                    _ForestPreset._ForestGroup[i]._UniqueID = (i+1);
                }
            }
            Init();
        }

        void LoadAsset()
        {
            bool saveAsset = false;

            _ForestGroup.Clear();
            for (int i = 0; i < _ForestPreset._ForestGroup.Count; i++)
            {
                _ForestGroup.Add(new ForestGroup());

                if (_ForestPreset._ForestGroup[i]._FoliageObjects.Count > 0)
                {
                    for (int n = 0; n < _ForestPreset._ForestGroup[i]._FoliageObjects.Count; n++)
                    {
                        _ForestGroup[i]._FoliageObjects.Add(new FoliageObjects());
                        _ForestGroup[i]._FoliageObjects[n]._FoliageObject = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._FoliageObject;
                        _ForestGroup[i]._FoliageObjects[n]._ParticleSystem = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._ParticleSystem;
                        _ForestGroup[i]._FoliageObjects[n]._ParticleSystemHeight = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._ParticleSystemHeight;
                        _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects.Clear();
                        for (int s = 0; s < _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects.Count; s++)
                        {
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects.Add(new SatelliteObjects());
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._FaceNormals = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._FaceNormals;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Hover = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Hover;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxNum = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxNum;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Object = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Object;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Rotation = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Rotation;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._YPos = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._YPos;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._SatelliteSpread = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._SatelliteSpread;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Scale = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Scale;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MinScale = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MinScale;
                            _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxScale = _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxScale;
                        }
                    }
                }


                //----------------------To be removed in V1.4---------------------
                if (_ForestPreset._ForestGroup[i]._OffsetOverDistance.Count > 0)
                {
                    _ForestGroup[i]._OffsetOverDist = new AnimationCurve();
                    foreach (OffsetOverDistance ood in _ForestPreset._ForestGroup[i]._OffsetOverDistance)
                    {
                        _ForestGroup[i]._OffsetOverDist.AddKey(ood.startDist, ood.OffsetOverDist);
                        _ForestGroup[i]._OffsetOverDist.AddKey(ood.endDist, ood.OffsetOverDist);

                        //Set as flat for default
                        for (int n = 0; n < _ForestGroup[i]._OffsetOverDist.keys.Length; n++)
                        {
                            AnimationUtility.SetKeyLeftTangentMode(_ForestGroup[i]._OffsetOverDist, n, AnimationUtility.TangentMode.Linear);
                            AnimationUtility.SetKeyRightTangentMode(_ForestGroup[i]._OffsetOverDist, n, AnimationUtility.TangentMode.Linear);
                        }
                    }
                    _ForestPreset._ForestGroup[i]._OffsetOverDistance.Clear();
                    saveAsset = true;
                }
                else
                {
                    //--------------------------Replaced by-----------------------
                    if (_ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length > 0)
                    {
                        _ForestGroup[i]._OffsetOverDist = new AnimationCurve();
                        for (int n = 0; n < _ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length; n++)
                        {
                            _ForestGroup[i]._OffsetOverDist.AddKey(_ForestPreset._ForestGroup[i]._OffsetOverDist.keys[n].time, _ForestPreset._ForestGroup[i]._OffsetOverDist.keys[n].value);
                        }
                        for (int f = 0; f < _ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length; f++)
                        {
                            AnimationUtility.SetKeyLeftTangentMode(_ForestGroup[i]._OffsetOverDist, f, AnimationUtility.GetKeyLeftTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, f));
                            AnimationUtility.SetKeyRightTangentMode(_ForestGroup[i]._OffsetOverDist, f, AnimationUtility.GetKeyRightTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, f));
                        }
                    }
                    //------------------------------------------------------------
                }

                if (_ForestPreset._ForestGroup[i]._TargetTextures.Count > 0)
                {
                    for (int n = 0; n < _ForestPreset._ForestGroup[i]._TargetTextures.Count; n++)
                    {
                        _ForestGroup[i]._TargetTextures.Add(_ForestPreset._ForestGroup[i]._TargetTextures[n]);
                    }
                }

                _ForestGroup[i]._GroupName = _ForestPreset._ForestGroup[i]._GroupName;
                _ForestGroup[i]._Icon = _ForestPreset._ForestGroup[i]._Icon;
                _ForestGroup[i]._MaxOffset = _ForestPreset._ForestGroup[i]._MaxOffset;
                _ForestGroup[i]._MinOffset = _ForestPreset._ForestGroup[i]._MinOffset;
                _ForestGroup[i]._Offset = _ForestPreset._ForestGroup[i]._Offset;
                _ForestGroup[i]._RandomRotation = _ForestPreset._ForestGroup[i]._RandomRotation;
                _ForestGroup[i]._RandomScale = _ForestPreset._ForestGroup[i]._RandomScale;
                _ForestGroup[i]._MinRotation = _ForestPreset._ForestGroup[i]._MinRotation;
                _ForestGroup[i]._MaxRotation = _ForestPreset._ForestGroup[i]._MaxRotation;
                _ForestGroup[i]._MinScale = _ForestPreset._ForestGroup[i]._MinScale;
                _ForestGroup[i]._MaxScale = _ForestPreset._ForestGroup[i]._MaxScale;
                _ForestGroup[i]._HasParticles = _ForestPreset._ForestGroup[i]._HasParticles;
                _ForestGroup[i]._FaceNormals = _ForestPreset._ForestGroup[i]._FaceNormals;
                _ForestGroup[i]._Overlap = _ForestPreset._ForestGroup[i]._Overlap;
                _ForestGroup[i]._Fill = _ForestPreset._ForestGroup[i]._Fill;
                _ForestGroup[i]._MinBorderOffset = _ForestPreset._ForestGroup[i]._MinBorderOffset;
                _ForestGroup[i]._MaxBorderOffset = _ForestPreset._ForestGroup[i]._MaxBorderOffset;
                _ForestGroup[i]._BorderOffset = _ForestPreset._ForestGroup[i]._BorderOffset;
                _ForestGroup[i]._OverlapDistance = _ForestPreset._ForestGroup[i]._OverlapDistance;
                _ForestGroup[i]._MinOverlapDistance = _ForestPreset._ForestGroup[i]._MinOverlapDistance;
                _ForestGroup[i]._MaxOverlapDistance = _ForestPreset._ForestGroup[i]._MaxOverlapDistance;
                _ForestGroup[i]._AngleLimit = _ForestPreset._ForestGroup[i]._AngleLimit;
                _ForestGroup[i]._MaxAngleLimit = _ForestPreset._ForestGroup[i]._MaxAngleLimit;
                _ForestGroup[i]._MinAngleLimit = _ForestPreset._ForestGroup[i]._MinAngleLimit;
                _ForestGroup[i]._DigDepth = _ForestPreset._ForestGroup[i]._DigDepth;
                _ForestGroup[i]._MinDigDepth = _ForestPreset._ForestGroup[i]._MinDigDepth;
                _ForestGroup[i]._MaxDigDepth = _ForestPreset._ForestGroup[i]._MaxDigDepth;
                _ForestGroup[i]._FaceBorderDirection = _ForestPreset._ForestGroup[i]._FaceBorderDirection;
                _ForestGroup[i]._UniqueID = _ForestPreset._ForestGroup[i]._UniqueID;
                _ForestGroup[i]._TerrainStamping._AllowStamping = _ForestPreset._ForestGroup[i]._TerrainStamping._AllowStamping;
                _ForestGroup[i]._TerrainStamping._AllowStampOverlap = _ForestPreset._ForestGroup[i]._TerrainStamping._AllowStampOverlap;
                _ForestGroup[i]._TerrainStamping._StampOpacity = _ForestPreset._ForestGroup[i]._TerrainStamping._StampOpacity;
                _ForestGroup[i]._TerrainStamping._StampSize = _ForestPreset._ForestGroup[i]._TerrainStamping._StampSize;
                _ForestGroup[i]._TerrainStamping._StampTex = _ForestPreset._ForestGroup[i]._TerrainStamping._StampTex;
                _ForestGroup[i]._TerrainStamping._TerrainTex = _ForestPreset._ForestGroup[i]._TerrainStamping._TerrainTex;
                _ForestGroup[i]._TerrainStamping._TileSize = _ForestPreset._ForestGroup[i]._TerrainStamping._TileSize;
                _ForestGroup[i]._TerrainStamping._AllowHeight = _ForestPreset._ForestGroup[i]._TerrainStamping._AllowHeight;
                _ForestGroup[i]._TerrainStamping._StampSizeHeight = _ForestPreset._ForestGroup[i]._TerrainStamping._StampSizeHeight;
                _ForestGroup[i]._TerrainStamping._MinHeightStrength = _ForestPreset._ForestGroup[i]._TerrainStamping._MinHeightStrength;
                _ForestGroup[i]._TerrainStamping._MaxHeightStrength = _ForestPreset._ForestGroup[i]._TerrainStamping._MaxHeightStrength;
                _ForestGroup[i]._TerrainStamping._StampTexHeight = _ForestPreset._ForestGroup[i]._TerrainStamping._StampTexHeight;
            }
            if (saveAsset) SaveAsset();
            Repaint();
        }

        void OnSelectionChange()
        {
            if (_FilterSelectionMode)
            {
                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponent<Terrain>() != null)
                    {

                        Terrain terrain = Selection.activeGameObject.GetComponent<Terrain>();
                        TerrainData terrainData = terrain.terrainData;
                        foreach (TerrainLayer splatTx in terrainData.terrainLayers)
                        {
                            bool exists = false;
                            foreach (Texture2D tx in _ForestGroup[_SelectionIndex]._TargetTextures)
                            {
                                if (splatTx.diffuseTexture == tx)
                                {
                                    exists = true;
                                }
                            }
                            if (!exists) _ForestGroup[_SelectionIndex]._TargetTextures.Add(splatTx.diffuseTexture);
                        }
                        Selection.objects = new Object[0];
                        Focus();
                        _FilterSelectionMode = false;
                    }
                }
            }
        }

        public void OnGUI()
        {
            _ButtonFontColour = Color.black;

            if (assetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        assetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (assetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                assetPath = dir;
                            }
                        }
                    }
                }
            }
            if (_ForestPreset == null) _PresetLoaded = false;

            if (_ForestPreset != null)
            {
                if (_PresetLoaded == false)
                {
                    _PresetName = _ForestPreset.name;
                    _PresetLoaded = true;
                }

                int checkId = _ForestPreset.GetHashCode();
                if (checkId != assetId)
                {
                    assetId = checkId;
                    LoadAsset();
                    _SelectionIndex = -1;
                    _ForestGroupSelection.Clear();
                    for (int i = 0; i < _ForestPreset._ForestGroup.Count; i++)
                    {
                        _ForestGroupSelection.Add(false);
                    }
                }
            }

            if (assetPath == null)
            {
                return;
            }
            //_ButtonFontColour = Color.black;
            _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
            _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");
            Texture satellite = AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Fr_Satellite_Black.png");

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
                satellite = AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Fr_Satellite_White.png");
            }
      

            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Forester_ForestPresetBanner.png"), GUILayout.Height(50));
            var NormalStyle = new GUIStyle(GUI.skin.button);
            NormalStyle.normal.background = _NormalButton;

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            var textFloatingLayout = new GUIStyle();
            textFloatingLayout.alignment = TextAnchor.MiddleCenter;

            var floatLayout = new GUIStyle(GUI.skin.textArea);
            floatLayout.alignment = TextAnchor.MiddleCenter;
            floatLayout.stretchWidth = true;
            floatLayout.fixedWidth = 40;

            var popupStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
            popupStyle.alignment = TextAnchor.MiddleCenter;

            var bannerLayout = new GUIStyle(GUI.skin.button);
            bannerLayout.alignment = TextAnchor.MiddleCenter;

            GUIStyle deleteStyle = new GUIStyle(GUI.skin.button);
            deleteStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Red.png");

            EditorGUILayout.LabelField("Properties", textLayout);

            GUILayout.Space(10);
            SerializedObject serialized = new SerializedObject(this);
            SerializedProperty forestPrefab = serialized.FindProperty("_ForestPreset");
            serialized.Update();
            EditorGUIUtility.labelWidth = 110;
            EditorGUILayout.ObjectField(forestPrefab, typeof(Object));
            GUILayout.BeginHorizontal();
            if (_ForestPreset == null && _PresetName == null || _ForestPreset == null && _PresetName == "") _PresetName = "<Insert Preset Name>";
            GUILayout.Label("Preset Name");
            EditorGUIUtility.labelWidth = 200;
            _PresetName = EditorGUILayout.TextField(_PresetName, textLayout);
            GUILayout.EndHorizontal();
            serialized.ApplyModifiedProperties();

            if (_ForestPreset == null)
            {
                if (GUILayout.Button("Create New Preset"))
                {
                    ForestPreset newPreset = CreateInstance<ForestPreset>();
                    if (!System.IO.Directory.Exists(assetPath + "/Presets/ForestPresets"))
                    {
                        System.IO.Directory.CreateDirectory(assetPath + "/Presets/ForestPresets");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    string name = _PresetName;
                    if (name == "<Insert Preset Name>")
                    {
                        name = "NewForestPreset";
                    }
                    AssetDatabase.CreateAsset(newPreset, assetPath + "/Presets/ForestPresets/" + name + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    _ForestPreset = AssetDatabase.LoadAssetAtPath<ForestPreset>(assetPath + "/Presets/ForestPresets/" + name + ".asset");
                    _SelectionIndex = -1;
                    LoadAsset();
                }
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUILayout.Space(454);
                return;
            }

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, true, GUILayout.Width(400), GUILayout.Height(425));

            if (EditorGUILayout.BeginFadeGroup(1))
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Foliage Groups", textLayout);
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUILayout.BeginVertical(GUI.skin.window, GUILayout.Width(360));

                _GroupSelectStyle = new GUIStyle[_ForestGroup.Count];
                for (int n = 0; n < _GroupSelectStyle.Length; n++)
                {
                    _GroupSelectStyle[n] = new GUIStyle(GUI.skin.button);
                    if (n == _SelectionIndex)
                    {
                        _GroupSelectStyle[n].normal.background = _ActiveButton;
                    }
                    else
                    {
                        _GroupSelectStyle[n].normal.background = _NormalButton;
                    }
                }

                scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.Width(350), GUILayout.Height(120));
                if (_ForestGroup.Count > 0)
                {
                    for (int i = 0; i < _ForestGroup.Count; i++)
                    {
                        //Panel
                        GUILayout.BeginVertical(GUI.skin.button, GUILayout.Width(0));
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(">", _GroupSelectStyle[i], GUILayout.Width(22), GUILayout.Height(45)))
                        {
                            _ForestGroupSelection[i] = true;
                            _SelectionIndex = i;
                        }


                        if (GUILayout.Button(_ForestGroup[i]._Icon, GUILayout.Width(50), GUILayout.Height(45)))
                        {
                            IconPicker = CreateInstance<IconPicker>();
                            IconPicker.Init(i, _ForestGroup);
                        }
                        _ForestGroup[i]._GroupName = EditorGUILayout.TextField(_ForestGroup[i]._GroupName, textLayout, GUILayout.Width(180), GUILayout.Height(47));
                        GUILayout.Label(_ForestGroup[i]._FoliageObjects.Count.ToString(), textFloatingLayout, GUILayout.Width(50), GUILayout.Height(50));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        //End
                    }
                }
                if (GUILayout.Button("Create New", GUILayout.Height(50)))
                {
                    _ForestGroup.Add(new ForestGroup());
                    _ForestGroup[_ForestGroup.Count - 1]._UniqueID = GenerateUniqueID();
                    _ForestGroupSelection.Add(false);
                    _GroupSelectStyle = new GUIStyle[_ForestGroup.Count];
                    for (int n = 0; n < _GroupSelectStyle.Length; n++)
                    {
                        _GroupSelectStyle[n] = new GUIStyle(GUI.skin.button);
                        _GroupSelectStyle[n].normal.background = _NormalButton;
                    }
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Duplicate Group", GUILayout.Width(120)))
                {
                    if (_SelectionIndex >= 0)
                    {
                        _ForestGroup.Add(new ForestGroup());
                        _ForestGroup[_ForestGroup.Count - 1]._UniqueID = GenerateUniqueID();
                        DeepCopyGroup(_SelectionIndex, _ForestGroup.Count - 1);
                        _ForestGroupSelection.Add(false);
                        _GroupSelectStyle = new GUIStyle[_ForestGroup.Count];
                        for (int n = 0; n < _GroupSelectStyle.Length; n++)
                        {
                            _GroupSelectStyle[n] = new GUIStyle(GUI.skin.button);
                            _GroupSelectStyle[n].normal.background = _NormalButton;
                        }
                    }
                }
                GUILayout.Space(2);
                if (GUILayout.Button("Delete Group", GUILayout.Width(120)))
                {
                    if (_SelectionIndex >= 0)
                    {
                        _ForestGroup.RemoveAt(_SelectionIndex);
                        _ForestGroupSelection.RemoveAt(_SelectionIndex);
                        _SelectionIndex = -1;
                    }
                }

                GUILayout.Space(2);
                if (GUILayout.Button("Clear All", GUILayout.Width(120)))
                {
                    _ForestGroup.Clear();
                    _ForestGroupSelection.Clear();
                    _SelectionIndex = -1;
                }

                GUILayout.Space(4);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Foliage Objects", textLayout);
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUILayout.BeginVertical(GUI.skin.window, GUILayout.Width(360));
                scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, true, GUILayout.Width(350), GUILayout.Height(120));
                if (_SelectionIndex >= 0)
                {
                    if (_ForestGroup[_SelectionIndex]._FoliageObjects.Count > 0)
                    {
                        GUIStyle[] satelliteStyle = new GUIStyle[_ForestGroup[_SelectionIndex]._FoliageObjects.Count];
                        for (int i = 0; i < _ForestGroup[_SelectionIndex]._FoliageObjects.Count; i++)
                        {
                            //Panel
                            GUILayout.BeginVertical(GUI.skin.button, GUILayout.Width(0));
                            GUILayout.BeginHorizontal();
                            Texture preview = AssetPreview.GetAssetPreview(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject);
                            if (preview == null)
                            {
                                //If null force load of asset (note to self this can result in slow down)
                                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject), ImportAssetOptions.ForceUpdate);
                                preview = AssetPreview.GetAssetPreview(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject);
                            }
                            if (GUILayout.Button(preview, GUILayout.Width(50), GUILayout.Height(50)))
                            {
                                EditorGUIUtility.PingObject(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject);
                            }

                            GUILayout.BeginVertical();
                            if (_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject != null)
                            {
                                GUILayout.Label(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject.name, textLayout, GUILayout.Width(208), GUILayout.Height(32));
                                EditorGUIUtility.labelWidth = 50;
                                //---------------------------------------
                                GUILayout.BeginHorizontal();
                                _ForestGroup[_SelectionIndex]._FoliageObjects[i]._ParticleSystem = EditorGUILayout.ObjectField(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._ParticleSystem, typeof(ParticleSystem), false, GUILayout.Width(75)) as ParticleSystem;
                                GUILayout.Label("Particle Y Pos:", GUILayout.Width(85));
                                _ForestGroup[_SelectionIndex]._FoliageObjects[i]._ParticleSystemHeight = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._FoliageObjects[i]._ParticleSystemHeight, floatLayout, GUILayout.Width(30));
                                GUILayout.EndHorizontal();
                                //---------------------------------------
                                GUILayout.EndVertical();
                                satelliteStyle[i] = new GUIStyle(GUI.skin.button);
                                if (_ForestGroup[_SelectionIndex]._FoliageObjects[i]._SatelliteObjects.Count > 0)
                                {
                                    satelliteStyle[i].normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Blue.png");
                                }
                                else
                                {
                                    satelliteStyle[i].normal.background = _NormalButton;
                                }

                                if (GUILayout.Button(satellite, satelliteStyle[i], GUILayout.Width(22), GUILayout.Height(49)))
                                {
                                    SatelliteObjectEditor _SatelliteObjectEditor = CreateInstance<SatelliteObjectEditor>();
                                    _SatelliteObjectEditor.StartEditor(this, _SelectionIndex, i);
                                }
                                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/Fr_Bin.png"), deleteStyle, GUILayout.Width(22), GUILayout.Height(49)))
                                {
                                    _ForestGroup[_SelectionIndex]._FoliageObjects.RemoveAt(i);
                                }
                            }
                            else
                            {
                                _ForestGroup[_SelectionIndex]._FoliageObjects.RemoveAt(i);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            //End
                        }
                    }
                }
                if (Selection.activeGameObject != null && _SelectionIndex >= 0)
                {
                    if (GUILayout.Button("Add New", GUILayout.Height(50)))
                    {
                        for (int i = 0; i < _ForestGroup[_SelectionIndex]._FoliageObjects.Count; i++)
                        {
                            if (_ForestGroup[_SelectionIndex]._FoliageObjects[i]._FoliageObject.name == Selection.activeGameObject.name)
                            {
                                return;
                            }
                        }
                        _ForestGroup[_SelectionIndex]._FoliageObjects.Add(new FoliageObjects());
                        _ForestGroup[_SelectionIndex]._FoliageObjects[_ForestGroup[_SelectionIndex]._FoliageObjects.Count - 1]._FoliageObject = Selection.activeGameObject;
                    }
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Offset Options", textLayout);
                if (_SelectionIndex >= 0)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Density - ", GUILayout.MaxWidth(90));
                    _ForestGroup[_SelectionIndex]._MinOffset = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinOffset, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._Offset = GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._Offset, _ForestGroup[_SelectionIndex]._MinOffset, _ForestGroup[_SelectionIndex]._MaxOffset, GUILayout.Width(65));
                    _ForestGroup[_SelectionIndex]._MaxOffset = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxOffset, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                    EditorGUILayout.LabelField(_ForestGroup[_SelectionIndex]._Offset.ToString("0.00"), floatLayout);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Fill - ", GUILayout.MaxWidth(90));
                    GUILayout.Label("0%", floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._Fill = GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._Fill, 0, 100, GUILayout.Width(65));
                    GUILayout.Label("100%", floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                    _ForestGroup[_SelectionIndex]._Fill = EditorGUILayout.FloatField(Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._Fill), floatLayout, GUILayout.Width(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Border Offset - ", GUILayout.MaxWidth(90));
                    _ForestGroup[_SelectionIndex]._MinBorderOffset = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinBorderOffset, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._BorderOffset = GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._BorderOffset, _ForestGroup[_SelectionIndex]._MinBorderOffset, _ForestGroup[_SelectionIndex]._MaxBorderOffset, GUILayout.Width(65));
                    _ForestGroup[_SelectionIndex]._MaxBorderOffset = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxBorderOffset, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                    EditorGUILayout.LabelField(_ForestGroup[_SelectionIndex]._BorderOffset.ToString("0.00"), floatLayout);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Dig Depth - ", GUILayout.MaxWidth(90));
                    _ForestGroup[_SelectionIndex]._MinDigDepth = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinDigDepth, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._DigDepth = GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._DigDepth, _ForestGroup[_SelectionIndex]._MinDigDepth, _ForestGroup[_SelectionIndex]._MaxDigDepth, GUILayout.Width(65));
                    _ForestGroup[_SelectionIndex]._MaxDigDepth = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxDigDepth, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                    EditorGUILayout.LabelField(_ForestGroup[_SelectionIndex]._DigDepth.ToString("0.00"), floatLayout);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Angle Limit - ", GUILayout.MaxWidth(90));
                    _ForestGroup[_SelectionIndex]._MinAngleLimit = EditorGUILayout.FloatField(Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._MinAngleLimit), floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._AngleLimit = GUILayout.HorizontalSlider(Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._AngleLimit), Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._MinAngleLimit), Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._MaxAngleLimit), GUILayout.Width(65));
                    _ForestGroup[_SelectionIndex]._MaxAngleLimit = EditorGUILayout.FloatField(Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._MaxAngleLimit), floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                    EditorGUILayout.LabelField(Mathf.RoundToInt(_ForestGroup[_SelectionIndex]._AngleLimit).ToString("0"), floatLayout);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    string borderDirText = _ForestGroup[_SelectionIndex]._RandomRotation ? "Face Border Direction (Overriden by Random Rotation)" : "Face Border Direction";
                    _ForestGroup[_SelectionIndex]._FaceBorderDirection = GUILayout.Toggle(_ForestGroup[_SelectionIndex]._FaceBorderDirection, borderDirText);
                    GUILayout.EndHorizontal();
                }
                    GUILayout.Space(10);

                //Begin offset group
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Offset Over Distance", textLayout);
                GUILayout.Space(5);
                if (_SelectionIndex >= 0)
                {
                    EditorGUILayout.CurveField(_ForestGroup[_SelectionIndex]._OffsetOverDist);
                    GUILayout.BeginHorizontal();
                    if (_ForestGroup[_SelectionIndex]._OffsetOverDist.keys.Length == 0)
                    {
                        ResetCurve();
                    }
                    if (GUILayout.Button("Reset Curve"))
                    {
                        ResetCurve();
                    }
                    GUILayout.EndHorizontal();
                }
                    GUILayout.Space(5);

                if (_SelectionIndex >= 0)
                {
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Randomisation", textLayout);
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUIStyle rotation = new GUIStyle(GUI.skin.button);
                    if (_ForestGroup[_SelectionIndex]._RandomRotation) rotation.normal.background = _ActiveButton;
                    if (!_ForestGroup[_SelectionIndex]._RandomRotation) rotation.normal.background = _NormalButton;
                    GUILayout.Space(20);
                    if (GUILayout.Button("Random Rotation", rotation, GUILayout.Width(180)))
                    {
                        if (_ForestGroup[_SelectionIndex]._RandomRotation)
                        {
                            _ForestGroup[_SelectionIndex]._RandomRotation = false;
                        }
                        else
                        {
                            _ForestGroup[_SelectionIndex]._RandomRotation = true;
                        }
                    }
                    GUILayout.Space(10);
                    GUILayout.Label("Min", textFloatingLayout);
                    _ForestGroup[_SelectionIndex]._MinRotation = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinRotation, floatLayout);
                    GUILayout.Space(-10);
                    GUILayout.Label("Max", textFloatingLayout);
                    _ForestGroup[_SelectionIndex]._MaxRotation = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxRotation, floatLayout);

                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUIStyle scale = new GUIStyle(GUI.skin.button);
                    if (_ForestGroup[_SelectionIndex]._RandomScale) scale.normal.background = _ActiveButton;
                    if (!_ForestGroup[_SelectionIndex]._RandomScale) scale.normal.background = _NormalButton;
                    GUILayout.Space(20);
                    if (GUILayout.Button("Random Scale", scale, GUILayout.Width(180)))
                    {
                        if (_ForestGroup[_SelectionIndex]._RandomScale)
                        {
                            _ForestGroup[_SelectionIndex]._RandomScale = false;
                        }
                        else
                        {
                            _ForestGroup[_SelectionIndex]._RandomScale = true;
                        }
                    }
                    GUILayout.Space(10);
                    GUILayout.Label("Min", textFloatingLayout);
                    _ForestGroup[_SelectionIndex]._MinScale = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinScale, floatLayout);
                    GUILayout.Space(-10);
                    GUILayout.Label("Max", textFloatingLayout);
                    _ForestGroup[_SelectionIndex]._MaxScale = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxScale, floatLayout);

                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Group Options", textLayout);
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUIStyle faceNormalsStyle = new GUIStyle(GUI.skin.button);
                    if (_ForestGroup[_SelectionIndex]._FaceNormals) faceNormalsStyle.normal.background = _ActiveButton;
                    if (!_ForestGroup[_SelectionIndex]._FaceNormals) faceNormalsStyle.normal.background = _NormalButton;
                    GUILayout.Space(10);
                    if (GUILayout.Button("Face Normals", faceNormalsStyle))
                    {
                        if (_ForestGroup[_SelectionIndex]._FaceNormals)
                        {
                            _ForestGroup[_SelectionIndex]._FaceNormals = false;
                        }
                        else
                        {
                            _ForestGroup[_SelectionIndex]._FaceNormals = true;
                        }
                    }
                    GUILayout.Space(10);

                    GUIStyle particlesStyle = new GUIStyle(GUI.skin.button);
                    if (_ForestGroup[_SelectionIndex]._HasParticles) particlesStyle.normal.background = _ActiveButton;
                    if (!_ForestGroup[_SelectionIndex]._HasParticles) particlesStyle.normal.background = _NormalButton;

                    if (GUILayout.Button("Add Particles", particlesStyle))
                    {
                        if (_ForestGroup[_SelectionIndex]._HasParticles)
                        {
                            _ForestGroup[_SelectionIndex]._HasParticles = false;
                        }
                        else
                        {
                            _ForestGroup[_SelectionIndex]._HasParticles = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Overlap - ", GUILayout.MaxWidth(60));
                    _ForestGroup[_SelectionIndex]._Overlap = GUILayout.Toggle(_ForestGroup[_SelectionIndex]._Overlap, "", GUILayout.MaxWidth(80));
                    if (!_ForestGroup[_SelectionIndex]._Overlap)
                    {
                        _ForestGroup[_SelectionIndex]._MinOverlapDistance = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MinOverlapDistance, floatLayout, GUILayout.Width(40));
                        GUILayout.Space(5);
                        _ForestGroup[_SelectionIndex]._OverlapDistance = GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._OverlapDistance, _ForestGroup[_SelectionIndex]._MinOverlapDistance, _ForestGroup[_SelectionIndex]._MaxOverlapDistance, GUILayout.Width(85));
                        _ForestGroup[_SelectionIndex]._MaxOverlapDistance = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._MaxOverlapDistance, floatLayout, GUILayout.Width(40));
                        GUILayout.Space(20);
                        EditorGUIUtility.labelWidth = 40;
                        EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
                        EditorGUILayout.LabelField(_ForestGroup[_SelectionIndex]._OverlapDistance.ToString("0.00"), floatLayout);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Texture Filtering", textLayout);
                GUILayout.Space(5);

                if (_SelectionIndex >= 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(115),GUILayout.Width(375));
                    scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(115));
                    GUILayout.BeginHorizontal(GUILayout.Width(0));
                    for (int i = 0; i < _ForestGroup[_SelectionIndex]._TargetTextures.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(40), GUILayout.Height(95));
                        //Rect labelRect = new Rect(rect.x + 5f, rect.y + 5, 85, 85);
                        GUILayout.Space(5);
                        Texture2D img = _ForestGroup[_SelectionIndex]._TargetTextures[i];
                        _ForestGroup[_SelectionIndex]._TargetTextures[i] = EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;
                        if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/Fr_Bin.png"), deleteStyle, GUILayout.Height(20)))
                        {
                            _ForestGroup[_SelectionIndex]._TargetTextures.RemoveAt(i);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    if (GUILayout.Button("+", GUILayout.Width(65), GUILayout.Height(70)))
                    {
                        _ForestGroup[_SelectionIndex]._TargetTextures.Add(null);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();

                    var filterSelectionStyle = new GUIStyle(GUI.skin.button);

                    if (_FilterSelectionMode)
                    {
                        filterSelectionStyle.normal.background = _ActiveButton;
                        filterSelectionStyle.normal.textColor = _ButtonFontColour;
                    }
                    else
                    {
                        filterSelectionStyle.normal.background = _NormalButton;
                    }

                    if (GUILayout.Button("Copy from selected terrain", filterSelectionStyle))
                    {
                        _FilterSelectionMode = _FilterSelectionMode ? false : true;
                    }

                    if (GUILayout.Button("Copy to all groups"))
                    {
                        Texture2D[] targetTxs = _ForestGroup[_SelectionIndex]._TargetTextures.ToArray();
                        foreach (ForestGroup fg in _ForestGroup)
                        {
                            if (fg._GroupName != _ForestGroup[_SelectionIndex]._GroupName)
                            {
                                fg._TargetTextures.Clear();
                                foreach (Texture2D tx in targetTxs)
                                {
                                    fg._TargetTextures.Add(tx);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                    GUILayout.Space(5);
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Terrain Stamping (Experimental)", textLayout);
                GUILayout.Space(5);

                if (_SelectionIndex >= 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("Terrain Tx", GUILayout.MaxWidth(70));
                    GUILayout.Space(10);
                    GUILayout.Label("Stamp", GUILayout.MaxWidth(50));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._TerrainStamping._AllowStamping = GUILayout.Toggle(_ForestGroup[_SelectionIndex]._TerrainStamping._AllowStamping, "Activate");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    Texture2D img1 = _ForestGroup[_SelectionIndex]._TerrainStamping._TerrainTex;
                    _ForestGroup[_SelectionIndex]._TerrainStamping._TerrainTex = EditorGUILayout.ObjectField(img1, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                    GUILayout.Space(5);
                    Texture2D img2 = _ForestGroup[_SelectionIndex]._TerrainStamping._StampTex;
                    _ForestGroup[_SelectionIndex]._TerrainStamping._StampTex = EditorGUILayout.ObjectField(img2, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tile Size:", GUILayout.MaxWidth(55));
                    GUILayout.Space(5);
                    GUILayout.Label("X", GUILayout.MaxWidth(15));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._TileSize.x = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._TileSize.x, floatLayout, GUILayout.Width(10));
                    GUILayout.Space(35);
                    GUILayout.Label("Y", GUILayout.MaxWidth(15));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._TileSize.y = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._TileSize.y, floatLayout, GUILayout.Width(10));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Stamp Size:", GUILayout.Width(79));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._StampSize = Mathf.RoundToInt(EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._StampSize, floatLayout));
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stamp Opacity:", GUILayout.MaxWidth(90));
                    EditorGUILayout.LabelField("0", GUILayout.Width(10));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._TerrainStamping._StampOpacity = Mathf.RoundToInt(GUILayout.HorizontalSlider(_ForestGroup[_SelectionIndex]._TerrainStamping._StampOpacity, 0, 100, GUILayout.Width(30)));
                    GUILayout.Space(5);
                    EditorGUILayout.LabelField("100", GUILayout.Width(25));
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField(_ForestGroup[_SelectionIndex]._TerrainStamping._StampOpacity.ToString("0"), floatLayout);
                    GUILayout.EndHorizontal();

                    //_ForestGroup[_SelectionIndex]._TerrainStamping._AllowStampOverlap = GUILayout.Toggle(_ForestGroup[_SelectionIndex]._TerrainStamping._AllowStampOverlap, "Allow Overlapping");
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();


                    GUILayout.Space(5);
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Height", GUILayout.MaxWidth(50));
                    GUILayout.Space(5);
                    _ForestGroup[_SelectionIndex]._TerrainStamping._AllowHeight = GUILayout.Toggle(_ForestGroup[_SelectionIndex]._TerrainStamping._AllowHeight, "Activate");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    Texture2D img3 = _ForestGroup[_SelectionIndex]._TerrainStamping._StampTexHeight;
                    _ForestGroup[_SelectionIndex]._TerrainStamping._StampTexHeight = EditorGUILayout.ObjectField(img3, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Stamp Size:", GUILayout.Width(100));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._StampSizeHeight = Mathf.RoundToInt(EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._StampSizeHeight, floatLayout));
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stamp Strength:", GUILayout.Width(100));
                    GUILayout.Space(5);
                    EditorGUILayout.LabelField("Min:", GUILayout.Width(30));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._MinHeightStrength = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._MinHeightStrength, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    EditorGUILayout.LabelField("Max:", GUILayout.Width(30));
                    _ForestGroup[_SelectionIndex]._TerrainStamping._MaxHeightStrength = EditorGUILayout.FloatField(_ForestGroup[_SelectionIndex]._TerrainStamping._MaxHeightStrength, floatLayout, GUILayout.Width(40));
                    GUILayout.Space(5);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndScrollView();

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);
    
            if (GUILayout.Button("Save Forest Preset"))
            {
                SaveAsset();
            }
            GUILayout.Space(10);
        }

        void SaveAsset()
        {
            _ForestPreset._ForestGroup.Clear();
            for (int i = 0; i < _ForestGroup.Count; i++)
            {
                _ForestPreset._ForestGroup.Add(new ForestGroup());

                if (_ForestGroup[i]._FoliageObjects.Count > 0)
                {
                    for (int n = 0; n < _ForestGroup[i]._FoliageObjects.Count; n++)
                    {
                        _ForestPreset._ForestGroup[i]._FoliageObjects.Add(new FoliageObjects());
                        _ForestPreset._ForestGroup[i]._FoliageObjects[n]._FoliageObject = _ForestGroup[i]._FoliageObjects[n]._FoliageObject;
                        _ForestPreset._ForestGroup[i]._FoliageObjects[n]._ParticleSystem = _ForestGroup[i]._FoliageObjects[n]._ParticleSystem;
                        _ForestPreset._ForestGroup[i]._FoliageObjects[n]._ParticleSystemHeight = _ForestGroup[i]._FoliageObjects[n]._ParticleSystemHeight;
                        for (int s = 0; s < _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects.Count; s++)
                        {
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects.Add(new SatelliteObjects());
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._FaceNormals = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._FaceNormals;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Hover = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Hover;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxNum = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxNum;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Object = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Object;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Rotation = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Rotation;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._YPos = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._YPos;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._SatelliteSpread = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._SatelliteSpread;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Scale = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._Scale;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MinScale = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MinScale;
                            _ForestPreset._ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxScale = _ForestGroup[i]._FoliageObjects[n]._SatelliteObjects[s]._MaxScale;
                        }
                    }
                }

                if (_ForestGroup[i]._OffsetOverDist.keys.Length > 0)
                {
                    _ForestPreset._ForestGroup[i]._OffsetOverDist = new AnimationCurve();
                    for (int n = 0; n < _ForestGroup[i]._OffsetOverDist.keys.Length; n++)
                    {
                        _ForestPreset._ForestGroup[i]._OffsetOverDist.AddKey(_ForestGroup[i]._OffsetOverDist.keys[n].time, _ForestGroup[i]._OffsetOverDist.keys[n].value);
                    }

                    for (int f = 0; f < _ForestGroup[i]._OffsetOverDist.keys.Length; f++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, f, AnimationUtility.GetKeyLeftTangentMode(_ForestGroup[i]._OffsetOverDist, f));
                        AnimationUtility.SetKeyRightTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, f, AnimationUtility.GetKeyRightTangentMode(_ForestGroup[i]._OffsetOverDist, f));
                    }
                }

                if (_ForestGroup[i]._TargetTextures.Count > 0)
                {
                    _ForestPreset._ForestGroup[i]._TargetTextures.Clear();
                    for (int n = 0; n < _ForestGroup[i]._TargetTextures.Count; n++)
                    {
                        _ForestPreset._ForestGroup[i]._TargetTextures.Add(_ForestGroup[i]._TargetTextures[n]);
                    }
                }

                _ForestPreset._ForestGroup[i]._GroupName = _ForestGroup[i]._GroupName;
                _ForestPreset._ForestGroup[i]._Icon = _ForestGroup[i]._Icon;
                _ForestPreset._ForestGroup[i]._MaxOffset = _ForestGroup[i]._MaxOffset;
                _ForestPreset._ForestGroup[i]._MinOffset = _ForestGroup[i]._MinOffset;
                _ForestPreset._ForestGroup[i]._Offset = _ForestGroup[i]._Offset;
                _ForestPreset._ForestGroup[i]._MaxBorderOffset = _ForestGroup[i]._MaxBorderOffset;
                _ForestPreset._ForestGroup[i]._MinBorderOffset = _ForestGroup[i]._MinBorderOffset;
                _ForestPreset._ForestGroup[i]._BorderOffset = _ForestGroup[i]._BorderOffset;
                _ForestPreset._ForestGroup[i]._RandomRotation = _ForestGroup[i]._RandomRotation;
                _ForestPreset._ForestGroup[i]._RandomScale = _ForestGroup[i]._RandomScale;
                _ForestPreset._ForestGroup[i]._MinRotation = _ForestGroup[i]._MinRotation;
                _ForestPreset._ForestGroup[i]._MaxRotation = _ForestGroup[i]._MaxRotation;
                _ForestPreset._ForestGroup[i]._MinScale = _ForestGroup[i]._MinScale;
                _ForestPreset._ForestGroup[i]._MaxScale = _ForestGroup[i]._MaxScale;
                _ForestPreset._ForestGroup[i]._FaceNormals = _ForestGroup[i]._FaceNormals;
                _ForestPreset._ForestGroup[i]._Overlap = _ForestGroup[i]._Overlap;
                _ForestPreset._ForestGroup[i]._Fill = _ForestGroup[i]._Fill;
                _ForestPreset._ForestGroup[i]._OverlapDistance = _ForestGroup[i]._OverlapDistance;
                _ForestPreset._ForestGroup[i]._MinOverlapDistance = _ForestGroup[i]._MinOverlapDistance;
                _ForestPreset._ForestGroup[i]._MaxOverlapDistance = _ForestGroup[i]._MaxOverlapDistance;
                _ForestPreset._ForestGroup[i]._AngleLimit = _ForestGroup[i]._AngleLimit;
                _ForestPreset._ForestGroup[i]._MinAngleLimit = _ForestGroup[i]._MinAngleLimit;
                _ForestPreset._ForestGroup[i]._MaxAngleLimit = _ForestGroup[i]._MaxAngleLimit;
                _ForestPreset._ForestGroup[i]._DigDepth = _ForestGroup[i]._DigDepth;
                _ForestPreset._ForestGroup[i]._MinDigDepth = _ForestGroup[i]._MinDigDepth;
                _ForestPreset._ForestGroup[i]._MaxDigDepth = _ForestGroup[i]._MaxDigDepth;
                _ForestPreset._ForestGroup[i]._FaceBorderDirection = _ForestGroup[i]._FaceBorderDirection;
                _ForestPreset._ForestGroup[i]._UniqueID = _ForestGroup[i]._UniqueID;
                _ForestPreset._ForestGroup[i]._TerrainStamping._AllowStamping = _ForestGroup[i]._TerrainStamping._AllowStamping;
                _ForestPreset._ForestGroup[i]._TerrainStamping._AllowStampOverlap = _ForestGroup[i]._TerrainStamping._AllowStampOverlap;
                _ForestPreset._ForestGroup[i]._TerrainStamping._StampOpacity = _ForestGroup[i]._TerrainStamping._StampOpacity;
                _ForestPreset._ForestGroup[i]._TerrainStamping._StampSize = _ForestGroup[i]._TerrainStamping._StampSize;
                _ForestPreset._ForestGroup[i]._TerrainStamping._StampTex = _ForestGroup[i]._TerrainStamping._StampTex;
                _ForestPreset._ForestGroup[i]._TerrainStamping._TerrainTex = _ForestGroup[i]._TerrainStamping._TerrainTex;
                _ForestPreset._ForestGroup[i]._TerrainStamping._TileSize = _ForestGroup[i]._TerrainStamping._TileSize;
                _ForestPreset._ForestGroup[i]._TerrainStamping._AllowHeight = _ForestGroup[i]._TerrainStamping._AllowHeight;
                _ForestPreset._ForestGroup[i]._TerrainStamping._StampSizeHeight = _ForestGroup[i]._TerrainStamping._StampSizeHeight;
                _ForestPreset._ForestGroup[i]._TerrainStamping._MinHeightStrength = _ForestGroup[i]._TerrainStamping._MinHeightStrength;
                _ForestPreset._ForestGroup[i]._TerrainStamping._MaxHeightStrength = _ForestGroup[i]._TerrainStamping._MaxHeightStrength;
                _ForestPreset._ForestGroup[i]._TerrainStamping._StampTexHeight = _ForestGroup[i]._TerrainStamping._StampTexHeight;

                //Check if group name exists as tag for assignment
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty tagsProp = tagManager.FindProperty("tags");

                bool exists = false;
                for (int n = 0; n < tagsProp.arraySize; n++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(n);
                    if (t.stringValue == _ForestGroup[i]._GroupName)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                    SerializedProperty tag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                    tag.stringValue = _ForestGroup[i]._GroupName;
                    tagManager.ApplyModifiedProperties();
                }
            }
            int id = _ForestPreset.GetInstanceID();
            string path = AssetDatabase.GetAssetPath(id);
            AssetDatabase.RenameAsset(path, _PresetName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(_ForestPreset);
        }

        void CallWarning(string header, string msg, string option1)
        {
            EditorUtility.DisplayDialog(header, msg, option1);
        }

        void DeepCopyGroup(int groupID, int copyID)
        {
            _ForestGroup[copyID]._BorderOffset = _ForestGroup[groupID]._BorderOffset;
            _ForestGroup[copyID]._DigDepth = _ForestGroup[groupID]._DigDepth;
            for(int i = 0; i< _ForestGroup[groupID]._FoliageObjects.Count;i++)
            {
                _ForestGroup[copyID]._FoliageObjects.Add(_ForestGroup[groupID]._FoliageObjects[i]);
            }
            _ForestGroup[copyID]._GroupName = _ForestGroup[groupID]._GroupName;
            _ForestGroup[copyID]._Icon = _ForestGroup[groupID]._Icon;
            _ForestGroup[copyID]._MaxBorderOffset = _ForestGroup[groupID]._MaxBorderOffset;
            _ForestGroup[copyID]._MaxDigDepth = _ForestGroup[groupID]._MaxDigDepth;
            _ForestGroup[copyID]._MaxOffset = _ForestGroup[groupID]._MaxOffset;
            _ForestGroup[copyID]._MaxRotation = _ForestGroup[groupID]._MaxRotation;
            _ForestGroup[copyID]._MaxScale = _ForestGroup[groupID]._MaxScale;
            _ForestGroup[copyID]._MinBorderOffset = _ForestGroup[groupID]._MinBorderOffset;
            _ForestGroup[copyID]._BorderOffset = _ForestGroup[groupID]._BorderOffset;
            _ForestGroup[copyID]._MinDigDepth = _ForestGroup[groupID]._MinDigDepth;
            _ForestGroup[copyID]._MinOffset = _ForestGroup[groupID]._MinOffset;
            _ForestGroup[copyID]._MinRotation = _ForestGroup[groupID]._MinRotation;
            _ForestGroup[copyID]._MinScale = _ForestGroup[groupID]._MinScale;
            _ForestGroup[copyID]._Offset = _ForestGroup[groupID]._Offset;
            _ForestGroup[copyID]._HasParticles = _ForestGroup[groupID]._HasParticles;
            _ForestGroup[copyID]._FaceNormals = _ForestGroup[groupID]._FaceNormals;
            _ForestGroup[copyID]._Overlap = _ForestGroup[groupID]._Overlap;
            _ForestGroup[copyID]._Fill = _ForestGroup[groupID]._Fill;
            _ForestGroup[copyID]._OverlapDistance = _ForestGroup[groupID]._OverlapDistance;
            _ForestGroup[copyID]._MinOverlapDistance = _ForestGroup[groupID]._MinOverlapDistance;
            _ForestGroup[copyID]._MaxOverlapDistance = _ForestGroup[groupID]._MaxOverlapDistance;
            _ForestGroup[copyID]._AngleLimit = _ForestGroup[groupID]._AngleLimit;
            _ForestGroup[copyID]._MinAngleLimit = _ForestGroup[groupID]._MinAngleLimit;
            _ForestGroup[copyID]._MaxAngleLimit = _ForestGroup[groupID]._MaxAngleLimit;
            _ForestGroup[copyID]._DigDepth = _ForestGroup[groupID]._DigDepth;
            _ForestGroup[copyID]._MinDigDepth = _ForestGroup[groupID]._MinDigDepth;
            _ForestGroup[copyID]._MaxDigDepth = _ForestGroup[groupID]._MaxDigDepth;
            _ForestGroup[copyID]._FaceBorderDirection = _ForestGroup[groupID]._FaceBorderDirection;
            _ForestGroup[copyID]._TerrainStamping._AllowStamping = _ForestGroup[groupID]._TerrainStamping._AllowStamping;
            _ForestGroup[copyID]._TerrainStamping._AllowStampOverlap = _ForestGroup[groupID]._TerrainStamping._AllowStampOverlap;
            _ForestGroup[copyID]._TerrainStamping._StampOpacity = _ForestGroup[groupID]._TerrainStamping._StampOpacity;
            _ForestGroup[copyID]._TerrainStamping._StampSize = _ForestGroup[groupID]._TerrainStamping._StampSize;
            _ForestGroup[copyID]._TerrainStamping._StampTex = _ForestGroup[groupID]._TerrainStamping._StampTex;
            _ForestGroup[copyID]._TerrainStamping._TerrainTex = _ForestGroup[groupID]._TerrainStamping._TerrainTex;
            _ForestGroup[copyID]._TerrainStamping._TileSize = _ForestGroup[groupID]._TerrainStamping._TileSize;
            _ForestGroup[copyID]._TerrainStamping._AllowHeight = _ForestGroup[groupID]._TerrainStamping._AllowHeight;
            _ForestGroup[copyID]._TerrainStamping._StampSizeHeight = _ForestGroup[groupID]._TerrainStamping._StampSizeHeight;
            _ForestGroup[copyID]._TerrainStamping._MinHeightStrength = _ForestGroup[groupID]._TerrainStamping._MinHeightStrength;
            _ForestGroup[copyID]._TerrainStamping._MaxHeightStrength = _ForestGroup[groupID]._TerrainStamping._MaxHeightStrength;
            _ForestGroup[copyID]._TerrainStamping._StampTexHeight = _ForestGroup[groupID]._TerrainStamping._StampTexHeight;


            for (int n = 0; n < _ForestGroup[groupID]._OffsetOverDistance.Count;n++)
            {
                _ForestGroup[copyID]._OffsetOverDistance.Add(_ForestGroup[groupID]._OffsetOverDistance[n]);
            }

            if (_ForestGroup[groupID]._TargetTextures.Count > 0)
            {
                _ForestPreset._ForestGroup[groupID]._TargetTextures.Clear();
                for (int n = 0; n < _ForestGroup[groupID]._TargetTextures.Count; n++)
                {
                    _ForestPreset._ForestGroup[groupID]._TargetTextures.Add(_ForestGroup[groupID]._TargetTextures[n]);
                }
            }

            _ForestGroup[copyID]._RandomRotation = _ForestGroup[groupID]._RandomRotation;
            _ForestGroup[copyID]._RandomScale = _ForestGroup[groupID]._RandomScale;
        }

        int GenerateUniqueID()
        {
            int highestValue = 1;
            for(int i = 0; i < _ForestGroup.Count;i++)
            {
                if(_ForestGroup[i]._UniqueID > highestValue)
                {
                    highestValue = _ForestGroup[i]._UniqueID;
                }
            }
            return (highestValue + 1);
        }

        void ResetCurve()
        {
            _ForestGroup[_SelectionIndex]._OffsetOverDist = new AnimationCurve();
            _ForestGroup[_SelectionIndex]._OffsetOverDist.AddKey(0, _ForestGroup[_SelectionIndex]._OverlapDistance);
            _ForestGroup[_SelectionIndex]._OffsetOverDist.AddKey(1, _ForestGroup[_SelectionIndex]._OverlapDistance);
        }
    }

    public class IconPicker : EditorWindow
    {
        Vector2 scrollPosition = Vector2.zero;
        public int selGridInt = 0;
        public Texture2D[] icons;
        string assetPath;
        IconPicker iconpicker_window;
        int indexSelection = 0;
        List<ForestGroup> forestGroup;

        public void Init(int index, List<ForestGroup> forestgroup)
        {
            forestGroup = forestgroup;
            indexSelection = index;
            // Get existing open window or if none, make a new one:
            iconpicker_window = GetWindow<IconPicker>();
            iconpicker_window.Show();
            iconpicker_window.minSize = new Vector2(160, 175);
            iconpicker_window.maxSize = new Vector2(160, 175);
        }

        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
        }

        void OnScene(SceneView sceneview)
        {

        }

        public void OnGUI()
        {
            if (assetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        assetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (assetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                assetPath = dir;
                            }
                        }
                    }
                }
            }

            if (!System.IO.Directory.Exists(assetPath + "/InternalResources/Sprites/Icons"))
            {
                iconpicker_window = GetWindow<IconPicker>();
                iconpicker_window.Focus();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(6);
            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Width(140), GUILayout.Height(115));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(140), GUILayout.Height(115));
            string[] files = System.IO.Directory.GetFiles(assetPath + "/InternalResources/Sprites/Icons", "*.png");
            icons = new Texture2D[files.Length];

            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(files[i]);
            }
            GUIStyle cellStyle = new GUIStyle(GUI.skin.button);
            cellStyle.fixedHeight = 35;

            selGridInt = GUILayout.SelectionGrid(selGridInt, icons, 3, cellStyle, GUILayout.Width(115), GUILayout.Height(115));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(7);
            if (GUILayout.Button("Cancel", GUILayout.Width(70)))
            {
                iconpicker_window = GetWindow<IconPicker>();
                iconpicker_window.Close();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Ok", GUILayout.Width(70)))
            {
                forestGroup[indexSelection]._Icon = icons[selGridInt];
                iconpicker_window = GetWindow<IconPicker>();
                iconpicker_window.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}


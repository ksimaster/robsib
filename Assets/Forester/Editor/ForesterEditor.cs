using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace Forester
{
    [CustomEditor(typeof(ForesterTool))]
    [System.Serializable]
    public class ForesterEditor : Editor
    {
        string _AssetPath;
        ForesterTool t;
        private ForesterPresetEditor _ForestPresetEditor;
        private AreaEditor _AreaEditor;
        private bool _EditMode = false;
        private GUIStyle[] _IncludeStyle;
        private GUIStyle[] _ParticlesStyle;
        private GUIStyle[] _FaceNormalsStyle;
        private int _GroupSelectIndex;
        private bool[] _GroupSelect;
        private GUIStyle[] _GroupSelectStyle;
        private bool _ClosedEnd = true;

        private bool _EditForestMode = false;
        private bool _PaintMode = false;
        private bool _FilterSelectionMode = false;

        private int _SelectedFoliageObject = -1;

        private bool _ByGroup = false;

        private bool _CtrlKeyDown = false;
        private bool _ShiftKeyDown = false;

        private float _TimeDelay = 0;

        private bool showOffsetOptions = false;
        private bool showGroupOptions = false;
        private bool showTxFilterOptions = false;
        private bool showStencilMaskOptions = false;
        private bool showStampOptions = false;
        private bool showRandOptions = false;
        private bool showSubOffsetOptions = false;
        private bool showOverDistOptions = false;

        Vector2 _GroupScroll = Vector2.zero;
        Vector2 _TargetTexScroll = Vector2.zero;
        Vector2 _StencilMaskScroll = Vector2.zero;

        Texture2D _NormalButton;
        Texture2D _ActiveButton;
        Color _ButtonFontColour;

        // draw lines between a chosen game object
        // and a selection of added game objects
        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;

            int num = 0;
            _IncludeStyle = new GUIStyle[num];
            _ParticlesStyle = new GUIStyle[num];
            _GroupSelectStyle = new GUIStyle[num];
            _FaceNormalsStyle = new GUIStyle[num];
            _GroupSelect = new bool[num];
            _GroupSelectIndex = -1;

            //TO BE REMOVED IN V1.4 THIS IS A LEGACY FIX
            t = target as ForesterTool;
            if (t._ForestGroup.Count > 0 && t._ForestGroup[0]._GroupName == "")
            {
                for (int i = 0; i < t._ForestGroup.Count; i++)
                {
                    t._ForestGroup[i]._Icon = t._ForestPreset._ForestGroup[i]._Icon;
                    t._ForestGroup[i]._GroupName = t._ForestPreset._ForestGroup[i]._GroupName;
                    t.Rebuild();
                }
            }
        }

        void OnDisable()
        {
            _EditMode = false;
            _PaintMode = false;
            ClearFrozen();
        }

        void OnScene(SceneView sceneview)
        {
            // get the chosen game object
            t = target as ForesterTool;
            if (_AssetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        _AssetPath = dir;
                        t._AssetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (_AssetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                _AssetPath = dir;
                                t._AssetPath = dir;
                            }
                        }
                    }
                }
            }
            _ClosedEnd = t._ClosedEnd;

            if (t == null || t._Points == null)
                return;

            // grab the center of the parent
            Vector3 center = t.transform.position;

            // iterate over game objects added to the array...
            if (t._Points.Count > 1)
            {
                for (int i = 0; i <= t._Points.Count - 1; i++)
                {
                    // ... and draw a line between them
                    if (t._Points[i] != null)
                        if (i > 0 && i < t._Points.Count - 1 && t._ClosedEnd || i > 0 && i <= t._Points.Count && !t._ClosedEnd)
                        {
                            Handles.DrawLine(t._Points[i - 1].transform.position, t._Points[i].transform.position);
                        }
                        else if (i == t._Points.Count - 1)
                        {
                            Handles.DrawLine(t._Points[i - 1].transform.position, t._Points[i].transform.position);
                            Handles.DrawLine(t._Points[i].transform.position, t._Points[0].transform.position);
                        }
                }
            }

            Event e = Event.current;
            _ShiftKeyDown = e.modifiers == EventModifiers.Shift ? true : false;
            _CtrlKeyDown = e.keyCode == KeyCode.LeftControl ? true : false;

            if (_EditMode)
            {
                SetSceneDirty();
                Ray ray;
                RaycastHit hit;
                ray = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y));

                if (Event.current.shift) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                if (Physics.Raycast(ray, out hit))
                {
                    int layerNum = LayerMask.NameToLayer("Terrain");
                    if (hit.transform.gameObject.layer == layerNum)
                    {
                        bool mouseActive = false;


                        if (e.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift && mouseActive == false)
                        {
                            mouseActive = true;
                            CreatePoint(t._Point, hit.point);
                        }
                        if (e.type == EventType.MouseUp)
                        {
                            mouseActive = false;
                        }
                    }
                }
            }

            if (_PaintMode && _GroupSelectIndex >= 0 && _SelectedFoliageObject >= 0)
            {
                SetSceneDirty();
                Ray ray;
                RaycastHit hit;
                ray = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y));

                bool mouseActive = false;

                if (Event.current.shift) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                if (Physics.Raycast(ray, out hit))
                {
                    int layerNum = LayerMask.NameToLayer("Terrain");
                    if (hit.transform.gameObject.layer == layerNum && !_CtrlKeyDown)
                    {
                        if (_ShiftKeyDown)
                        {
                            while (EditorApplication.timeSinceStartup < _TimeDelay)
                            {
                                return;
                            }
                            mouseActive = true;
                            t.CheckPrerequisites(_GroupSelectIndex, _SelectedFoliageObject, false, true, hit);
                            _TimeDelay = (float)EditorApplication.timeSinceStartup + t._ForestGroup[_GroupSelectIndex]._PaintFreq;
                        }
                    }


                    if (_CtrlKeyDown)
                    {
                        if (hit.transform.gameObject.name == t._ForestPreset._ForestGroup[_GroupSelectIndex]._FoliageObjects[_SelectedFoliageObject]._FoliageObject.name)
                        {
                            if (e.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift && mouseActive == false || e.type == EventType.KeyDown)
                            {
                                t.RemoveFoliage(_GroupSelectIndex, hit.transform.gameObject, -1);
                                return;
                            }
                        }
                        else
                        {

                            for (int i = 0; i < t._CreatedObjects[_GroupSelectIndex]._FoliageInfo.Count; i++)
                            {
                                _CtrlKeyDown = e.keyCode == KeyCode.LeftControl ? true : false;
                                if (e.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift && mouseActive == false || e.type == EventType.KeyDown)
                                {
                                    if (t._CreatedObjects[_GroupSelectIndex]._FoliageInfo[i]._FoliageObject == null) return;
                                    if (t._CreatedObjects[_GroupSelectIndex]._FoliageInfo[i]._FoliageObject.name == t._ForestPreset._ForestGroup[_GroupSelectIndex]._FoliageObjects[_SelectedFoliageObject]._FoliageObject.name)
                                    {
                                        float dist = Vector3.Distance(t._CreatedObjects[_GroupSelectIndex]._FoliageInfo[i]._SurfacePos, hit.point);
                                        if (dist < 0.5f)
                                        {
                                            if (_CtrlKeyDown) t.RemoveFoliage(_GroupSelectIndex, t._CreatedObjects[_GroupSelectIndex]._FoliageInfo[i]._FoliageObject, i);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (e.type == EventType.MouseUp)
                {
                    mouseActive = false;
                }
            }

            if (t._ForestPreset == null || t._ForestPreset.name != t._ForestPresetName)
            {
                _GroupSelectIndex = 0;
                _ByGroup = false;
            }
        }

        public override void OnInspectorGUI()
        {
            if (_AssetPath == null)
            {
                return;
            }

            if (t._ForestPreset == null)
            {
                int num = 0;
                t._ForestGroup.Clear();
                t._IncludeGroup = new bool[num];
                _IncludeStyle = new GUIStyle[num];
                _ParticlesStyle = new GUIStyle[num];
                _FaceNormalsStyle = new GUIStyle[num];
                _GroupSelectStyle = new GUIStyle[num];
                _GroupSelect = new bool[num];
                _GroupSelectIndex = -1;
                t._ForestPresetName = null;

            }


            if (t._ForestPreset != null)
            {
                if (t._ForestPreset.name != t._ForestPresetName)
                {
                    int num = t._ForestPreset._ForestGroup.Count;
                    t._ForestGroup.Clear();
                    for (int i = 0; i < num; i++)
                    {
                        t._ForestGroup.Add(new ForestGroup());
                    }
                    _IncludeStyle = new GUIStyle[num];
                    _ParticlesStyle = new GUIStyle[num];
                    _FaceNormalsStyle = new GUIStyle[num];
                    _GroupSelectStyle = new GUIStyle[num];
                    _GroupSelect = new bool[num];

                    for (int i = 0; i < num; i++)
                    {
                        t._ForestGroup[i]._RandomRotation = t._ForestPreset._ForestGroup[i]._RandomRotation;
                        t._ForestGroup[i]._MinRotation = t._ForestPreset._ForestGroup[i]._MinRotation;
                        t._ForestGroup[i]._MaxRotation = t._ForestPreset._ForestGroup[i]._MaxRotation;

                        t._ForestGroup[i]._RandomScale = t._ForestPreset._ForestGroup[i]._RandomScale;
                        t._ForestGroup[i]._MinScale = t._ForestPreset._ForestGroup[i]._MinScale;
                        t._ForestGroup[i]._MaxScale = t._ForestPreset._ForestGroup[i]._MaxScale;

                        t._ForestGroup[i]._MinOffset = t._ForestPreset._ForestGroup[i]._MinOffset;
                        t._ForestGroup[i]._MaxOffset = t._ForestPreset._ForestGroup[i]._MaxOffset;
                        t._ForestGroup[i]._Offset = t._ForestPreset._ForestGroup[i]._Offset;

                        t._ForestGroup[i]._MinBorderOffset = t._ForestPreset._ForestGroup[i]._MinBorderOffset;
                        t._ForestGroup[i]._MaxBorderOffset = t._ForestPreset._ForestGroup[i]._MaxBorderOffset;
                        t._ForestGroup[i]._BorderOffset = t._ForestPreset._ForestGroup[i]._BorderOffset;

                        t._ForestGroup[i]._MinDigDepth = t._ForestPreset._ForestGroup[i]._MinDigDepth;
                        t._ForestGroup[i]._MaxDigDepth = t._ForestPreset._ForestGroup[i]._MaxDigDepth;
                        t._ForestGroup[i]._DigDepth = t._ForestPreset._ForestGroup[i]._DigDepth;

                        t._ForestGroup[i]._FaceNormals = t._ForestPreset._ForestGroup[i]._FaceNormals;
                        t._ForestGroup[i]._HasParticles = t._ForestPreset._ForestGroup[i]._HasParticles;

                        for (int n = 0; n < t._ForestPreset._ForestGroup[i]._OffsetOverDistance.Count; n++)
                        {
                            t._ForestGroup[i]._OffsetOverDistance.Add(new OffsetOverDistance());
                            t._ForestGroup[i]._OffsetOverDistance[n].endDist = t._ForestPreset._ForestGroup[i]._OffsetOverDistance[n].endDist;
                            t._ForestGroup[i]._OffsetOverDistance[n].startDist = t._ForestPreset._ForestGroup[i]._OffsetOverDistance[n].startDist;
                            t._ForestGroup[i]._OffsetOverDistance[n].OffsetOverDist = Mathf.Clamp(t._ForestPreset._ForestGroup[i]._OffsetOverDistance[n].OffsetOverDist, 1, 100);
                        }
                    }
                    t._ForestPresetName = t._ForestPreset.name;
                }
                else
                {
                    if (_IncludeStyle.Length != t._ForestPreset._ForestGroup.Count)
                    {
                        int num = t._ForestGroup.Count;
                        _IncludeStyle = new GUIStyle[num];
                        _ParticlesStyle = new GUIStyle[num];
                        _FaceNormalsStyle = new GUIStyle[num];
                        _GroupSelectStyle = new GUIStyle[num];
                        _GroupSelect = new bool[num];
                    }
                }
            }
            _ButtonFontColour = Color.black;

            Color separatorsCol;

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                separatorsCol = new Color(0.5f, 0.5f, 0.5f, 0.6f);
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
            }
            else
            {
                separatorsCol = new Color(0.5f, 0.5f, 0.5f, 0.35f);
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");
            }

            var popupStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
            popupStyle.alignment = TextAnchor.MiddleCenter;


            var textFloatingLayout = new GUIStyle();
            textFloatingLayout.alignment = TextAnchor.MiddleCenter;
            textFloatingLayout.fixedWidth = 40;

            var floatLayout = new GUIStyle(GUI.skin.textArea);
            floatLayout.alignment = TextAnchor.MiddleCenter;
            floatLayout.stretchWidth = true;
            floatLayout.fixedWidth = 40;

            //Style for normal buttons that dont highlight
            var normalStyle = new GUIStyle(GUI.skin.button);
            normalStyle.normal.background = _NormalButton;

            GUIStyle deleteStyle = new GUIStyle(GUI.skin.button);
            deleteStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Red.png");

            GUIStyle activeStyle = new GUIStyle(GUI.skin.button);
            activeStyle.normal.background = _ActiveButton;
            activeStyle.normal.textColor = _ButtonFontColour;

            GUIStyle notActiveStyle = new GUIStyle(GUI.skin.button);
            notActiveStyle.normal.background = _NormalButton;

            Rect pos = EditorGUILayout.BeginHorizontal();
            float div = (float)Screen.width/512;
            GUI.DrawTexture(new Rect(pos.x,pos.y,Mathf.Clamp(512 * div,128,512),Mathf.Clamp(65 * div,10,65)), AssetDatabase.LoadAssetAtPath<Texture>(_AssetPath + "/InternalResources/Sprites/Forester.png"));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(Mathf.Clamp(65 * div, 10, 65));

            pos = EditorGUILayout.BeginHorizontal();
            GUI.color = separatorsCol;
            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(15 * div, 15, 15)),"", GUI.skin.button);
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Documentation",GUI.skin.label))
            {
                Application.OpenURL("https://docs.google.com/document/d/1HvF5zEeA9FJ8WXhVv1Ry3bryq0DrG4iTYu2-X15cRUQ/edit?usp=sharing");
            }
            GUILayout.Label("|");
            if (GUILayout.Button("Video Tutorials", GUI.skin.label))
            {
                Application.OpenURL("https://www.youtube.com/playlist?list=PL9CU-Qj1tQBXjaTqS6C8sUInXOpbksrBG");
            }
            GUILayout.Label("|");
            if (GUILayout.Button("Contact", GUI.skin.label))
            {
                EmailMe();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            var bannerLayout = new GUIStyle(GUI.skin.button);
            bannerLayout.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("Create Area", textLayout);
            if (_EditMode) EditorGUILayout.LabelField("Use shift + Left click to create a forest node", textLayout);
            GUILayout.Space(5);

            var style = new GUIStyle(GUI.skin.button);

            if (_EditMode)
            {
                style.normal.background = _ActiveButton;
                style.normal.textColor = _ButtonFontColour;
            }
            else
            {
                style.normal.background = _NormalButton;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------
            //Create Area 
            //-------------------------------------------------------------------------------------------------------------------------------------------------------

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            SerializedObject serialized = new SerializedObject(t);
            SerializedProperty areaPreset = serialized.FindProperty("_AreaPreset");
            serialized.Update();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.ObjectField(areaPreset, typeof(Object), GUILayout.ExpandWidth(true));
            serialized.ApplyModifiedProperties();

            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                _AreaEditor = CreateInstance<AreaEditor>();
                _AreaEditor.StartEditor(t._AreaPreset);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rebuild", normalStyle, GUILayout.MaxWidth(100)))
            {
                t.Rebuild();
            }

            if (GUILayout.Button("Edit Area", style))
            {
                _EditMode = _EditMode ? false : true;
                if (!_EditMode) Selection.activeGameObject = t.gameObject; 
                ActiveEditorTracker.sharedTracker.isLocked = _EditMode;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            var closeEndStyle = new GUIStyle(GUI.skin.button);

            if (_ClosedEnd)
            {
                closeEndStyle.normal.background = _ActiveButton;
                closeEndStyle.normal.textColor = _ButtonFontColour;
            }
            else
            {
                closeEndStyle.normal.background = _NormalButton;
            }


            if (GUILayout.Button("Closed End", closeEndStyle))
            {
                if (!_ClosedEnd)
                {
                    _ClosedEnd = true;
                    t._ClosedEnd = true;
                }
                else
                {
                    _ClosedEnd = false;
                    t._ClosedEnd = false;
                }
            }
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GameObject selection = Selection.activeGameObject;
            if (GUILayout.Button("Insert", normalStyle))
            {
                if (selection == null || !selection.name.Contains("Node"))
                {
                    EditorUtility.DisplayDialog("No forest node selected", "You must first select a forest node before trying to insert a new one", "Opps Ok");
                    return;
                }
                if (selection.GetComponent<PointNode>() != null)
                {
                    if (selection.GetComponent<PointNode>().forester != t)
                    {
                        EditorUtility.DisplayDialog("Invalid node selected", "Please select a forest node associated with this Forester toolset or change to this nodes toolset", "Opps Ok");
                        return;
                    }
                }
                Insert();
            }

            if (GUILayout.Button("Remove Selected", normalStyle))
            {
                if (selection == null || !selection.name.Contains("Node"))
                {
                    EditorUtility.DisplayDialog("No forest node selected", "You must first select a forest node before trying to remove one", "Opps Ok");
                    return;
                }
                if (selection.GetComponent<PointNode>() != null)
                {
                    if (selection.GetComponent<PointNode>().forester != t)
                    {
                        EditorUtility.DisplayDialog("Invalid node selected", "Please select a forest node associated with this Forester toolset or change to this nodes toolset", "Opps Ok");
                        return;
                    }
                }
                RemoveSelectedPoint();
            }

            if (GUILayout.Button("Clear All", normalStyle))
            {
                ClearAllPoints();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            pos = EditorGUILayout.BeginHorizontal();
            GUI.color = separatorsCol;
            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Forest Creation", textLayout);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            SerializedProperty forestPreset = serialized.FindProperty("_ForestPreset");
            serialized.Update();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.ObjectField(forestPreset, typeof(Object), GUILayout.ExpandWidth(true));
            serialized.ApplyModifiedProperties();
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                _ForestPresetEditor = CreateInstance<ForesterPresetEditor>();
                _ForestPresetEditor.StartEditor(t._ForestPreset);
            }

            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                if (t._ForestPreset != null)
                {
                    t.SaveToSelectedPreset();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (t._ForestPreset != null)
            {
                pos = EditorGUILayout.BeginHorizontal();
                GUI.color = separatorsCol;
                GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                int includeCount = 0;
                for (int i = 0; i < t._IncludeGroup.Length; i++)
                {
                    if (t._IncludeGroup[i]) includeCount++;
                }
                if (includeCount > 0)
                {
                    if (GUILayout.Button("Grow Forest", normalStyle, GUILayout.ExpandWidth(true)))
                    {
                        if (_ByGroup && _GroupSelectIndex < 0)
                        {
                            return;
                        }
                        AddTag("Foliage");
                        t.Deforestation(_ByGroup, _GroupSelectIndex);
                        t._ShowProgressBar = true;
                        t.CheckPrerequisites(_GroupSelectIndex, 0, _ByGroup, false);
                    }
                }
                if (GUILayout.Button("Deforestation", normalStyle, GUILayout.ExpandWidth(true)))
                {
                    t.Deforestation(_ByGroup, _GroupSelectIndex,true);
                }

                _ByGroup = GUILayout.Toggle(_ByGroup, "By Group?", GUILayout.Width(78));

                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(100));
                _GroupScroll = GUILayout.BeginScrollView(_GroupScroll, GUILayout.ExpandWidth(true), GUILayout.Height(100));
                GUILayout.BeginVertical(GUILayout.Height(25));
                //Check if array length for groups is still valid
                if (t._IncludeGroup.Length != t._ForestGroup.Count)
                {
                    int num = t._ForestGroup.Count;
                    t._IncludeGroup = new bool[num];
                    _IncludeStyle = new GUIStyle[num];
                    _FaceNormalsStyle = new GUIStyle[num];
                    _ParticlesStyle = new GUIStyle[num];
                    _GroupSelectStyle = new GUIStyle[num];
                    _GroupSelect = new bool[num];
                    _GroupSelectIndex = -1;
                }
                //For Loop to load preset groups
                if (t._ForestGroup.Count > 0)
                {
                    for (int i = 0; i < t._ForestGroup.Count; i++)
                    {
                        GUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(25));
                        _GroupSelectStyle[i] = new GUIStyle(GUI.skin.button);
                        if (_GroupSelect[i])
                        {
                            _GroupSelectStyle[i].normal.background = _ActiveButton;
                            _GroupSelectStyle[i].normal.textColor = _ButtonFontColour;
                        }
                        if (!_GroupSelect[i]) _GroupSelectStyle[i].normal.background = _NormalButton;
                        if (GUILayout.Button(">", _GroupSelectStyle[i], GUILayout.Width(22), GUILayout.Height(22)))
                        {
                            _GroupSelectIndex = i;
                            for (int n = 0; n < _GroupSelect.Length; n++)
                            {
                                _GroupSelect[n] = false;
                            }
                            _GroupSelect[i] = true;
                        }

                        Texture particlesTx;
                        Texture normalsTx;
                        Texture includeTx;

                        if (EditorGUIUtility.isProSkin)
                        {
                            particlesTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Particles.png");
                            normalsTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Normals.png");
                            includeTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_NotVisible_White.png");
                        }
                        else
                        {
                            particlesTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Particles_Black.png");
                            normalsTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Normals_Black.png");
                            includeTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_NotVisible.png");
                        }

                        _IncludeStyle[i] = new GUIStyle(GUI.skin.button);
                        if (t._IncludeGroup[i])
                        {
                            includeTx = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Visible_Black.png");
                            _IncludeStyle[i].normal.background = _ActiveButton;
                            _IncludeStyle[i].normal.textColor = _ButtonFontColour;
                        }
                        if (!t._IncludeGroup[i]) _IncludeStyle[i].normal.background = _NormalButton;
                        _ParticlesStyle[i] = new GUIStyle(GUI.skin.button);

                        if (t._ForestGroup[i]._HasParticles)
                        {
                            particlesTx = EditorGUIUtility.isProSkin ? AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Particles_Black.png") : particlesTx;
                            _ParticlesStyle[i].normal.background = _ActiveButton;
                            _ParticlesStyle[i].normal.textColor = _ButtonFontColour;
                        }
                        if (!t._ForestGroup[i]._HasParticles) _ParticlesStyle[i].normal.background = _NormalButton;
                        _FaceNormalsStyle[i] = new GUIStyle(GUI.skin.button);
                        if (t._ForestGroup[i]._FaceNormals)
                        {
                            normalsTx = EditorGUIUtility.isProSkin ? AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Normals_Black.png") : normalsTx;
                            _FaceNormalsStyle[i].normal.background = _ActiveButton;
                            _FaceNormalsStyle[i].normal.textColor = _ButtonFontColour;
                        }

                        if (!t._ForestGroup[i]._FaceNormals) _FaceNormalsStyle[i].normal.background = _NormalButton;

                        if (t._ForestPreset != null && t._ForestPreset._ForestGroup.Count > i)
                        {
                            GUILayout.Label(t._ForestPreset._ForestGroup[i]._Icon, GUILayout.Width(25), GUILayout.Height(25));
                            GUILayout.Label(t._ForestPreset._ForestGroup[i]._GroupName, textLayout, GUILayout.MinWidth(50), GUILayout.ExpandWidth(true), GUILayout.Height(25));
                        }
                        if (GUILayout.Button(new GUIContent(particlesTx, "Enable particle effects"), _ParticlesStyle[i], GUILayout.Width(10), GUILayout.Width(32), GUILayout.Height(22)))
                        {
                            t._ForestGroup[i]._HasParticles = t._ForestGroup[i]._HasParticles ? false : true;
                        }
                        if (GUILayout.Button(new GUIContent(normalsTx, "Align to surface normals"), _FaceNormalsStyle[i], GUILayout.Width(10), GUILayout.Width(32), GUILayout.Height(22)))
                        {
                            t._ForestGroup[i]._FaceNormals = t._ForestGroup[i]._FaceNormals ? false : true;
                        }
                        if (GUILayout.Button(new GUIContent(includeTx, "Include group"), _IncludeStyle[i], GUILayout.Width(10), GUILayout.Width(32), GUILayout.Height(22)))
                        {
                            t._IncludeGroup[i] = t._IncludeGroup[i] ? false : true;
                            t.ShowHideGroups(i);
                            SetSceneDirty();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                //------------------------------
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Include All"))
                {
                    t.IncludeAllGroups();
                }
                if (GUILayout.Button("Include None"))
                {
                    t.IncludeAllGroups(false);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                //------------------------------
                if (_GroupSelectIndex >= 0)
                {
                    //----------------------------------------------------------------------------------------------------------------------------
                    //Offset over distance override
                    pos = EditorGUILayout.BeginHorizontal();
                    GUI.color = separatorsCol;
                    GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);

                    string offsetState = showOffsetOptions ? "-" : "+";
                    Rect offsetBanner = EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Forest Options", bannerLayout))
                    {
                        showOffsetOptions = showOffsetOptions ? false : true;
                    }
                    GUI.Label(new Rect(offsetBanner.x + 5, offsetBanner.y, 20, 20), offsetState);
                    EditorGUILayout.EndHorizontal();

                    if (showOffsetOptions)
                    {
                        GUILayout.Space(5);
                        string subOffsetState = showSubOffsetOptions ? "-" : "+";
                        Rect subOffsetBanner = EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Offset Options", textLayout))
                        {
                            showSubOffsetOptions = showSubOffsetOptions ? false : true;
                        }
                        GUI.Label(new Rect(subOffsetBanner.x + 5, subOffsetBanner.y, 20, 20), subOffsetState);
                        EditorGUILayout.EndHorizontal();

                        if (showSubOffsetOptions)
                        {
                            GUILayout.Space(5);
                            GUILayout.BeginVertical();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Density -", GUILayout.Width(75));
                            t._ForestGroup[_GroupSelectIndex]._MinOffset = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinOffset, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            t._ForestGroup[_GroupSelectIndex]._Offset = GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._Offset, t._ForestGroup[_GroupSelectIndex]._MinOffset, t._ForestGroup[_GroupSelectIndex]._MaxOffset,GUILayout.MaxWidth(120));
                            t._ForestGroup[_GroupSelectIndex]._MaxOffset = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxOffset, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);

                            EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                            EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._Offset.ToString("0.00"), floatLayout);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            if (t._ClosedEnd)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField("Fill - ", GUILayout.MaxWidth(75));
                                GUILayout.Label("0%", floatLayout, GUILayout.Width(40));
                                GUILayout.Space(5);
                                t._ForestGroup[_GroupSelectIndex]._Fill = GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._Fill, 0, 100,GUILayout.MaxWidth(120));
                                GUILayout.Label("100%", floatLayout, GUILayout.Width(40));
                                GUILayout.Space(5);
                                EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                                t._ForestGroup[_GroupSelectIndex]._Fill = EditorGUILayout.FloatField(Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._Fill), floatLayout, GUILayout.Width(20));
                                GUILayout.Space(20);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }

                            if (!t._ClosedEnd)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField("Offset -", GUILayout.Width(75));
                                t._ForestGroup[_GroupSelectIndex]._MinBorderOffset = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinBorderOffset, floatLayout, GUILayout.Width(40));
                                GUILayout.Space(5);
                                t._ForestGroup[_GroupSelectIndex]._BorderOffset = GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._BorderOffset, t._ForestGroup[_GroupSelectIndex]._MinBorderOffset, t._ForestGroup[_GroupSelectIndex]._MaxBorderOffset, GUILayout.MaxWidth(120));
                                t._ForestGroup[_GroupSelectIndex]._MaxBorderOffset = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxBorderOffset, floatLayout, GUILayout.Width(40));
                                GUILayout.Space(5);
                                EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                                EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._BorderOffset.ToString("0.00"), floatLayout);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Dig Depth -", GUILayout.Width(75));
                            t._ForestGroup[_GroupSelectIndex]._MinDigDepth = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinDigDepth, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            t._ForestGroup[_GroupSelectIndex]._DigDepth = GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._DigDepth, t._ForestGroup[_GroupSelectIndex]._MinDigDepth, t._ForestGroup[_GroupSelectIndex]._MaxDigDepth, GUILayout.MaxWidth(120));
                            t._ForestGroup[_GroupSelectIndex]._MaxDigDepth = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxDigDepth, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                            EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._DigDepth.ToString("0.00"), floatLayout);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Angle Limit -", GUILayout.Width(75));
                            t._ForestGroup[_GroupSelectIndex]._MinAngleLimit = EditorGUILayout.FloatField(Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._MinAngleLimit), floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            t._ForestGroup[_GroupSelectIndex]._AngleLimit = GUILayout.HorizontalSlider(Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._AngleLimit), Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._MinAngleLimit), Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._MaxAngleLimit), GUILayout.MaxWidth(120));
                            t._ForestGroup[_GroupSelectIndex]._MaxAngleLimit = EditorGUILayout.FloatField(Mathf.RoundToInt(t._ForestGroup[_GroupSelectIndex]._MaxAngleLimit), floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                            EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._AngleLimit.ToString("0"), floatLayout);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();

                            pos = EditorGUILayout.BeginHorizontal();
                            GUI.color = separatorsCol;
                            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);

                            if (!_ClosedEnd)
                            {
                                GUILayout.Space(5);
                                GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                t._ForestGroup[_GroupSelectIndex]._FaceBorderDirection = GUILayout.Toggle(t._ForestGroup[_GroupSelectIndex]._FaceBorderDirection, "Face Border Direction");
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                        }
                        //Check that values havent dipped to 0 which is not allowed
                        if (t._ForestGroup[_GroupSelectIndex]._MinOffset < 0) t._ForestGroup[_GroupSelectIndex]._MinOffset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._MaxOffset < 0) t._ForestGroup[_GroupSelectIndex]._MaxOffset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._Offset < 0) t._ForestGroup[_GroupSelectIndex]._Offset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._MinOffset < 0) t._ForestGroup[_GroupSelectIndex]._MinOffset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._MinBorderOffset < 0) t._ForestGroup[_GroupSelectIndex]._MinBorderOffset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._MaxBorderOffset < 0) t._ForestGroup[_GroupSelectIndex]._MaxBorderOffset = 0.01f;
                        if (t._ForestGroup[_GroupSelectIndex]._BorderOffset < 0) t._ForestGroup[_GroupSelectIndex]._BorderOffset = 0.01f;


                        if (t._ClosedEnd)
                        {
                            GUILayout.Space(5);
                            string overDistState = showOverDistOptions ? "-" : "+";
                            Rect overDistBanner = EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Offset Over Distance", textLayout))
                            {
                                showOverDistOptions = showOverDistOptions ? false : true;
                            }
                            GUI.Label(new Rect(overDistBanner.x + 5, overDistBanner.y, 20, 20), overDistState);
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            if (showOverDistOptions)
                            {
                                EditorGUILayout.CurveField(t._ForestGroup[_GroupSelectIndex]._OffsetOverDist);
                                GUILayout.BeginHorizontal();
                                if(GUILayout.Button("Reset Curve"))
                                {
                                    t._ForestGroup[_GroupSelectIndex]._OffsetOverDist = new AnimationCurve();
                                    t._ForestGroup[_GroupSelectIndex]._OffsetOverDist.AddKey(0, t._ForestGroup[_GroupSelectIndex]._OverlapDistance);
                                    t._ForestGroup[_GroupSelectIndex]._OffsetOverDist.AddKey(1, t._ForestGroup[_GroupSelectIndex]._OverlapDistance);
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.Space(5);
                            }
                        }
                    }

                    pos = EditorGUILayout.BeginHorizontal();
                    GUI.color = separatorsCol;
                    GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);

                    string groupState = showGroupOptions ? "-" : "+";
                    Rect groupBanner = EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Group Options", bannerLayout))
                    {
                        showGroupOptions = showGroupOptions ? false : true;
                    }
                    GUI.Label(new Rect(groupBanner.x + 5, groupBanner.y, 20, 20), groupState);
                    EditorGUILayout.EndHorizontal();

                    if (showGroupOptions)
                    {
                        GUILayout.Space(5);
                        string randState = showRandOptions ? "-" : "+";
                        Rect randBanner = EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Randomisation", textLayout))
                        {
                            showRandOptions = showRandOptions ? false : true;
                        }
                        GUI.Label(new Rect(randBanner.x + 5, randBanner.y, 20, 20), randState);
                        EditorGUILayout.EndHorizontal();

                        if(showRandOptions)
                        {
                            GUILayout.Space(5);
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUIStyle rotation = new GUIStyle(GUI.skin.button);
                            if (t._ForestGroup[_GroupSelectIndex]._RandomRotation)
                            {
                                rotation.normal.background = _ActiveButton;
                                rotation.normal.textColor = _ButtonFontColour;
                            }
                            else
                            {
                                rotation.normal.background = _NormalButton;
                            }

                            if (GUILayout.Button("Random Rotation", rotation, GUILayout.MaxWidth(150), GUILayout.Height(30)))
                            {
                                if (t._ForestGroup[_GroupSelectIndex]._RandomRotation)
                                {
                                    t._ForestGroup[_GroupSelectIndex]._RandomRotation = false;
                                }
                                else
                                {
                                    t._ForestGroup[_GroupSelectIndex]._RandomRotation = true;
                                }
                            }
                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            GUILayout.Label("Min");
                            GUILayout.EndHorizontal();
                            t._ForestGroup[_GroupSelectIndex]._MinRotation = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinRotation, floatLayout, GUILayout.Width(50));
                            GUILayout.EndVertical();

                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max");
                            GUILayout.EndHorizontal();
                            t._ForestGroup[_GroupSelectIndex]._MaxRotation = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxRotation, floatLayout, GUILayout.Width(50));
                            GUILayout.EndVertical();

                            GUILayout.Space(5);

                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);
                            
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUIStyle scale = new GUIStyle(GUI.skin.button);
                            if (t._ForestGroup[_GroupSelectIndex]._RandomScale)
                            {
                                scale.normal.background = _ActiveButton;
                                scale.normal.textColor = _ButtonFontColour;
                            }
                            else
                            {
                                scale.normal.background = _NormalButton;
                            }

                            if (GUILayout.Button("Random Scale", scale, GUILayout.MaxWidth(150), GUILayout.Height(30)))
                            {
                                t._ForestGroup[_GroupSelectIndex]._RandomScale = t._ForestGroup[_GroupSelectIndex]._RandomScale ? false : true;
                            }
                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            GUILayout.Label("Min");
                            GUILayout.EndHorizontal();
                            t._ForestGroup[_GroupSelectIndex]._MinScale = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinScale, floatLayout, GUILayout.Width(50));
                            GUILayout.EndVertical();

                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max");
                            GUILayout.EndHorizontal();
                            t._ForestGroup[_GroupSelectIndex]._MaxScale = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxScale, floatLayout, GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            pos = EditorGUILayout.BeginHorizontal();
                            GUI.color = separatorsCol;
                            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);

                        }

                            //--------------------------------------------Filter Textures--------------------------------------------
                            GUILayout.Space(5);
                        string txFilterState = showTxFilterOptions ? "-" : "+";
                        Rect txFilterBanner = EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Filter Textures", textLayout))
                        {
                            showTxFilterOptions = showTxFilterOptions ? false : true;
                        }
                        GUI.Label(new Rect(txFilterBanner.x + 5, txFilterBanner.y, 20, 20), txFilterState);
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        if (showTxFilterOptions)
                        {
                            GUILayout.BeginVertical(GUI.skin.window, GUILayout.MinHeight(115));
                            _TargetTexScroll = GUILayout.BeginScrollView(_TargetTexScroll, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.MinHeight(115));
                            GUILayout.BeginHorizontal(GUILayout.Width(0));

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
                                            foreach (Texture2D tx in t._ForestGroup[_GroupSelectIndex]._TargetTextures)
                                            {
                                                if (splatTx.diffuseTexture == tx)
                                                {
                                                    exists = true;
                                                }
                                            }
                                            if (!exists) t._ForestGroup[_GroupSelectIndex]._TargetTextures.Add(splatTx.diffuseTexture);
                                        }
                                        _FilterSelectionMode = false;
                                        Selection.activeGameObject = t.gameObject;
                                        ActiveEditorTracker.sharedTracker.isLocked = false;
                                    }
                                }
                            }
                            for (int i = 0; i < t._ForestGroup[_GroupSelectIndex]._TargetTextures.Count; i++)
                            {
                                EditorGUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(40), GUILayout.Height(95));
                                //Rect labelRect = new Rect(rect.x + 5f, rect.y + 5, 85, 85);
                                GUILayout.Space(5);
                                Texture2D img = t._ForestGroup[_GroupSelectIndex]._TargetTextures[i];
                                t._ForestGroup[_GroupSelectIndex]._TargetTextures[i] = EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;
                                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Bin.png"), deleteStyle, GUILayout.Height(20)))
                                {
                                    t._ForestGroup[_GroupSelectIndex]._TargetTextures.RemoveAt(i);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            if (GUILayout.Button("+", GUILayout.Width(65), GUILayout.Height(70)))
                            {
                                t._ForestGroup[_GroupSelectIndex]._TargetTextures.Add(null);
                                SetSceneDirty();
                            }

                            GUILayout.EndHorizontal();
                            GUILayout.EndScrollView();

                            GUILayout.BeginHorizontal();

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
                                Selection.activeGameObject = t.gameObject;
                                ActiveEditorTracker.sharedTracker.isLocked = _FilterSelectionMode; // Lock active inspector window
                                SetSceneDirty();
                            }

                            if (GUILayout.Button("Copy to all groups",normalStyle))
                            {
                                Texture2D[] targetTxs = t._ForestGroup[_GroupSelectIndex]._TargetTextures.ToArray();
                                foreach (ForestGroup fg in t._ForestGroup)
                                {
                                    if (fg._GroupName != t._ForestGroup[_GroupSelectIndex]._GroupName)
                                    {
                                        fg._TargetTextures.Clear();
                                        foreach (Texture2D tx in targetTxs)
                                        {
                                            fg._TargetTextures.Add(tx);
                                        }
                                    }
                                }
                                SetSceneDirty();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            GUILayout.Space(5);

                            pos = EditorGUILayout.BeginHorizontal();
                            GUI.color = separatorsCol;
                            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);
                        }

                        //--------------------------------------------Stencil Mask--------------------------------------------
                        string stencilMaskState = showStencilMaskOptions ? "-" : "+";
                        Rect stencilMaskBanner = EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Stencil Masks", textLayout))
                        {
                            showStencilMaskOptions = showStencilMaskOptions ? false : true;
                        }
                        GUI.Label(new Rect(stencilMaskBanner.x + 5, stencilMaskBanner.y, 20, 20), stencilMaskState);
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(5);
                        if (showStencilMaskOptions)
                        {
                            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(115));
                            _StencilMaskScroll = GUILayout.BeginScrollView(_StencilMaskScroll, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(115));
                            GUILayout.BeginHorizontal(GUILayout.Width(0));

                            for (int i = 0; i < t._StencilMasks.Count; i++)
                            {
                                EditorGUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(40), GUILayout.Height(95));
                                GUILayout.Space(5);
                                Texture2D img = t._StencilMasks[i]._StencilTex as Texture2D;
                                t._StencilMasks[i]._StencilTex = EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;
                                if (img != null)
                                {
                                    try
                                    {
                                        img.GetPixels32();
                                    }
                                    catch (UnityException)
                                    {
                                        EditorUtility.DisplayDialog("Texture not marked readable", "The texture '" + img + "' is not marked readable, please mark this as readable or choose another texture to use", "Ok");
                                        t._StencilMasks[i]._StencilTex = null;
                                        img = null;
                                    }
                                }
                                if (t._StencilMasks[i]._StencilObject == null)
                                {
                                    DestroyImmediate(t._StencilMasks[i]._StencilObject);
                                    t._StencilMasks.RemoveAt(i);
                                    for (int g = 0; g < t._ForestGroup.Count; g++)
                                    {
                                        t._ForestGroup[g]._StencilMaskState.RemoveAt(i);
                                    }
                                    i = -1;
                                }
                                else
                                {
                                    t._StencilMasks[i]._StencilObject.GetComponent<StencilMesh>()._Tex = img;
                                    if (img != null && t._StencilMasks[i]._StencilObject.GetComponent<MeshRenderer>().sharedMaterial != null)
                                    {
                                        t._StencilMasks[i]._StencilObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = img;
                                    }
                                    GUILayout.BeginHorizontal();
                                    string visibleIcnPath = t._ForestGroup[_GroupSelectIndex]._StencilMaskState[i] ? "/InternalResources/Sprites/Fr_Visible_Black.png" : "/InternalResources/Sprites/Fr_NotVisible.png";
                                    GUIStyle styleState = t._ForestGroup[_GroupSelectIndex]._StencilMaskState[i] ? activeStyle : notActiveStyle;
                                    visibleIcnPath = EditorGUIUtility.isProSkin && !t._ForestGroup[_GroupSelectIndex]._StencilMaskState[i] ? "/InternalResources/Sprites/Fr_NotVisible_White.png" : visibleIcnPath;
                                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + visibleIcnPath), styleState, GUILayout.Height(20), GUILayout.Width(30)))
                                    {
                                        t._ForestGroup[_GroupSelectIndex]._StencilMaskState[i] = t._ForestGroup[_GroupSelectIndex]._StencilMaskState[i] ? false : true;
                                    }
                                    if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Bin.png"), deleteStyle, GUILayout.Height(20), GUILayout.Width(30)))
                                    {
                                        DestroyImmediate(t._StencilMasks[i]._StencilObject);
                                        t._StencilMasks.RemoveAt(i);
                                        for (int g = 0; g < t._ForestGroup.Count; g++)
                                        {
                                            t._ForestGroup[g]._StencilMaskState.RemoveAt(i);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            if (GUILayout.Button("+", GUILayout.Width(65), GUILayout.Height(70)))
                            {
                                t._StencilMasks.Add(new StencilMask());
                                //Create new stencil mesh here
                                GameObject newStencil = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(_AssetPath + "/InternalResources/Prefabs/StencilMask.prefab"), t.gameObject.transform) as GameObject;
                                newStencil.transform.position = t._Center;
                                while(newStencil == null)
                                {
                                    return;
                                }
                                int maskIndex = t._StencilMasks.Count - 1;
                                newStencil.name = "StencilMask" + (maskIndex).ToString();
                                t._StencilMasks[maskIndex]._StencilObject = newStencil;
                                for (int g = 0; g < t._ForestGroup.Count; g++)
                                {
                                    t._ForestGroup[g]._StencilMaskState.Add(true);
                                }
                                SetSceneDirty();
                            }

                            GUILayout.EndHorizontal();
                            GUILayout.EndScrollView();
                            GUILayout.BeginHorizontal();

                            GUIStyle ignoreStencilAreaState = t._ForestGroup[_GroupSelectIndex]._StencilExcludeState ? activeStyle : notActiveStyle;
                            if (GUILayout.Button("Exclude outside of stencil", ignoreStencilAreaState))
                            {
                                t._ForestGroup[_GroupSelectIndex]._StencilExcludeState = t._ForestGroup[_GroupSelectIndex]._StencilExcludeState ? false : true;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            GUILayout.Space(5);

                            pos = EditorGUILayout.BeginHorizontal();
                            GUI.color = separatorsCol;
                            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);
                        }


                            //--------------------------------------------Terrain Stamping--------------------------------------------
                            string stampState = showStampOptions ? "-" : "+";
                        Rect stampBanner = EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Terrain Stamping (Experimental)", textLayout))
                        {
                            showStampOptions = showStampOptions ? false : true;
                        }
                        GUI.Label(new Rect(stampBanner.x + 5, stampBanner.y, 20, 20), stampState);
                        EditorGUILayout.EndHorizontal();

                        if (showStampOptions)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            GUILayout.Label("Terrain Tx", GUILayout.MaxWidth(70));
                            GUILayout.Space(10);
                            GUILayout.Label("Stamp", GUILayout.MaxWidth(50));
                            GUILayout.Space(5);
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowStamping = GUILayout.Toggle(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowStamping, "Activate");
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            Texture2D img1 = t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TerrainTex;
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TerrainTex = EditorGUILayout.ObjectField(img1, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                            GUILayout.Space(5);
                            Texture2D img2 = t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampTex;
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampTex = EditorGUILayout.ObjectField(img2, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                            GUILayout.BeginVertical();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Tile Size:", GUILayout.MaxWidth(55));
                            GUILayout.Space(5);
                            GUILayout.Label("X", GUILayout.MaxWidth(15));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TileSize.x = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TileSize.x, floatLayout, GUILayout.Width(10));
                            GUILayout.Space(35);
                            GUILayout.Label("Y", GUILayout.MaxWidth(15));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TileSize.y = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._TileSize.y, floatLayout, GUILayout.Width(10));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Stamp Size:", GUILayout.Width(79));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampSize = Mathf.RoundToInt(EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampSize, floatLayout));
                            GUILayout.EndHorizontal();


                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Stamp Opacity:", GUILayout.MaxWidth(90));
                            EditorGUILayout.LabelField("0", GUILayout.Width(10));
                            GUILayout.Space(5);
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampOpacity = Mathf.RoundToInt(GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampOpacity, 0, 100, GUILayout.Width(30)));
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("100", GUILayout.Width(25));
                            GUILayout.Space(5);
                            EditorGUIUtility.labelWidth = 20;
                            EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampOpacity.ToString("0"), floatLayout);
                            GUILayout.EndHorizontal();

                            //t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowStampOverlap = GUILayout.Toggle(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowStampOverlap, "Allow Overlapping");
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                        GUILayout.Space(5);
                            pos = EditorGUILayout.BeginHorizontal();
                            GUI.color = separatorsCol;
                            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);

                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.Label("Height", GUILayout.MaxWidth(50));
                            GUILayout.Space(7);
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowHeight = GUILayout.Toggle(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._AllowHeight, "Activate");
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            Texture2D img3 = t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampTexHeight;
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampTexHeight = EditorGUILayout.ObjectField(img3, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;

                            GUILayout.BeginVertical();
                            GUILayout.Space(10);
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Stamp Size:", GUILayout.Width(139));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampSizeHeight = Mathf.RoundToInt(EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._StampSizeHeight, floatLayout));
                            GUILayout.EndHorizontal();


                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Stamp Strength:", GUILayout.Width(100));
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Min:", GUILayout.Width(30));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._MinHeightStrength = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._MinHeightStrength, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Max:", GUILayout.Width(30));
                            t._ForestGroup[_GroupSelectIndex]._TerrainStamping._MaxHeightStrength = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._TerrainStamping._MaxHeightStrength, floatLayout, GUILayout.Width(40));
                            GUILayout.Space(5);
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.Space(5);
                    pos = EditorGUILayout.BeginHorizontal();
                    GUI.color = separatorsCol;
                    GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);

                    GUIStyle editForestStyle = new GUIStyle(GUI.skin.button);
                    if (_EditForestMode) editForestStyle.normal.background = _ActiveButton;
                    if (!_EditForestMode) editForestStyle.normal.background = _NormalButton;

                    GUIStyle paintModeStyle = new GUIStyle(GUI.skin.button);
                    if (_PaintMode)
                    {
                        paintModeStyle.normal.background = _ActiveButton;
                        paintModeStyle.normal.textColor = _ButtonFontColour;
                    }
                    if (!_PaintMode) paintModeStyle.normal.background = _NormalButton;

                    if (_PaintMode)
                    {
                        EditorGUILayout.LabelField("Hold shift to paint the object below / Hold Ctrl to delete", textLayout);
                        GUILayout.Space(5);
                    }

                    if (GUILayout.Button("Paint Objects", paintModeStyle))
                    {
                        _PaintMode = _PaintMode ? false : true;
                    }

                    if (_PaintMode)
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Frequency -", GUILayout.Width(75));
                        t._ForestGroup[_GroupSelectIndex]._MinPaintFreq = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MinPaintFreq, floatLayout, GUILayout.Width(40));
                        GUILayout.Space(5);
                        t._ForestGroup[_GroupSelectIndex]._PaintFreq = GUILayout.HorizontalSlider(t._ForestGroup[_GroupSelectIndex]._PaintFreq, t._ForestGroup[_GroupSelectIndex]._MinPaintFreq, t._ForestGroup[_GroupSelectIndex]._MaxPaintFreq, GUILayout.MaxWidth(120));
                        t._ForestGroup[_GroupSelectIndex]._MaxPaintFreq = EditorGUILayout.FloatField(t._ForestGroup[_GroupSelectIndex]._MaxPaintFreq, floatLayout, GUILayout.Width(40));
                        GUILayout.Space(5);

                        EditorGUILayout.LabelField("Val:", GUILayout.Width(25));
                        EditorGUILayout.LabelField(t._ForestGroup[_GroupSelectIndex]._PaintFreq.ToString("0.00"), floatLayout);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        //Having to put this here to stop error when a preset is changed with more groups seems to be reoccuring
                        if (t._ForestPreset == null || t._ForestPreset.name != t._ForestPresetName)
                        {
                            _GroupSelectIndex = 0;
                            _ByGroup = false;
                        }

                        int objectCount = t._ForestPreset._ForestGroup[_GroupSelectIndex]._FoliageObjects.Count;
                        string[] foliageObjects = new string[objectCount];
                        for (int i = 0; i < objectCount; i++)
                        {
                            foliageObjects[i] = t._ForestPreset._ForestGroup[_GroupSelectIndex]._FoliageObjects[i]._FoliageObject.name;
                        }
                        _SelectedFoliageObject = EditorGUILayout.Popup(_SelectedFoliageObject, foliageObjects, popupStyle);
                    }

                    GUILayout.Space(5);
                    pos = EditorGUILayout.BeginHorizontal();
                    GUI.color = separatorsCol;
                    GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Freeze Selected"))
                    {
                        GameObject[] foliageObjects = Selection.gameObjects;
                        for (int i = 0; i < foliageObjects.Length; i++)
                        {
                            bool exists = false;
                            if (t._FrozenObjects.Count > 0)
                            {
                                for (int e = 0; e < t._FrozenObjects.Count; e++)
                                {
                                    if (t._FrozenObjects[e]._FoliageObject == foliageObjects[i])
                                        exists = true;
                                    e = t._FrozenObjects.Count;
                                }
                            }

                            bool isFoliageObj = false;
                            for (int f = 0; f < t._ForestGroup.Count; f++)
                            {
                                if (foliageObjects[i].tag == t._ForestGroup[f]._GroupName)
                                {
                                    isFoliageObj = true;
                                }
                            }

                            if (isFoliageObj && !exists)
                            {
                                t._FrozenObjects.Add(new FrozenObjects());
                                if (foliageObjects[i].transform.parent.tag == "Foliage")
                                {
                                    t._FrozenObjects[t._FrozenObjects.Count - 1]._Parent = foliageObjects[i].transform.parent.transform.gameObject;
                                }
                                t._FrozenObjects[t._FrozenObjects.Count - 1]._FoliageObject = foliageObjects[i];
                                t._FrozenObjects[t._FrozenObjects.Count - 1]._Materials = new Material[foliageObjects[i].GetComponent<Renderer>().sharedMaterials.Length];
                                t._FrozenObjects[t._FrozenObjects.Count - 1]._Materials = foliageObjects[i].GetComponent<Renderer>().sharedMaterials;

                                t._FrozenObjects[t._FrozenObjects.Count - 1]._Index = i; ;
                                Material[] mat = new Material[foliageObjects[i].GetComponent<Renderer>().sharedMaterials.Length];
                                for (int n = 0; n < mat.Length; n++)
                                {
                                    mat[n] = AssetDatabase.LoadAssetAtPath<Material>(_AssetPath + "/InternalResources/Materials/Frozen.mat");
                                }
                                foliageObjects[i].GetComponent<Renderer>().sharedMaterials = mat;
                            }
                        }
                    }
                    if (GUILayout.Button("Clear Frozen"))
                    {
                        ClearFrozen();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(5);
            pos = EditorGUILayout.BeginHorizontal();
            GUI.color = separatorsCol;
            GUI.Box(new Rect(pos.x, pos.y, Screen.width - 18, Mathf.Clamp(5 * div, 5, 5)), "",GUI.skin.button);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            //DrawDefaultInspector(); // For Debug
        }


        void AddTag(string name)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Adding a Tag
            string s = name;

            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(s)) { found = true; break; }
            }

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = s;
            }

            tagManager.ApplyModifiedProperties();
        }

        void Insert()
        {
            GameObject selected = Selection.activeGameObject;
            int selectedID = selected.GetComponent<PointNode>().id;
            GameObject newPoint = Instantiate(t._Point, new Vector3(selected.transform.position.x + 1, selected.transform.position.y, selected.transform.position.z + 1), selected.transform.rotation) as GameObject;
            t._Points.Insert(selectedID + 1, newPoint);

            newPoint.transform.SetParent(t.transform);

            if (t._Points.Count > 0)
            {
                newPoint.transform.localScale = t._Points[0].transform.localScale;
            }
            else
            {
                newPoint.transform.localScale = new Vector3(0.2999998f, 0.3f, 0.2999998f);
            }

            for (int i = 0; i <= t._Points.Count - 1; i++)
            {
                t._Points[i].GetComponent<PointNode>().id = i;
                t._Points[i].gameObject.name = ("Node" + i.ToString());
                t._Points[i].transform.SetSiblingIndex(i);
            }

            newPoint.GetComponent<PointNode>().forester = t;
            t.Deforestation(_ByGroup, _GroupSelectIndex,true);
            SetSceneDirty();
        }

        void ClearFrozen()
        {
            t = target as ForesterTool;
            for (int i = 0; i < t._FrozenObjects.Count; i++)
            {
                t._FrozenObjects[i]._FoliageObject.GetComponent<Renderer>().sharedMaterials = t._FrozenObjects[i]._Materials;
            }
            t._FrozenObjects.Clear();
        }

        void CreatePoint(GameObject point, Vector3 pos)
        {
            GameObject newPoint = Instantiate(point, pos, t.transform.rotation) as GameObject;
            t._Points.Add(newPoint);

            newPoint.transform.SetParent(t.transform);
            if (t._Points.Count > 0)
            {
                newPoint.transform.localScale = t._Points[0].transform.localScale;
            }
            else
            {
                newPoint.transform.localScale = new Vector3(0.2999998f, 0.3f, 0.2999998f);
            }
            newPoint.name = ("Node" + (t._Points.Count - 1).ToString());
            newPoint.GetComponent<PointNode>().id = t._Points.Count - 1;
            newPoint.GetComponent<PointNode>().forester = t;
        }

        void RemoveSelectedPoint()
        {
            GameObject point = Selection.activeGameObject;
            int pointID = point.GetComponent<PointNode>().id;
            t._Points.RemoveAt(pointID);
            DestroyImmediate(point);
            for (int i = pointID; i <= t._Points.Count - 1; i++)
            {
                t._Points[i].GetComponent<PointNode>().id--;
                t._Points[i].name = "Node" + t._Points[i].GetComponent<PointNode>().id;
            }
            SetSceneDirty();
        }

        void ClearAllPoints()
        {
            foreach (GameObject node in t._Points)
            {
                DestroyImmediate(node);
            }
            t._Points.Clear();
            SetSceneDirty();
        }

        public void EmailMe()
        {
            string email = "markypocock@gmail.com";
            string subject = MyEscapeURL("FORESTER FEEDBACK/FORESTER SUGGESTION/FORESTER ISSUE");
            string body = MyEscapeURL("Please Enter your message here\n\n\n\n" +
            "____" +
            "\n\nPlease Do Not Modify This\n\n" +
            "Unity Version: " + Application.unityVersion + "\n\n" +
            "OS: " + SystemInfo.operatingSystem + "\n\n" +
            "____");
            //Open the Default Mail App
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        string MyEscapeURL(string url)
        {
            return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }

        void SetSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
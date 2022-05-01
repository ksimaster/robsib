using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.UI;

namespace Forester
{
    public class AreaEditor : EditorWindow
    {
        public string _PresetName;
        public AreaPreset _AreaPreset;
        List<Rect> _Windows = new List<Rect>();
        List<int> _WindowsToAttach = new List<int>();
        List<int> _AttachedWindows = new List<int>();
        static Texture _NodeTexture;
        static Texture _NodeTexSelected;
        static Texture _Background;
        static AreaEditor _Editor;
        static string assetPath;
        private float _Selected = -1;
        private bool _CreatingNode = false;
        private bool _CloseEnd = true;

        Texture2D _NormalButton;
        Texture2D _ActiveButton;

        private string _CurrentPreset;

        [MenuItem("Tools/Forester/Tools/Area Editor")]

        static void Init()
        {
            _Editor = GetWindow<AreaEditor>("Area Editor");

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
            GUIContent titleContent = new GUIContent("Area Editor", AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/Fr_WindowIcon.png"));
            _Editor.titleContent = titleContent;
            _Editor.maxSize = new Vector2(590.0f, 705.0f);
            _Editor.minSize = new Vector2(590.0f, 705.0f);

        }

        public void StartEditor(AreaPreset areaPreset)
        {
            _AreaPreset = areaPreset;
            Init();
        }

        void Update()
        {
            for (int i = 0; i < _Windows.Count; i++)
            {
                if (_Windows[i].x > 570)
                {
                    _Windows[i] = new Rect(new Vector2(560, _Windows[i].y), _Windows[i].size);
                }
                if (_Windows[i].x < 0)
                {
                    _Windows[i] = new Rect(new Vector2(0, _Windows[i].y), _Windows[i].size);
                }
                if (_Windows[i].y > 570)
                {
                    _Windows[i] = new Rect(new Vector2(_Windows[i].x, 560), _Windows[i].size);
                }
                if (_Windows[i].y < 0)
                {
                    _Windows[i] = new Rect(new Vector2(_Windows[i].x, 0), _Windows[i].size);
                }
            }

            if (_AreaPreset != null)
            {
                if (_CurrentPreset != _AreaPreset.name)
                {
                    LoadAsset();
                }
            }

            if (_AreaPreset == null)
            {
                _CurrentPreset = "null";
            }

        }

        void OnGUI()
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

            if (assetPath == null)
            {
                return;
            }

            _NodeTexture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/AreaEditor_Node.png") as Texture;
            _Background = AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/AreaEditor_Bg.png") as Texture;
            _NodeTexSelected = AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/AreaEditor_Node_Sel.png") as Texture;

            //_ButtonFontColour = Color.black;
            _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
            _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
            }

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = _NormalButton;

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            var textFloatingLayout = new GUIStyle();
            textFloatingLayout.alignment = TextAnchor.MiddleCenter;

            var floatLayout = new GUIStyle(GUI.skin.textArea);
            floatLayout.alignment = TextAnchor.MiddleCenter;
            floatLayout.stretchWidth = true;
            floatLayout.fixedWidth = 40;

            //GUI.DrawTexture(new Rect(new Vector2(10, 10), new Vector2(570, 110)), AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Forester_AreaEditorBanner.png"), ScaleMode.StretchToFill, false);
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Forester_AreaEditorBanner.png"));
            GUILayout.BeginArea((new Rect(new Vector2(10, 70), new Vector2(570, 90))));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.BeginHorizontal();
            if (_WindowsToAttach.Count == 2)
            {
                _AttachedWindows.Add(_WindowsToAttach[0]);
                _AttachedWindows.Add(_WindowsToAttach[1]);
                _WindowsToAttach = new List<int>();
            }

            if (_AttachedWindows.Count >= 2)
            {
                for (int i = 0; i < _AttachedWindows.Count; i += 2)
                {
                    DrawNodeCurve(_Windows[_AttachedWindows[i]], _Windows[_AttachedWindows[i + 1]]);
                }
            }

            SerializedObject serialized = new SerializedObject(this);
            SerializedProperty areaPreset = serialized.FindProperty("_AreaPreset");
            serialized.Update();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.ObjectField(areaPreset, typeof(Object));
            serialized.ApplyModifiedProperties();

            if (GUILayout.Button("New", buttonStyle))
            {
                NewPreset();
            }


            if (GUILayout.Button("Save", buttonStyle))
            {
                SaveAsset();
            }

            GUILayout.Box("", GUILayout.Height(17), GUILayout.Width(5));

            GUIStyle closeEndStyle = new GUIStyle(GUI.skin.button);
            if (_CloseEnd) closeEndStyle.normal.background = _ActiveButton;
            if (!_CloseEnd) closeEndStyle.normal.background = _NormalButton;

            if (GUILayout.Button("Close End",closeEndStyle))
            {
                if(_CloseEnd)
                {
                    _CloseEnd = false;
                }
                else
                {
                    _CloseEnd = true;
                }
            }

            if (Event.current.button == 0 && Event.current.isMouse)
            {
                if (Event.current.mousePosition.x < 570 && Event.current.mousePosition.x > 0 && Event.current.mousePosition.y - 65 < 570 && Event.current.mousePosition.y - 65 > 0)
                {
                    List<float> distances = new List<float>();
                    foreach (Rect window in _Windows)
                    {
                        distances.Add(Vector2.Distance(new Vector2(window.x, window.y), new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y - 65)));
                    }

                    float nearest = 9999999;
                    float nearestID = -1;

                    for (int i = 0; i < distances.Count; i++)
                    {
                        if (distances[i] < nearest && distances[i] < 10)
                        {
                            nearestID = i;
                            nearest = distances[i];
                        }
                    }
                    _Selected = nearestID;
                }
            }

            if (Event.current.button == 1)
            {
                if (!_CreatingNode)
                {
                    NewNode(Event.current.mousePosition);
                    _CreatingNode = true;
                }
            }
            else
            {
                _CreatingNode = false;
            }

            if (GUILayout.Button("Insert", buttonStyle))
            {
                NewNode(new Vector2(_Windows[(int)_Selected].x + 10, _Windows[(int)_Selected].y + 10), true, (int)_Selected + 1);
            }

            if (GUILayout.Button("Remove", buttonStyle))
            {
                if (_Selected != -1)
                {
                    RemoveNode((int)_Selected);
                }
            }

            if (GUILayout.Button("Clear All", buttonStyle))
            {
                _Windows.Clear();
            }

            GUILayout.EndHorizontal();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            if (_AreaPreset == null && _PresetName == null || _AreaPreset == null && _PresetName == "") _PresetName = "<Insert Preset Name>";
            _PresetName = EditorGUILayout.TextField(_PresetName, textLayout); 
            GUILayout.EndArea();

            if (_AreaPreset != null)
            {
                GUILayout.BeginArea(new Rect(new Vector2(10, 130), new Vector2(570, 570)));
                GUI.DrawTexture(new Rect(new Vector2(0, 0), new Vector2(570, 570)), _Background, ScaleMode.StretchToFill, false);
                BeginWindows();

                for (int i = 0; i < _Windows.Count; i++)
                {

                    GUIStyle imageStyle = new GUIStyle(GUI.skin.window);
                    imageStyle.imagePosition = ImagePosition.ImageOnly;
                    if (i == _Selected)
                    {
                        _Windows[i] = GUI.Window(i, _Windows[i], DrawNodeWindow, _NodeTexSelected, GUIStyle.none);
                    }
                    else
                    {
                        _Windows[i] = GUI.Window(i, _Windows[i], DrawNodeWindow, _NodeTexture, GUIStyle.none);
                    }

                    if (i < _Windows.Count && _Windows.Count > 1 && i > 0)
                    {
                        Handles.DrawLine(new Vector3(_Windows[i].x + 2, _Windows[i].y + 2, 0), new Vector3(_Windows[i - 1].x + 2, _Windows[i - 1].y + 2, 0));
                    }
                }

                if (_CloseEnd)
                {
                    if (_Windows.Count > 1)
                    {
                        Handles.DrawLine(new Vector3(_Windows[_Windows.Count - 1].x + 2, _Windows[_Windows.Count - 1].y + 2, 0), new Vector3(_Windows[0].x + 2, _Windows[0].y + 2, 0));
                    }
                }

                EndWindows();
                GUILayout.EndArea();
            }
        }

        void RemoveNode(int id)
        {
            _Windows.RemoveAt(id);
        }

        void NewNode(Vector2 coords, bool insertAt = false, int id = -1)
        {
            if (!insertAt)
            {
                if (coords == Vector2.zero)
                {
                    if (_Windows.Count == 0)
                    {
                        _Windows.Add(new Rect(570 / 2, 570 / 2, 10, 10));
                    }
                    else
                    {
                        _Windows.Add(new Rect(_Windows[_Windows.Count - 1].x + 10, _Windows[_Windows.Count - 1].y + 10, 10, 10));
                    }
                }

                if (coords != Vector2.zero)
                {
                    _Windows.Add(new Rect(coords.x, coords.y - 65, 10, 10));
                }
            }
            else
            {
                _Windows.Insert(id, new Rect(coords.x, coords.y, 10, 10));
            }
        }

        void DrawNodeWindow(int id)
        {
            /*
            if (GUILayout.Button("Attach"))
            {
                _WindowsToAttach.Add(id);
            }
            */
            GUI.DragWindow();
        }

        void NewPreset()
        {
            AreaPreset newPreset = CreateInstance<AreaPreset>();
            if (!System.IO.Directory.Exists(assetPath + "/Presets/AreaPresets"))
            {
                System.IO.Directory.CreateDirectory(assetPath + "/Presets/AreaPresets");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            string name = "NewAreaPreset";
            AssetDatabase.CreateAsset(newPreset, assetPath + "/Presets/AreaPresets/" + name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _AreaPreset = AssetDatabase.LoadAssetAtPath<AreaPreset>(assetPath + "/Presets/AreaPresets/" + name + ".asset") as AreaPreset;
            LoadAsset();
        }
        
        void LoadAsset()
        {
            _Windows.Clear();
            for (int i = 0; i < _AreaPreset._Windows.Count; i++)
            {
                _Windows.Add(_AreaPreset._Windows[i]);
            }
            _CloseEnd = _AreaPreset._CloseEnd;
            _CurrentPreset = _AreaPreset.name;
            _PresetName = _AreaPreset.name;
        }

        void SaveAsset()
        {
            _AreaPreset._Windows.Clear();
            _AreaPreset._Positions.Clear();
            for(int i = 0;i< _Windows.Count;i++)
            {
                _AreaPreset._Windows.Add(_Windows[i]);
                //Convert to vector3
                _AreaPreset._Positions.Add(new Vector3(_Windows[i].x, 0, _Windows[i].y) / 10);

            }

            _AreaPreset._CloseEnd = _CloseEnd;

            int id = _AreaPreset.GetInstanceID();
            string path = AssetDatabase.GetAssetPath(id);
            AssetDatabase.RenameAsset(path, _PresetName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(_AreaPreset);
        }
        
        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {// Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }
    }
}

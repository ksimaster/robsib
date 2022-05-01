using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Forester
{
    public class SatelliteObjectEditor : EditorWindow
    {
        string assetPath;
        public ForesterPresetEditor _ForestPresetEditor;
        public int _ForestGroup = -1;
        public int _FoliageObject = -1;
        static SatelliteObjectEditor _Window;
        Vector2 scrollPosition1 = Vector2.zero;
        public List<SatelliteObjects> _SatelliteObjects = new List<SatelliteObjects>();
        GUIStyle[] _SelectionStyle;
        int _Selection = -1;


        Texture2D _NormalButton;
        Texture2D _ActiveButton;

        static void Init()
        {
            _Window = GetWindow<SatelliteObjectEditor>("Satellite Objects");

            if (_Window.assetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        _Window.assetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (_Window.assetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                _Window.assetPath = dir;
                            }
                        }
                    }
                }
            }

            GUIContent titleContent = new GUIContent("Satellite Objects", AssetDatabase.LoadAssetAtPath<Texture2D>(_Window.assetPath + "/InternalResources/Sprites/Fr_WindowIcon.png"));
            _Window.titleContent = titleContent;
            _Window.Show();
            _Window.minSize = new Vector2(260, 250);
            _Window.maxSize = new Vector2(260, 250);
        }

        public void StartEditor(ForesterPresetEditor forestPresetEditor, int forestGroup, int foliageObject)
        {
            Init();
            _ForestPresetEditor = forestPresetEditor;
            _ForestGroup = forestGroup;
            _FoliageObject = foliageObject;
            Load();
        }

        void OnGUI()
        {

            //_ButtonFontColour = Color.black;
            _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
            _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
            }

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Box("Satellite Objects", textLayout);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Width(230), GUILayout.Height(152));
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.Width(230), GUILayout.Height(152));
            _SelectionStyle = new GUIStyle[_SatelliteObjects.Count];

            for (int i = 0; i < _SatelliteObjects.Count; i++)
            {
                GUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Width(205), GUILayout.Height(25));

                if (_Selection == i)
                {
                    _SelectionStyle[i] = new GUIStyle(GUI.skin.button);
                    _SelectionStyle[i].normal.background = _ActiveButton;
                }
                else
                {
                    _SelectionStyle[i] = new GUIStyle(GUI.skin.button);
                    _SelectionStyle[i].normal.background = _NormalButton;
                }

                if (GUILayout.Button(">", _SelectionStyle[i], GUILayout.Width(17)))
                {
                    _Selection = i;
                }
                _SatelliteObjects[i]._Object = EditorGUILayout.ObjectField(_SatelliteObjects[i]._Object, typeof(GameObject), false, GUILayout.Width(150), GUILayout.Height(17)) as GameObject;
                if (GUILayout.Button("X"))
                {
                    _SatelliteObjects.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            //Add New
            if (GUILayout.Button("Add New", GUILayout.Width(205), GUILayout.Height(25)))
            {
                _SatelliteObjects.Add(new SatelliteObjects());
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (_Selection > _SatelliteObjects.Count)
            {
                _Selection = -1;
            }

            if (_Selection >= 0 && _Selection < _SatelliteObjects.Count && _SatelliteObjects.Count > 0)
            {
                _Window.minSize = new Vector2(260, 355);
                _Window.maxSize = new Vector2(260, 355);

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Distance - ", GUILayout.Width(100));
                _SatelliteObjects[_Selection]._SatelliteSpread = EditorGUILayout.FloatField(_SatelliteObjects[_Selection]._SatelliteSpread);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Max Spawns - ", GUILayout.Width(100));
                _SatelliteObjects[_Selection]._MaxNum = EditorGUILayout.IntField(_SatelliteObjects[_Selection]._MaxNum);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _SatelliteObjects[_Selection]._Hover = GUILayout.Toggle(_SatelliteObjects[_Selection]._Hover, "Hover (Y Position) - ");
                if (_SatelliteObjects[_Selection]._Hover) _SatelliteObjects[_Selection]._YPos = EditorGUILayout.FloatField(_SatelliteObjects[_Selection]._YPos);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _SatelliteObjects[_Selection]._Scale = GUILayout.Toggle(_SatelliteObjects[_Selection]._Scale, "Scale - ",GUILayout.Width(60));
                if (_SatelliteObjects[_Selection]._Scale)
                {
                    EditorGUIUtility.labelWidth = 30;
                    _SatelliteObjects[_Selection]._MinScale = EditorGUILayout.FloatField("Min: ", _SatelliteObjects[_Selection]._MinScale);
                    _SatelliteObjects[_Selection]._MaxScale = EditorGUILayout.FloatField("Max: ", _SatelliteObjects[_Selection]._MaxScale);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUIStyle _FaceNormalsStyle = new GUIStyle(GUI.skin.button);
                GUIStyle _RotationStyle = new GUIStyle(GUI.skin.button);

                if (_SatelliteObjects[_Selection]._FaceNormals == true)
                {
                    _FaceNormalsStyle.normal.background = _ActiveButton;
                }
                else
                {
                    _FaceNormalsStyle.normal.background = _NormalButton;
                }

                if (_SatelliteObjects[_Selection]._Rotation == true)
                {
                    _RotationStyle.normal.background = _ActiveButton;
                }
                else
                {
                    _RotationStyle.normal.background = _NormalButton;
                }

                if (GUILayout.Button("Face Normals", _FaceNormalsStyle, GUILayout.Width(124)))
                {
                    if (_SatelliteObjects[_Selection]._FaceNormals != true)
                    {
                        _SatelliteObjects[_Selection]._FaceNormals = true;
                    }
                    else
                    {
                        _SatelliteObjects[_Selection]._FaceNormals = false;
                    }
                }
                if (GUILayout.Button("Rotation", _RotationStyle, GUILayout.Width(124)))
                {
                    if (_SatelliteObjects[_Selection]._Rotation != true)
                    {
                        _SatelliteObjects[_Selection]._Rotation = true;
                    }
                    else
                    {
                        _SatelliteObjects[_Selection]._Rotation = false;
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                _Window.minSize = new Vector2(260, 250);
                _Window.maxSize = new Vector2(260, 250);
            }
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel", GUILayout.Width(124)))
            {
                _Window.Close();
            }
            if (GUILayout.Button("Ok", GUILayout.Width(124)))
            {
                _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects.Clear();
                for (int i = 0; i < _SatelliteObjects.Count; i++)
                {
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects.Add(new SatelliteObjects());
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._FaceNormals = _SatelliteObjects[i]._FaceNormals;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Rotation = _SatelliteObjects[i]._Rotation;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MaxNum = _SatelliteObjects[i]._MaxNum;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Object = _SatelliteObjects[i]._Object;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Hover = _SatelliteObjects[i]._Hover;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._YPos = _SatelliteObjects[i]._YPos;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._SatelliteSpread = _SatelliteObjects[i]._SatelliteSpread;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Scale = _SatelliteObjects[i]._Scale;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MaxScale = _SatelliteObjects[i]._MaxScale;
                    _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MinScale = _SatelliteObjects[i]._MinScale;
                }
                _Window.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void Load()
        {
            _SatelliteObjects.Clear();
            for (int i = 0; i < _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects.Count; i++)
            {
                _SatelliteObjects.Add(new SatelliteObjects());
                _SatelliteObjects[i]._FaceNormals = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._FaceNormals;
                _SatelliteObjects[i]._Rotation = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Rotation;
                _SatelliteObjects[i]._MaxNum = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MaxNum;
                _SatelliteObjects[i]._Object = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Object;
                _SatelliteObjects[i]._Hover = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Hover;
                _SatelliteObjects[i]._YPos = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._YPos;
                _SatelliteObjects[i]._SatelliteSpread = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._SatelliteSpread;
                _SatelliteObjects[i]._Scale = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._Scale;
                _SatelliteObjects[i]._MinScale = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MinScale;
                _SatelliteObjects[i]._MaxScale = _ForestPresetEditor._ForestGroup[_ForestGroup]._FoliageObjects[_FoliageObject]._SatelliteObjects[i]._MaxScale;
            }
        }
    }
}
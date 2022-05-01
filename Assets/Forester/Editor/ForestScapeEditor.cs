using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Forester
{
    [CustomEditor(typeof(ForestScape))]
    public class ForestScapeEditor : Editor
    {
        ForestScape t;
        AreaEditor _AreaEditor;
        ForesterPresetEditor _ForestPresetEditor;

        Texture2D _NormalButton;
        Texture2D _ActiveButton;

        Vector2 scrollPosition1 = Vector2.zero;
        Vector2 scrollPosition2 = Vector2.zero;
        Vector2 scrollPosition3 = Vector2.zero;
        Vector2 scrollPosition4 = Vector2.zero;
        Vector2 _TargetTexScroll = Vector2.zero;

        private bool _FilterSelectionMode = false;

        Color _ButtonFontColour;

        string _AssetPath;

        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;

        }

        void OnDisable()
        {

        }

        void OnScene(SceneView sceneView)
        {
            _ButtonFontColour = Color.black;

            t = target as ForestScape;

            if (t == null)
            {
                return;
            }

            //Check if stored preset hasn't changed
            if (t._ForestScapePreset == null)
            {
                t._CurrentPreset = "Null";
            }
            else
            {
                if(t._ForestScapePreset.name != t._CurrentPreset)
                {
                    LoadForestScapePreset();
                }
            }

            FindAssetPath();

            //Check if any gameobjects are null and preventing inspector from displaying
            for (int i = 0; i < t._ForestTools.Count; i++)
            {
                if (t._ForestTools[i] == null)
                {
                    t._ForestTools.RemoveAt(i);
                    i = 0;
                }
            }

            //t._ForestTools.Clear();
            if (t._ForestTools.Count > 0)
            {
                for (int a = 0; a < t._ForestTools.Count; a++)
                {
                    ForesterTool foresterTool = t._ForestTools[a].GetComponent<ForesterTool>();
                    // iterate over game objects added to the array...
                    if (foresterTool._Points.Count > 1)
                    {
                        for (int i = 0; i <= foresterTool._Points.Count - 1; i++)
                        {
                            // ... and draw a line between them
                            if (foresterTool._Points[i] != null)
                                if (i > 0 && i < foresterTool._Points.Count - 1 && foresterTool._ClosedEnd || i > 0 && i <= foresterTool._Points.Count && !foresterTool._ClosedEnd)
                                {
                                    Handles.DrawLine(foresterTool._Points[i - 1].transform.position, foresterTool._Points[i].transform.position);
                                }
                                else if (i == foresterTool._Points.Count - 1)
                                {
                                    Handles.DrawLine(foresterTool._Points[i - 1].transform.position, foresterTool._Points[i].transform.position);
                                    Handles.DrawLine(foresterTool._Points[i].transform.position, foresterTool._Points[0].transform.position);
                                }
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (t == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(t);

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
            }
            else
            {
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");
            }

            var popupStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
            popupStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle deleteStyle = new GUIStyle(GUI.skin.button);
            deleteStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Red.png");

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

            //Style for red buttons with no highlighting
            var redStyle = new GUIStyle(GUI.skin.button);
            redStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Red.png");

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            Rect pos = EditorGUILayout.BeginHorizontal();
            float div = (float)Screen.width / 512;
            GUI.DrawTexture(new Rect(pos.x, pos.y, Mathf.Clamp(512 * div, 128, 512), Mathf.Clamp(65 * div, 10, 65)), AssetDatabase.LoadAssetAtPath<Texture>(_AssetPath + "/InternalResources/Sprites/Forester_ForestScapeBanner.png"));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(Mathf.Clamp(65 * div, 10, 65));

            GUILayout.BeginHorizontal();
            GUILayout.Label("ForestScape Preset -", GUILayout.Width(125));
            EditorGUIUtility.labelWidth = 80;
            t._ForestScapePreset = EditorGUILayout.ObjectField(t._ForestScapePreset,typeof(ForestScapePreset), false, GUILayout.ExpandWidth(true)) as ForestScapePreset;
            if (t._ForestScapePreset == null && t._PresetName == null || t._ForestScapePreset == null && t._PresetName == "") t._PresetName = "<Insert Preset Name>";
            t._PresetName = GUILayout.TextField(t._PresetName, textLayout);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(400));
            scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, false, true, GUILayout.ExpandWidth(true), GUILayout.Height(400));

            EditorGUILayout.LabelField("Assign Areas", textLayout);

            GUILayout.Space(5);

            if (GUILayout.Button("Load Selected Area Presets",normalStyle))
            {
                Object[] selected = Selection.objects;
                if (selected.Length == 0) return;
                foreach (Object go in selected)
                {
                    string type = go.GetType().ToString();
                    if(type != "Forester.AreaPreset")
                    {
                        return;
                    }
                }
                LoadSelectedArea();
            }

            GUILayout.Space(5);

            GUILayout.BeginVertical(GUI.skin.window, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            for (int i = 0; i < t._AreaPresets.Count; i++)
            {
                //Panel
                GUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(20));

                int num = i + 1;
                GUILayout.Box(num.ToString(), normalStyle, GUILayout.Width(22), GUILayout.Height(15));

                t._AreaPresets[i]._AreaPreset = EditorGUILayout.ObjectField(t._AreaPresets[i]._AreaPreset, typeof(AreaPreset), false, GUILayout.Width(180), GUILayout.Height(16)) as AreaPreset;
                GUIStyle[] _AreaPresetStyles = new GUIStyle[t._AreaPresets.Count];
                _AreaPresetStyles[i] = new GUIStyle(GUI.skin.button);

                if (t._AreaPresets[i]._Include)
                {
                    _AreaPresetStyles[i].normal.background = _ActiveButton;
                }
                else
                {
                    _AreaPresetStyles[i].normal.background = _NormalButton;
                }

                if (GUILayout.Button("Include", _AreaPresetStyles[i], GUILayout.Height(15)))
                {
                    if(t._AreaPresets[i]._Include)
                    {
                        t._AreaPresets[i]._Include = false;
                    }
                    else
                    {
                        t._AreaPresets[i]._Include = true;
                    }
                }

                if (GUILayout.Button("Edit",normalStyle, GUILayout.Height(15)))
                {
                    _AreaEditor = CreateInstance<AreaEditor>();
                    _AreaEditor.StartEditor(t._AreaPresets[i]._AreaPreset);
                }

                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture>(_AssetPath + "/InternalResources/Sprites/Fr_Bin.png"), redStyle, GUILayout.Width(22), GUILayout.Height(15)))
                {
                    t._AreaPresets.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
                //End
            }

            if (GUILayout.Button("Add", GUILayout.Height(25)))
            {
                t._AreaPresets.Add(new AreaPresetType());
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Include All",normalStyle))
            {
                for (int i = 0; i < t._AreaPresets.Count; i++)
                {
                    t._AreaPresets[i]._Include = true;
                }
            }
            if(GUILayout.Button("Include None",normalStyle))
            {
                for (int i = 0; i < t._AreaPresets.Count; i++)
                {
                    t._AreaPresets[i]._Include = false;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Distribution and Randomisation", textLayout);
            GUILayout.Space(5);

            GUILayout.BeginVertical(GUI.skin.window, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, false, true, GUILayout.ExpandWidth(true), GUILayout.Height(100));

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Density - ", GUILayout.MaxWidth(80));
            t._MinFill = EditorGUILayout.FloatField(t._MinFill, floatLayout, GUILayout.Width(40));
            GUILayout.Space(5);
            t._Fill = GUILayout.HorizontalSlider(t._Fill, t._MinFill, t._MaxFill, GUILayout.Width(85));
            t._MaxFill = EditorGUILayout.FloatField(t._MaxFill, floatLayout, GUILayout.Width(40));
            GUILayout.Space(20);
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
            EditorGUILayout.LabelField(t._Fill.ToString("0.00"), floatLayout);

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Offset - ", GUILayout.MaxWidth(80));
            t._MinOffset = EditorGUILayout.FloatField(t._MinOffset, floatLayout, GUILayout.Width(40));
            GUILayout.Space(5);
            t._Offset = GUILayout.HorizontalSlider(t._Offset, t._MinOffset, t._MaxOffset, GUILayout.Width(85));
            t._MaxOffset = EditorGUILayout.FloatField(t._MaxOffset, floatLayout, GUILayout.Width(40));
            GUILayout.Space(20);
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.LabelField("Value:", GUILayout.Width(40));
            EditorGUILayout.LabelField(t._Offset.ToString("0.00"), floatLayout);

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotation - ", GUILayout.MaxWidth(80));
            GUILayout.Label("Min -", GUILayout.Width(30));
            t._MinRotation = EditorGUILayout.FloatField(t._MinRotation, floatLayout, GUILayout.Width(40));
            GUILayout.Space(5);
            GUILayout.Label("Max -", GUILayout.Width(35));
            t._MaxRotation = EditorGUILayout.FloatField(t._MaxRotation, floatLayout, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scale - ", GUILayout.MaxWidth(80));
            GUILayout.Label("Min -",GUILayout.Width(30));
            t._MinScale = EditorGUILayout.FloatField(t._MinScale, floatLayout, GUILayout.Width(40));
            GUILayout.Space(5);
            GUILayout.Label("Max -", GUILayout.Width(35));
            t._MaxScale = EditorGUILayout.FloatField(t._MaxScale, floatLayout, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Allow Overlapping of Foliage between Forest Tools - ", GUILayout.MaxWidth(300));
            t._OverlapFoliage = GUILayout.Toggle(t._OverlapFoliage,GUIContent.none);
            GUILayout.EndHorizontal();
            if (!t._OverlapFoliage)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Overlapping Distance Set by Forest Preset (See Overlap in Group Options)", GUILayout.MaxWidth(430));
                GUILayout.EndVertical();
            }
            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Assign Forest Types", textLayout);
            GUILayout.Space(5);
            if (GUILayout.Button("Load Selected Forest Presets",normalStyle))
            {
                Object[] selected = Selection.objects;
                if (selected.Length == 0) return;
                foreach (Object go in selected)
                {
                    string type = go.GetType().ToString();
                    if (type != "Forester.ForestPreset")
                    {
                        return;
                    }
                }
                LoadSelectedForest();
            }
            GUILayout.Space(5);


            GUILayout.BeginVertical(GUI.skin.window, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, true, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            for (int i = 0; i < t._ForestPresets.Count; i++)
            {
                //Panel
                GUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(20));

                int num = i + 1;
                GUILayout.Box(num.ToString(), normalStyle, GUILayout.Width(22), GUILayout.Height(15));

                t._ForestPresets[i]._ForestPreset = EditorGUILayout.ObjectField(t._ForestPresets[i]._ForestPreset, typeof(ForestPreset), false, GUILayout.Width(180), GUILayout.Height(16)) as ForestPreset;
                GUIStyle[] _ForestPresetStyles = new GUIStyle[t._ForestPresets.Count];
                _ForestPresetStyles[i] = new GUIStyle(GUI.skin.button);

                if (t._ForestPresets[i]._Include)
                {
                    _ForestPresetStyles[i].normal.background = _ActiveButton;
                }
                else
                {
                    _ForestPresetStyles[i].normal.background = _NormalButton;
                }

                if (GUILayout.Button("Include",_ForestPresetStyles[i], GUILayout.Height(15)))
                {
                    if (t._ForestPresets[i]._Include)
                    {
                        t._ForestPresets[i]._Include = false;
                    }
                    else
                    {
                        t._ForestPresets[i]._Include = true;
                    }
                }

                if (GUILayout.Button("Edit",normalStyle, GUILayout.Height(15)))
                {
                    _ForestPresetEditor = CreateInstance<ForesterPresetEditor>();
                    _ForestPresetEditor.StartEditor(t._ForestPresets[i]._ForestPreset);
                }

                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture>(_AssetPath + "/InternalResources/Sprites/Fr_Bin.png"),redStyle, GUILayout.Width(22), GUILayout.Height(15)))
                {
                    t._ForestPresets.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
                //End
            }

            if (GUILayout.Button("Add", GUILayout.Height(25)))
            {
                t._ForestPresets.Add(new ForestPresetType());
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Include All",normalStyle))
            {
                for(int i = 0; i < t._ForestPresets.Count;i++)
                {
                    t._ForestPresets[i]._Include = true;
                }
            }
            if(GUILayout.Button("Include None",normalStyle))
            {
                for (int i = 0; i < t._ForestPresets.Count; i++)
                {
                    t._ForestPresets[i]._Include = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Create", textLayout);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Object - ", GUILayout.Width(90));
            t._TargetedObject = EditorGUILayout.ObjectField(t._TargetedObject, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
            if(t._TargetedObject == null)
            {
                GUILayout.Label("Distance Expansion:", GUILayout.Width(120));
               t._DistanceExpansion = EditorGUILayout.FloatField(t._DistanceExpansion, floatLayout,GUILayout.Width(40));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.Label("Filter Textures", textLayout);
            GUILayout.Space(5);

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
                            foreach (Texture2D tx in t._TargetTextures)
                            {
                                if (splatTx.diffuseTexture == tx)
                                {
                                    exists = true;
                                }
                            }
                            if (!exists) t._TargetTextures.Add(splatTx.diffuseTexture);
                        }
                        _FilterSelectionMode = false;
                        Selection.activeGameObject = t.gameObject;
                        ActiveEditorTracker.sharedTracker.isLocked = false;
                    }
                }
            }

            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(115));
            _TargetTexScroll = GUILayout.BeginScrollView(_TargetTexScroll, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(115));
            GUILayout.BeginHorizontal(GUILayout.Width(0));
            for (int i = 0; i < t._TargetTextures.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(40), GUILayout.Height(95));
                //Rect labelRect = new Rect(rect.x + 5f, rect.y + 5, 85, 85);
                GUILayout.Space(5);
                Texture2D img = t._TargetTextures[i];
                t._TargetTextures[i] = EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.Width(65), GUILayout.Height(65)) as Texture2D;
                if (GUILayout.Button(AssetDatabase.LoadAssetAtPath<Texture2D>(_AssetPath + "/InternalResources/Sprites/Fr_Bin.png"), deleteStyle, GUILayout.Height(20)))
                {
                    t._TargetTextures.RemoveAt(i);
                }
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("+", GUILayout.Width(65), GUILayout.Height(70)))
            {
                t._TargetTextures.Add(null);
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
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            if (GUILayout.Button("Create ForestScape"))
            {
                CreateForestScape();
            }

            if (GUILayout.Button("Clear ForestScape"))
            {
                ClearForestscape();
            }

            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.Box("", GUILayout.Width(5), GUILayout.Height(38));
            GUILayout.Space(2);
            GUILayout.BeginVertical();
            if(GUILayout.Button("Mass Forestation"))
            {
                MassForestation();
            }
            if(GUILayout.Button("Mass Deforestation"))
            {
                MassDeforestation();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            if (GUILayout.Button("Save To Preset"))
            {
                if (t._ForestScapePreset == null)
                {
                    ForestScapePreset newPreset = CreateInstance<ForestScapePreset>();
                    if (!Directory.Exists(_AssetPath + "/Presets/ForestScapePresets"))
                    {
                        Directory.CreateDirectory(_AssetPath + "/Presets/ForestScapePresets");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    string name = t._PresetName;
                    if (name == "<Insert Preset Name>")
                    {
                        name = "NewForestPreset";
                    }
                    AssetDatabase.CreateAsset(newPreset, _AssetPath + "/Presets/ForestScapePresets/" + name + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    t._ForestScapePreset = AssetDatabase.LoadAssetAtPath<ForestScapePreset>(_AssetPath + "/Presets/ForestScapePresets/" + name + ".asset");
                }
                //Save settings
                t._ForestScapePreset._AreaInclude = t._AreaInclude;
                t._ForestScapePreset._ForestInclude = t._ForestInclude;
                t._ForestScapePreset._DensityRandom = t._DensityRandom;
                t._ForestScapePreset._OffsetRandom = t._OffsetRandom;
                t._ForestScapePreset._OverlapFoliage = t._OverlapFoliage;
                t._ForestScapePreset._MinFill = t._MinFill;
                t._ForestScapePreset._MaxFill = t._MaxFill;
                t._ForestScapePreset._Fill = t._Fill;
                t._ForestScapePreset._MinOffset = t._MinOffset;
                t._ForestScapePreset._MaxOffset = t._MaxOffset;
                t._ForestScapePreset._Offset = t._Offset;
                t._ForestScapePreset._MinRotation = t._MinRotation;
                t._ForestScapePreset._MaxRotation = t._MaxRotation;
                t._ForestScapePreset._MinScale = t._MinScale;
                t._ForestScapePreset._MaxScale = t._MaxScale;

                t._ForestScapePreset._AreaPresets.Clear();
                for (int a = 0; a < t._AreaPresets.Count; a++)
                {
                    t._ForestScapePreset._AreaPresets.Add(new AreaPresetType());
                    t._ForestScapePreset._AreaPresets[t._ForestScapePreset._AreaPresets.Count - 1]._AreaPreset = t._AreaPresets[a]._AreaPreset;
                    t._ForestScapePreset._AreaPresets[t._ForestScapePreset._AreaPresets.Count - 1]._Include = t._AreaPresets[a]._Include;
                }

                t._ForestScapePreset._ForestPresets.Clear();
                for (int b = 0; b < t._ForestPresets.Count; b++)
                {
                    t._ForestScapePreset._ForestPresets.Add(new ForestPresetType());
                    t._ForestScapePreset._ForestPresets[t._ForestScapePreset._ForestPresets.Count - 1]._ForestPreset = t._ForestPresets[b]._ForestPreset;
                    t._ForestScapePreset._ForestPresets[t._ForestScapePreset._ForestPresets.Count - 1]._Include = t._ForestPresets[b]._Include;
                }

                if (t._TargetTextures.Count > 0)
                {
                    t._ForestScapePreset._TargetTexture.Clear();
                    for (int c = 0; c < t._TargetTextures.Count; c++)
                    {
                        t._ForestScapePreset._TargetTexture.Add(t._TargetTextures[c]);
                    }
                }

                int id = t._ForestScapePreset.GetInstanceID();
                string path = AssetDatabase.GetAssetPath(id);
                AssetDatabase.RenameAsset(path, t._PresetName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(t._ForestScapePreset);

            }

            serialized.Update();
            serialized.ApplyModifiedProperties();
            //DrawDefaultInspector(); //Debug Only
        }

        void FindAssetPath()
        {
            //Check if asset path is still valid
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
        }

        void LoadForestScapePreset()
        {
            t._PresetName = t._ForestScapePreset.name;
            t._AreaInclude = t._ForestScapePreset._AreaInclude;
            t._ForestInclude = t._ForestScapePreset._ForestInclude;
            t._DensityRandom = t._ForestScapePreset._DensityRandom;
            t._OffsetRandom = t._ForestScapePreset._OffsetRandom;
            t._OverlapFoliage = t._ForestScapePreset._OverlapFoliage;
            t._MinFill = t._ForestScapePreset._MinFill;
            t._MaxFill = t._ForestScapePreset._MaxFill;
            t._Fill = t._ForestScapePreset._Fill;
            t._MinOffset = t._ForestScapePreset._MinOffset;
            t._MaxOffset = t._ForestScapePreset._MaxOffset;
            t._Offset = t._ForestScapePreset._Offset;
            t._MinRotation = t._ForestScapePreset._MinRotation;
            t._MaxRotation = t._ForestScapePreset._MaxRotation;
            t._MinScale = t._ForestScapePreset._MinScale;
            t._MaxScale = t._ForestScapePreset._MaxScale;

            t._AreaPresets.Clear();
            for(int a = 0; a < t._ForestScapePreset._AreaPresets.Count;a++)
            {
                t._AreaPresets.Add(new AreaPresetType());
                t._AreaPresets[t._AreaPresets.Count - 1]._AreaPreset = t._ForestScapePreset._AreaPresets[a]._AreaPreset;
                t._AreaPresets[t._AreaPresets.Count - 1]._Include = t._ForestScapePreset._AreaPresets[a]._Include;
            }

            t._ForestPresets.Clear();
            for (int b = 0; b < t._ForestScapePreset._ForestPresets.Count; b++)
            {
                t._ForestPresets.Add(new ForestPresetType());
                t._ForestPresets[t._ForestPresets.Count - 1]._ForestPreset = t._ForestScapePreset._ForestPresets[b]._ForestPreset;
                t._ForestPresets[t._ForestPresets.Count - 1]._Include = t._ForestScapePreset._ForestPresets[b]._Include;
            }

            if (t._ForestScapePreset._TargetTexture.Count > 0)
            {
                t._TargetTextures.Clear();
                for (int c = 0; c < t._ForestScapePreset._TargetTexture.Count; c++)
                {
                    t._TargetTextures.Add(t._ForestScapePreset._TargetTexture[c]);
                }
            }

            t._CurrentPreset = t._PresetName;

            //-----------------------------------------------------------------------
            //If a forestscape has already been created then forest  presets from the new preset must be reloaded onto existing tools
            if(t._ForestTools.Count > 0)
            {
                for(int i = 0; i < t._ForestTools.Count;i++)
                {
                    t._ForestTools[i].GetComponent<ForesterTool>()._ForestPreset = t._ForestPresets[PickRandomForestPreset()]._ForestPreset;
                }
            }
        }

        void MassForestation()
        {
            int countIncluded = 0;
            for (int n = 0; n < t._ForestPresets.Count; n++)
            {
                if (t._ForestPresets[n]._Include)
                {
                    countIncluded++;
                }
                if (t._ForestPresets[n]._ForestPreset == null)
                {
                    EditorUtility.DisplayDialog("A forest preset is empty", "A forest preset has been left empty, you will need to remove this or add a preset to continue", "Ok");
                    return;
                }
            }

            float progress = 0;
            if (t._ForestPresets.Count > 0 && t._ForestTools.Count > 0 && countIncluded > 0)
            {
                for (int i = 0; i < t._ForestTools.Count; i++)
                {
                    ForesterTool foresterTool = t._ForestTools[i].GetComponent<ForesterTool>();
                    foresterTool._ShowProgressBar = false;
                    foresterTool.Deforestation(false,-1,true);
                    foresterTool._ForestPreset = t._ForestPresets[PickRandomForestPreset()]._ForestPreset;
                    foresterTool.LoadForestPreset(); // Reload preset to prevent clearing
                    foresterTool.IncludeAllGroups();
                    foresterTool.CalulateCenter();
                    foresterTool.CalculateDistance();
                    foresterTool._ShowProgressBar = false;

                    if (t._TargetTextures.Count > 0)
                    {
                        foreach (ForestGroup fg in foresterTool._ForestGroup)
                        {
                            fg._TargetTextures = t._TargetTextures;
                        }
                    }

                    foresterTool.CheckPrerequisites(0, 0, false, false);

                    float rawProgress = ((foresterTool._Progress * 100) + (i * 100)) / ((t._ForestTools.Count - 1) * 100);
                    progress = Mathf.Clamp((rawProgress - 0) / (1 - 0), 0, 1); // normalised
                    if (EditorUtility.DisplayCancelableProgressBar("Planting Forestscape...", (Mathf.RoundToInt(progress * 100)) + "%", progress))
                    {
                        break;
                    }
                }

                if (!t._OverlapFoliage)
                {
                    for (int i = 0; i < t._ForestTools.Count; i++)
                    {
                        ForesterTool forestTool = t._ForestTools[i].GetComponent<ForesterTool>();
                        forestTool.ForestOverlap();
                    }
                }
                
            }
            EditorUtility.ClearProgressBar();
            GUIUtility.ExitGUI();
        }

        void MassDeforestation()
        {
            if (t._ForestTools.Count > 0)
            {
                for (int a = 0; a < t._ForestTools.Count; a++)
                {

                    ForesterTool foresterTool = t._ForestTools[a].GetComponent<ForesterTool>();
                    foresterTool._ShowProgressBar = false;
                    foresterTool.Deforestation(false,-1,true);
                    DestroyImmediate(foresterTool._ForestHolder);
                    foresterTool._CreatedObjects.Clear();

                    float progress = ((float)(a + 1) / t._ForestTools.Count) * 100;

                    if (EditorUtility.DisplayCancelableProgressBar("Clearing Forest...", Mathf.Round(progress) + "%", Mathf.Clamp((progress / 100), 0, 1)))
                    {
                        break;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            GUIUtility.ExitGUI();
        }

        void LoadSelectedArea()
        {
            List<AreaPreset> areaPresetList = new List<AreaPreset>();

            foreach (object obj in Selection.objects)
            {
                areaPresetList.Add((AreaPreset)obj);
            }

            for (int i = 0; i < areaPresetList.Count; i++)
            {
                foreach (AreaPresetType areaPresetType in t._AreaPresets)
                {
                    if (areaPresetType._AreaPreset.name == areaPresetList[i].name)
                        return;
                }
                t._AreaPresets.Add(new AreaPresetType());
                t._AreaPresets[t._AreaPresets.Count - 1]._AreaPreset = areaPresetList[i];
            }
        }

        void LoadSelectedForest()
        {
            List<ForestPreset> forestPresetList = new List<ForestPreset>();

            foreach (object obj in Selection.objects)
            {
                forestPresetList.Add((ForestPreset)obj);
            }

            for (int i = 0; i < forestPresetList.Count; i++)
            {
                foreach (ForestPresetType forestPreset in t._ForestPresets)
                {
                    if (forestPreset._ForestPreset.name == forestPresetList[i].name)
                        return;
                }
                t._ForestPresets.Add(new ForestPresetType());
                t._ForestPresets[t._ForestPresets.Count - 1]._ForestPreset = forestPresetList[i];
            }
        }

        void CreateForestScape()
        {
            float width = 0;
            float height = 0;

            bool hasAreaPreset = false;
            bool hasForestPreset = false;

            for (int i = 0; i < t._AreaPresets.Count; i++)
            {
                if (t._AreaPresets[i]._Include && t._AreaPresets[i]._AreaPreset != null) hasAreaPreset = true;
                if (t._AreaPresets[i]._AreaPreset == null)
                {
                    EditorUtility.DisplayDialog("Empty Area Preset found", "Please remove any empty area presets to continue", "Ok");
                    return;
                }
            }
            for (int i = 0; i < t._ForestPresets.Count; i++)
            {
                if (t._ForestPresets[i]._Include && t._ForestPresets[i]._ForestPreset != null) hasForestPreset = true;
                if (t._ForestPresets[i]._ForestPreset == null)
                {
                    EditorUtility.DisplayDialog("Empty Forest Preset found", "Please remove any empty forest presets to continue", "Ok");
                    return;
                }
            }

            if (hasForestPreset && hasAreaPreset)
            {

                if (t._AreaPresets.Count == 0 || t._ForestPresets.Count == 0)
                {
                    return;
                }

                ClearForestscape();

                Vector3 center = new Vector3(0, 0, 0);
                if (t._TargetedObject != null)
                {
                    int layerNum = LayerMask.NameToLayer("Terrain");
                    if (t._TargetedObject.layer == layerNum && t._TargetedObject.GetComponent<Renderer>() != null)
                    {
                        width = t._TargetedObject.GetComponent<Renderer>().bounds.size.x;
                        height = t._TargetedObject.GetComponent<Renderer>().bounds.size.z;
                        center = t._TargetedObject.GetComponent<Renderer>().bounds.center;
                    }
                    else if (t._TargetedObject.layer == layerNum && t._TargetedObject.GetComponent<Terrain>() != null)
                    {
                        width = t._TargetedObject.GetComponent<Terrain>().terrainData.size.x;
                        height = t._TargetedObject.GetComponent<Terrain>().terrainData.size.z;
                        center = new Vector3(t._TargetedObject.transform.position.x + width / 2, 0, t._TargetedObject.transform.position.z + height / 2);
                    }
                }
                else
                {
                    width = t._DistanceExpansion;
                    height = t._DistanceExpansion;
                    center = t.transform.position;
                }
                //Create surrounding nodes
                float rawProgress = 0;
                int expectedNodeCount = 0;

                for (float z = 0; z < height + (height / 10); z += height / t._Fill)
                {
                    for (float x = 0; x < width + (width / 10); x += width / t._Fill)
                    {
                        expectedNodeCount++;
                    }
                }
                Vector3 startPos = new Vector3(center.x - width / 2, 0, center.z - height / 2);
                for (float z = 0; z < height + (height / 10); z += height / t._Fill)
                {
                    for (float x = 0; x < width + (width / 10); x += width / t._Fill)
                    {
                        CreateForestNode(startPos, new Vector3(x, 0, z));
                        rawProgress += 1;
                    }
                    float progress = (rawProgress / expectedNodeCount) * 100;
                    if (EditorUtility.DisplayCancelableProgressBar("Creating Forest Areas...", Mathf.RoundToInt(progress) + "%", progress / 100))
                    {
                        break;
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            GUIUtility.ExitGUI();
        }

        void CreateForestNode(Vector3 pos, Vector3 offset)
        {
            GameObject newForest = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(_AssetPath + "/InternalResources/Prefabs/Forester.prefab")) as GameObject;
            t._ForestTools.Add(newForest);
            int forestIndex = t._ForestTools.Count - 1;
            newForest.name = "Forester";
            newForest.transform.SetParent(t.gameObject.transform);
            newForest.transform.position = new Vector3((pos.x + offset.x) + Random.Range(-t._Offset, t._Offset), pos.y, (pos.z + offset.z) + Random.Range(-t._Offset, t._Offset));

            int randomAreaPreset = Random.Range(0, t._AreaPresets.Count);
            int randomForestPreset = 0;

            int areaIncCount = 0;
            for (int a = 0; a < t._AreaPresets.Count; a++)
            {
                if (t._AreaPresets[a]._Include) areaIncCount++;
            }

            if (areaIncCount > 0)
            {
                while (!t._AreaPresets[randomAreaPreset]._Include)
                {
                    randomAreaPreset = Random.Range(0, t._AreaPresets.Count);
                }
            }
            else
            {
                return;
            }

            int forestIncCount = 0;
            for(int b = 0;b < t._ForestPresets.Count;b++)
            {
                if (t._ForestPresets[b]._Include) forestIncCount++;
            }

            if (forestIncCount > 0)
            {
                randomForestPreset = PickRandomForestPreset();
            }

            else
            {
                return;
            }


            //Add Properties
            ForesterTool foresterTool = newForest.GetComponent<ForesterTool>();
            foresterTool._AreaPreset = t._AreaPresets[randomAreaPreset]._AreaPreset;
            foresterTool._ForestPreset = t._ForestPresets[randomForestPreset]._ForestPreset;
            foresterTool.ResetAssetPath();
            foresterTool.LoadForestPreset();
            foresterTool.IncludeAllGroups();
            foresterTool.SetScaleAndRot(true, true, t._MinScale, t._MaxScale, t._MinRotation, t._MaxRotation);

            if (t._TargetTextures.Count > 0)
            {
                foreach (ForestGroup fg in foresterTool._ForestGroup)
                {
                    fg._TargetTextures = t._TargetTextures;
                }
            }

            foresterTool.Rebuild(true);
            
            //--------------------------------------------------------
            //Check validity of forestscape
            bool valid = false;
            t._ForestTools[forestIndex].transform.position = new Vector3(t._ForestTools[forestIndex].transform.position.x, t._ForestTools[forestIndex].transform.position.y + 0.5f, t._ForestTools[forestIndex].transform.position.z);
            foreach (GameObject point in t._ForestTools[forestIndex].GetComponent<ForesterTool>()._Points)
            {
                RaycastHit hit;
                if (Physics.Raycast(point.transform.position, Vector3.down, out hit))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {
                        valid = true;
                    }
                }
            }

            t._ForestTools[forestIndex].transform.position = new Vector3(t._ForestTools[forestIndex].transform.position.x, t._ForestTools[forestIndex].transform.position.y - 0.5f, t._ForestTools[forestIndex].transform.position.z);

            if (!valid)
            {
                t._ForestTools.RemoveAt(forestIndex);
                DestroyImmediate(newForest);
                return;
            }


            //--------------------------------------------------------

            foresterTool.CalulateCenter();
            foresterTool.CalculateDistance();
        }

        int PickRandomForestPreset ()
        {
            int randomForestPreset = Random.Range(0, t._ForestPresets.Count);
            while (!t._ForestPresets[randomForestPreset]._Include)
            {
                randomForestPreset = Random.Range(0, t._ForestPresets.Count);
            }
            return randomForestPreset;
        }

        void ClearForestscape()
        {
            if (t._ForestTools.Count > 0)
            {
                for (int a = 0; a < t._ForestTools.Count; a++)
                {
                    ForesterTool foresterTool = t._ForestTools[a].GetComponent<ForesterTool>();
                    foresterTool.Deforestation(false,-1,false);
                    DestroyImmediate(foresterTool._ForestHolder);
                    foresterTool._CreatedObjects.Clear();
                    DestroyImmediate(t._ForestTools[a]);
                }
                t._ForestTools.Clear();
            }
        }
    }
}

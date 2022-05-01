using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

namespace Forester
{
    public class AutoVertexColorEditor : EditorWindow
    {

        string assetPath;

        private List<int> selectedVertsIndex = new List<int>();
        private List<int> frozenVertsIndex = new List<int>();
        private Color[] _StoredVertexColors;

        bool _FreePaint = false;
        bool _SelectVertices = false;
        bool _FreezeSelection = false;
        bool _ShowColours = false;

        public float _OutInValue = 1.0f;
        public float _TopBotValue = 1.0f;
        public float _ColourValue = 1.0f;

        public Gradient _VerticalColorRamp;
        public Gradient _HorizontalColorRamp;
        private GradientColorKey[] defaultVerticalGradient;
        private GradientColorKey[] defaultHorizontalGradient;

        private bool _CtrlKeyDown = true;
        private bool _ShiftKeyDown = true;

        Texture2D _NormalButton;
        Texture2D _ActiveButton;
        Color _ButtonFontColour;



        public GameObject selection;
        public Material[] selectionMaterials;

        public Color[] colorsY;
        public Color[] colorsX;
        public Color[] colorsZ;
        public Color[] blendedColors;
        public Vector3[] vertices;
        public Mesh mesh;
        public float _Spread = 0.2f;

        public GameObject _Point;
        public List<GameObject> _Points = new List<GameObject>();


        [MenuItem("Tools/Forester/Tools/Auto Vertex Color")]
        static void Init()
        {

            // Get existing open window or if none, make a new one:
            AutoVertexColorEditor window = GetWindow<AutoVertexColorEditor>("Auto Vertex Color");

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
            GUIContent titleContent = new GUIContent("AVC Editor", AssetDatabase.LoadAssetAtPath<Texture2D>(window.assetPath + "/InternalResources/Sprites/Fr_WindowIcon.png"));
            window.titleContent = titleContent;
            window.Show();
            window.minSize = new Vector2(250, 330);
            window.maxSize = new Vector2(250, 330);
        }

        void OnDestroy()
        {
            GarbageCleanup();
        }

        void GarbageCleanup()
        {
            ReturnMaterials();
            ClearSelectedVertices();
            ClearFrozen();
        }

        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
            SetDefaultGradient();
        }

        void SetDefaultGradient()
        {
            _VerticalColorRamp = new Gradient();
            defaultVerticalGradient = new GradientColorKey[2];
            defaultVerticalGradient[0].color = Color.red;
            defaultVerticalGradient[0].time = 0;
            defaultVerticalGradient[1].color = Color.green;
            defaultVerticalGradient[1].time = 1;
            _VerticalColorRamp.colorKeys = defaultVerticalGradient;

            _HorizontalColorRamp = new Gradient();
            defaultHorizontalGradient = new GradientColorKey[3];
            defaultHorizontalGradient[0].color = Color.green;
            defaultHorizontalGradient[0].time = 0;
            defaultHorizontalGradient[1].color = Color.red;
            defaultHorizontalGradient[1].time = 0.5f;
            defaultHorizontalGradient[2].color = Color.green;
            defaultHorizontalGradient[2].time = 1;
            _HorizontalColorRamp.colorKeys = defaultHorizontalGradient;
        }

        void OnScene(SceneView sceneview)
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

            Event e = Event.current;

            if (Selection.activeGameObject != selection)
            {
                _ShowColours = false;
                GarbageCleanup();
            }

            _CtrlKeyDown = e.keyCode == KeyCode.LeftControl ? true : false;

            _ShiftKeyDown = e.modifiers == EventModifiers.Shift ? true : false;

            if (_FreePaint || _SelectVertices || _ShowColours)
            {
                Ray ray;
                RaycastHit hit;
                ray = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y));

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == selection)
                    {
                        Vector3[] vertPos = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
                        int[] triangles = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.triangles;
                        Color[] vertColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;

                        Mesh mesh = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh;

                            if (_FreePaint && _ShiftKeyDown)
                            {
                                float[] distance = new float[vertPos.Length];
                                for (int i = 0; i < vertPos.Length; i++)
                                {
                                    distance[i] = Vector3.Distance(Selection.activeGameObject.transform.TransformPoint(vertPos[i]), hit.point);
                                }
                                for (int i = 0; i < distance.Length; i++)
                                {
                                    if (distance[i] == Mathf.Min(distance))
                                    {
                                        vertColors[i] = Color.Lerp(Color.red, Color.green, _ColourValue);
                                    }
                                }

                                for (int n = 0; n < frozenVertsIndex.Count; n++)
                                {
                                    vertColors[frozenVertsIndex[n]] = Color.blue;
                                }

                                mesh.colors = vertColors;
                            }

                            if (_SelectVertices)
                            {
                                SetPoint();
                                float[] distance = new float[vertPos.Length];
                                for (int i = 0; i < vertPos.Length; i++)
                                {
                                    distance[i] = Vector3.Distance(Selection.activeGameObject.transform.TransformPoint(vertPos[i]), hit.point);
                                }
                            for (int i = 0; i < distance.Length; i++)
                            {
                                if (distance[i] == Mathf.Min(distance))
                                {
                                    for (int n = 0; n < _Points.Count; n++)
                                    {

                                        if (_Points[n].name == "VertPoint" + (i).ToString())
                                        {
                                            if (_CtrlKeyDown)
                                            {
                                                DestroyImmediate(_Points[n]);
                                                _Points.RemoveAt(n);
                                                selectedVertsIndex.RemoveAt(n);
                                                return;
                                            }
                                            return;
                                        }
                                    }
                                    if (!_CtrlKeyDown && _ShiftKeyDown)
                                    {
                                        selectedVertsIndex.Add(i);
                                        GameObject newPoint = Instantiate(_Point, Selection.activeGameObject.transform.TransformPoint(vertPos[i]), _Point.transform.rotation) as GameObject;
                                        newPoint.hideFlags = HideFlags.HideInHierarchy;
                                        _Points.Add(newPoint);
                                        newPoint.name = ("VertPoint" + (i).ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (assetPath == null)
            {
                return;
            }

            _ButtonFontColour = Color.black;

            Color separatorsCol;

            //Is dark theme?
            if (EditorGUIUtility.isProSkin)
            {
                separatorsCol = new Color(0.5f, 0.5f, 0.5f, 0.6f);
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Dark.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Dark.png");
            }
            else
            {
                separatorsCol = new Color(0.5f, 0.5f, 0.5f, 0.35f);
                _NormalButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Normal_Light.png");
                _ActiveButton = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/InternalResources/Sprites/ButtonIcon_Active_Light.png");
            }



            Rect pos = EditorGUILayout.BeginHorizontal();
            float div = (float)Screen.width / 512;
            GUI.DrawTexture(new Rect(pos.x+5, pos.y, Mathf.Clamp(512 * div, 128, 512), Mathf.Clamp(65 * div, 10, 65)), AssetDatabase.LoadAssetAtPath<Texture>(assetPath + "/InternalResources/Sprites/Forester_AutoVertexBanner.png"));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(Mathf.Clamp(65 * div, 10, 65));

            pos = EditorGUILayout.BeginHorizontal();
            GUI.color = separatorsCol;
            GUI.Box(new Rect(pos.x+5, pos.y, Screen.width - 8, Mathf.Clamp(15 * div, 15, 15)), "", GUI.skin.button);
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Documentation", GUI.skin.label))
            {
                Application.OpenURL("https://docs.google.com/document/d/1l9FrHSqnTfKGsi2TgIF8vZM7ttiZY36Rt5fqGXwMVK8/edit?usp=sharing");
            }
            /*
            GUILayout.Label("|");
            if (GUILayout.Button("Video Tutorials", GUI.skin.label))
            {
                Application.OpenURL("https://www.youtube.com/playlist?list=PL9CU-Qj1tQBXjaTqS6C8sUInXOpbksrBG");
            }
            */
            GUILayout.Label("|");
            if (GUILayout.Button("Contact", GUI.skin.label))
            {
                EmailMe();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
           
            var NormalStyle = new GUIStyle(GUI.skin.button);
            NormalStyle.normal.background = _NormalButton;

            var textLayout = new GUIStyle(GUI.skin.textArea);
            textLayout.alignment = TextAnchor.MiddleCenter;

            var floatLayout = new GUIStyle(GUI.skin.textArea);
            floatLayout.alignment = TextAnchor.MiddleCenter;
            floatLayout.stretchWidth = true;
            floatLayout.fixedWidth = 40;

            EditorGUILayout.LabelField("Version 1.2", textLayout);

            GUILayout.Space(10);


            if (GUILayout.Button("Auto Generate", NormalStyle))
            {
                //Automatically generate from values below
                BuildVertexColours(Selection.activeGameObject, _VerticalColorRamp, _HorizontalColorRamp, frozenVertsIndex);

            }
            SerializedObject serializedGradient = new SerializedObject(this);
            serializedGradient.Update();
            SerializedProperty verticalColorGradient = serializedGradient.FindProperty("_VerticalColorRamp");
            EditorGUILayout.PropertyField(verticalColorGradient);
            SerializedProperty horizontalColorGradient = serializedGradient.FindProperty("_HorizontalColorRamp");
            EditorGUILayout.PropertyField(horizontalColorGradient);
            serializedGradient.ApplyModifiedProperties();



            GUILayout.Space(10);
            var style = new GUIStyle(GUI.skin.button);

            if (_FreePaint)
            {
                style.normal.background = _ActiveButton;
                style.normal.textColor = _ButtonFontColour;
            }
            else
            {
                style.normal.background = _NormalButton;
            }

            if (GUILayout.Button("Free Paint", style))
            {
                if (!_FreePaint)
                {
                    _SelectVertices = false;
                    ClearSelectedVertices();
                    _FreePaint = true;
                    SetPoint();
                }
                else
                {
                    _FreePaint = false;
                }
            }

            var vertStyle = new GUIStyle(GUI.skin.button);

            if (_SelectVertices)
            {
                vertStyle.normal.background = _ActiveButton;
                vertStyle.normal.textColor = _ButtonFontColour;
            }
            else
            {
                vertStyle.normal.background = _NormalButton;
            }

            if (GUILayout.Button("Select Vertices", vertStyle))
            {
                if (!_SelectVertices)
                {
                    _SelectVertices = true;
                    _FreePaint = false;
                }
                else
                {
                    ClearSelectedVertices();
                }
            }

            if (GUILayout.Button("Fill Object / Selection", NormalStyle))
            {
                Color[] vertColors;
                if (selection == null || selection.GetComponent<MeshFilter>() == null)
                {
                    return;
                }
                vertColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;
                if (!_SelectVertices)
                {
                    for (int i = 0; i < vertColors.Length; i++)
                    {
                        vertColors[i] = Color.Lerp(Color.red, Color.green, _ColourValue);
                    }
                }
                else
                {
                    for (int i = 0; i < selectedVertsIndex.Count; i++)
                    {
                        vertColors[selectedVertsIndex[i]] = Color.Lerp(Color.red, Color.green, _ColourValue);
                    }
                }

                for (int n = 0; n < frozenVertsIndex.Count; n++)
                {
                    vertColors[frozenVertsIndex[n]] = Color.blue;
                }

                mesh.colors = vertColors;
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.Space(25);
            EditorGUILayout.LabelField("Set Paint Value:", GUILayout.MaxWidth(100));

            _ColourValue = EditorGUILayout.FloatField(_ColourValue, floatLayout);
            _ColourValue = Mathf.Clamp(_ColourValue, 0.0f, 1.0f);

            if (GUILayout.Button("-", NormalStyle))
            {
                if (selection == null || selection.GetComponent<MeshFilter>() == null)
                {
                    return;
                }
                Color[] vertColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;
                if (_SelectVertices)
                {
                    for (int i = 0; i < selectedVertsIndex.Count; i++)
                    {
                        vertColors[selectedVertsIndex[i]] = Color.Lerp(Color.red, Color.green, Mathf.Clamp(vertColors[selectedVertsIndex[i]].g - _ColourValue, 0, 1));
                    }
                }

                for (int n = 0; n < frozenVertsIndex.Count; n++)
                {
                    vertColors[frozenVertsIndex[n]] = Color.blue;
                }

                mesh.colors = vertColors;
            }

            if (GUILayout.Button("+", NormalStyle))
            {
                if (selection == null || selection.GetComponent<MeshFilter>() == null)
                {
                    return;
                }
                Color[] vertColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;
                if (_SelectVertices)
                {
                    for (int i = 0; i < selectedVertsIndex.Count; i++)
                    {
                        vertColors[selectedVertsIndex[i]] = Color.Lerp(Color.red, Color.green, Mathf.Clamp(vertColors[selectedVertsIndex[i]].g + _ColourValue, 0, 1));
                    }
                }

                for (int n = 0; n < frozenVertsIndex.Count; n++)
                {
                    vertColors[frozenVertsIndex[n]] = Color.blue;
                }

                mesh.colors = vertColors;
            }

            GUILayout.Space(80);

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Freeze/Unfreeze", NormalStyle))
            {
                if (selection == null || selection.GetComponent<MeshFilter>() == null)
                {
                    return;
                }
                Color[] vertColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;
                if (vertColors.Length < 1)
                {
                    CallWarning("Mesh has no vertex colors!", "The current mesh has no vertex colors assigned, please assign some and then retry my friend.", "Ah snap...");
                }
                if (!_FreezeSelection && _SelectVertices)
                {
                    _FreezeSelection = true;
                    for (int i = 0; i < selectedVertsIndex.Count; i++)
                    {
                        frozenVertsIndex.Add(selectedVertsIndex[i]);
                        _StoredVertexColors = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.colors;
                        vertColors[frozenVertsIndex[i]] = Color.blue;
                    }
                    mesh.colors = vertColors;
                }
                else
                {
                    ClearFrozen();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Toggle Colors", NormalStyle))
            {
                if (!_ShowColours)
                {
                    _ShowColours = true;
                    ColourMode(_ShowColours);
                }
                else
                {
                    _ShowColours = false;
                    ColourMode(_ShowColours);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Save As Prefab", NormalStyle))
            {
                CreatePrefab(selection, mesh);
            }

            GUILayout.Space(10);
        }

        void ClearSelectedVertices()
        {
            _SelectVertices = false;
            for (int i = 0; i < _Points.Count; i++)
            {
                DestroyImmediate(_Points[i]);
            }
            _Points.Clear();
            selectedVertsIndex.Clear();
        }

        void ClearFrozen()
        {
            _FreezeSelection = false;
            Color[] vertColors;
            if (mesh != null)
            {
                vertColors = mesh.colors;
                if (vertColors.Length > 0)
                {
                    for (int i = 0; i < frozenVertsIndex.Count; i++)
                    {
                        vertColors[frozenVertsIndex[i]] = _StoredVertexColors[frozenVertsIndex[i]];
                    }
                    frozenVertsIndex.Clear();
                    mesh.colors = vertColors;
                }
            }
        }

        void CallWarning(string header, string msg, string option1)
        {
            EditorUtility.DisplayDialog(header, msg, option1);
        }

        public void SetPoint()
        {
            if (_Point == null)
            {
                _Point = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath + "/InternalResources/Prefabs/VertexPoint.prefab");
            }
        }

        public void GrabMaterials(GameObject go)
        {
            selection = go;
            if (go == null || selection.GetComponent<MeshRenderer>() == null)
            {
                return;
            }
            selectionMaterials = go.GetComponent<MeshRenderer>().sharedMaterials;
            if (selection.GetComponent<MeshFilter>() == null)
            {
                return;
            }
            mesh = go.GetComponent<MeshFilter>().sharedMesh;
        }

        public void ReturnMaterials()
        {
            if (selection == null || selection.GetComponent<MeshRenderer>() == null)
            {
                GrabMaterials(Selection.activeGameObject);
                return;
            }
            if (selection != null && selectionMaterials.Length > 0) selection.GetComponent<MeshRenderer>().sharedMaterials = selectionMaterials;
            GrabMaterials(Selection.activeGameObject);
        }

        public void ColourMode(bool vertex)
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<MeshRenderer>() == null)
            {
                return;
            }
            MeshRenderer renderer = selection.GetComponent<MeshRenderer>();
            if (vertex)
            {
                Material[] vertMat = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    vertMat[i] = AssetDatabase.LoadAssetAtPath<Material>(assetPath + "/InternalResources/Materials/VertexColorMode.mat");
                }
                renderer.sharedMaterials = vertMat;
            }
            if (!vertex)
            {
                renderer.sharedMaterials = selectionMaterials;
            }
        }


        public void BuildVertexColours(GameObject go, Gradient _VerticalGradient, Gradient _HorizontalGradient, List<int> frozenIndex)
        {
            if (selection == null || selection.GetComponent<MeshFilter>() == null)
            {
                return;
            }
            mesh = go.GetComponent<MeshFilter>().sharedMesh;
            vertices = mesh.vertices;

            blendedColors = new Color[vertices.Length];
            colorsY = new Color[vertices.Length];
            colorsX = new Color[vertices.Length];
            colorsZ = new Color[vertices.Length];

            float lowestValueY = 0;
            float highestValueY = 0;
            float lowestValueX = 0;
            float highestValueX = 0;
            float lowestValueZ = 0;
            float highestValueZ = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                lowestValueY = Mathf.Min(lowestValueY, vertices[i].y);
                highestValueY = Mathf.Max(highestValueY, vertices[i].y);

                lowestValueX = Mathf.Min(lowestValueX, vertices[i].x);
                highestValueX = Mathf.Max(highestValueX, vertices[i].x);

                lowestValueZ = Mathf.Min(lowestValueZ, vertices[i].z);
                highestValueZ = Mathf.Max(highestValueZ, vertices[i].z);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                float yRangeNormalised = (vertices[i].y - lowestValueY) / (highestValueY - lowestValueY);
                float xRangeNormalised = (vertices[i].x - lowestValueX) / (highestValueX - lowestValueX);
                float zRangeNormalised = (vertices[i].z - lowestValueZ) / (highestValueZ - lowestValueZ);

                colorsY[i] = _VerticalGradient.Evaluate(yRangeNormalised);
                colorsX[i] = _HorizontalGradient.Evaluate(xRangeNormalised);
                colorsZ[i] = _HorizontalGradient.Evaluate(zRangeNormalised);
            }

            blendedColors = colorsX;
            for (int i = 0; i < colorsX.Length; i++)
            {
                if (colorsX[i].r > 0.5f && colorsZ[i].g > 0.5f)
                {
                    blendedColors[i] = Color.Lerp(colorsX[i], colorsZ[i], 1f);
                }

                if (blendedColors[i].r > 0.5f && colorsY[i].g > 0.5f)
                {
                    blendedColors[i] = Color.Lerp(blendedColors[i], colorsY[i], 1f);
                }
            }

            for (int n = 0; n < frozenIndex.Count; n++)
            {
                blendedColors[frozenIndex[n]] = Color.blue;
            }

            mesh.colors = blendedColors;
        }

        public void CreatePrefab(GameObject go, Mesh mesh)
        {
            if (selection == null || selection.GetComponent<MeshFilter>() == null)
            {
                return;
            }

            Directory.CreateDirectory("Assets/Prefabs/Forest/Meshes/");
            GameObject newPrefab;
            newPrefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/Forest/" + go.transform.name + "_VC" + ".prefab");

            //Duplicates mesh
            Mesh meshInstance = new Mesh();
            meshInstance.vertices = mesh.vertices;
            meshInstance.triangles = mesh.triangles;
            meshInstance.subMeshCount = mesh.subMeshCount;
            meshInstance.uv = mesh.uv;
            meshInstance.normals = mesh.normals;
            meshInstance.colors = mesh.colors;
            meshInstance.tangents = mesh.tangents;

            for (int i = 0; i < meshInstance.subMeshCount; i++)
            {
                int[] submeshTris = Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.GetTriangles(i);
                meshInstance.SetTriangles(submeshTris, i);
            }
            AssetDatabase.CreateAsset(meshInstance, "Assets/Prefabs/Forest/Meshes/" + go.transform.name + "_Mesh.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            newPrefab.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Prefabs/Forest/Meshes/" + go.transform.name + "_Mesh.asset");
            newPrefab.GetComponent<MeshRenderer>().sharedMaterials = selectionMaterials;
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
    }
}

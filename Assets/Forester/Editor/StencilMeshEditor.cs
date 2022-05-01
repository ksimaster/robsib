using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Forester
{
    [CustomEditor(typeof(StencilMesh)), CanEditMultipleObjects]
    [System.Serializable]
    public class StencilMeshEditor : Editor
    {
        StencilMesh s;
        string _AssetPath;
        Vector2 _GroupScroll = Vector2.zero;
        Vector2 _TargetTexScroll = Vector2.zero;

        List<Vector3> _Vertices = new List<Vector3>();
        List<Vector3> _Normals = new List<Vector3>();
        List<int> _Triangles = new List<int>();
        List<Vector2> _UVs = new List<Vector2>();

        Texture2D _NormalButton;
        Texture2D _ActiveButton;
        Color _ButtonFontColour;

        // draw lines between a chosen game object
        // and a selection of added game objects
        void OnEnable()
        {
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
            s = target as StencilMesh;

            if (_AssetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        _AssetPath = dir;
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
                            }
                        }
                    }
                }
            }

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            bool exists = false;
            for (int a = 0; a < 32; a++)
            {
                if (layers.GetArrayElementAtIndex(a).stringValue == "ForestStencil")
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                for (int b = 8; b < 32; b++)
                {
                    if (!exists & layers.GetArrayElementAtIndex(b) == null || !exists & layers.GetArrayElementAtIndex(b).stringValue == "")
                    {
                        layers.GetArrayElementAtIndex(b).stringValue = "ForestStencil";
                        exists = true;
                    }
                }
            }
            tagManager.ApplyModifiedProperties();


            MeshRenderer meshRenderer = s.GetComponent<MeshRenderer>();
            if (meshRenderer.sharedMaterial == null)
            {
                Material mat = Instantiate(AssetDatabase.LoadAssetAtPath<Material>(_AssetPath + "/InternalResources/Materials/StencilMask.mat"));
                meshRenderer.material = new Material(mat);
            }
            meshRenderer.sharedMaterial.mainTexture = s.GetComponent<StencilMesh>()._Tex;
            CreateMesh(true);
        }

        void OnDisable()
        {

        }

        void OnScene(SceneView sceneview)
        {
            CreateMesh();
        }

        void CreateMesh(bool update = false)
        {
            s = target as StencilMesh;
            if (s != null)
            {
                if (s._LastPosition != s.transform.position || s._LastRotation != s.transform.eulerAngles || s._LastScale != s.transform.localScale || update)
                {
                    s._LastPosition = s.transform.position;
                    s._LastRotation = s.transform.eulerAngles;
                    s._LastScale = s.transform.localScale;
                    _Vertices.Clear();
                    _Triangles.Clear();
                    _Normals.Clear();
                    _UVs.Clear();
                    int adjustedQuads = Mathf.RoundToInt(Mathf.Clamp(s._Subdivisions * Mathf.RoundToInt(s.transform.localScale.x), 0.1f, Mathf.Sqrt(s._MaxResolution)));
                    for (int c = 0; c < adjustedQuads; c++)
                    {
                        for (int r = 0; r < adjustedQuads; r++)
                        {
                            List<int> vertIndex = new List<int>();
                            List<Vector3> vertices = new List<Vector3>();
                            List<Vector3> normals = new List<Vector3>();
                            for (int x = 0; x < 2; x++)
                            {
                                for (int z = 0; z < 2; z++)
                                {
                                    Matrix4x4 scale = Matrix4x4.Scale(Vector3.one / adjustedQuads);
                                    s.transform.localScale = new Vector3(s.transform.localScale.x, 1, s.transform.localScale.z);
                                    Vector3 vertPos = new Vector3((x + r) - (adjustedQuads / 2), 0, (z + c) - (adjustedQuads / 2));
                                    vertPos = new Vector3(vertPos.x, 2000, vertPos.z);
                                    vertPos = scale.MultiplyPoint3x4(vertPos);
                                    Vector3 vertPos2World = s.transform.TransformPoint(vertPos);

                                    RaycastHit hit;
                                    int layerMask = LayerMask.NameToLayer("Terrain");
                                    if (Physics.Raycast(vertPos2World, -s.transform.up, out hit, 3000, 1 << layerMask))
                                    {
                                        vertPos = s.transform.InverseTransformPoint(hit.point);
                                        s._LastHeight = vertPos.y + s._DistanceFromSurface;
                                        vertPos = new Vector3(vertPos.x,s._LastHeight,vertPos.z);
                                        normals.Add(hit.normal);
                                    }
                                    else
                                    {
                                        normals.Add(s.transform.up);
                                        vertPos = new Vector3(vertPos.x, s._LastHeight, vertPos.z);
                                    }
                                    vertices.Add(vertPos);
                                }
                            }

                            //build valid verts
                            List<bool> existsList = new List<bool>();
                            for (int a = 0; a < vertices.Count; a++)
                            {
                                Vector3 vert = vertices[a];
                                bool exists = false;
                                for (int i = 0; i < _Vertices.Count; i++)
                                {
                                    if (_Vertices[i] == vert)
                                    {
                                        exists = true;
                                        existsList.Add(true);
                                        vertIndex.Add(i);
                                    }
                                }
                                if (!exists)
                                {
                                    existsList.Add(false);
                                    _Vertices.Add(vert);
                                    vertIndex.Add(_Vertices.Count - 1);
                                }
                            }

                            _Triangles.Add(vertIndex[0]);
                            _Triangles.Add(vertIndex[2]);
                            _Triangles.Add(vertIndex[1]);

                            _Triangles.Add(vertIndex[2]);
                            _Triangles.Add(vertIndex[3]);
                            _Triangles.Add(vertIndex[1]);

                            if (!existsList[0]) _Normals.Add(normals[0]);
                            if (!existsList[1]) _Normals.Add(normals[1]);
                            if (!existsList[2]) _Normals.Add(normals[2]);
                            if (!existsList[3]) _Normals.Add(normals[3]);

                            float v1 = (r + 1) / (float)adjustedQuads;
                            float u1 = (c + 1) / (float)adjustedQuads;

                            float v0 = v1 - (1 / (float)adjustedQuads);
                            float u0 = u1 - (1 / (float)adjustedQuads);

                            if (!existsList[0]) _UVs.Add(new Vector2(u0, v0));
                            if (!existsList[1]) _UVs.Add(new Vector2(u1, v0));
                            if (!existsList[2]) _UVs.Add(new Vector2(u0, v1));
                            if (!existsList[3]) _UVs.Add(new Vector2(u1, v1));
                            if (s._Mesh != null)
                            {
                                s._Mesh.Clear();
                                s._Mesh.vertices = _Vertices.ToArray();
                                s._Mesh.triangles = _Triangles.ToArray();
                                s._Mesh.triangles = s._Mesh.triangles.Reverse().ToArray();
                                if (_Normals.Count != _Vertices.Count || _UVs.Count != _Vertices.Count) break;
                                s._Mesh.normals = _Normals.ToArray();
                                s._Mesh.uv = _UVs.ToArray();
                            }
                            SetDirty();
                        }
                    }
                    s.gameObject.GetComponent<MeshCollider>().sharedMesh = s._Mesh;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // For Debug
        }

        new void SetDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
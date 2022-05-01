using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using System.Collections.Generic;

namespace Forester
{
    [ExecuteInEditMode]
    [System.Serializable]
    public class ForesterTool : MonoBehaviour
    {
        [HideInInspector]
        public string _AssetPath;
        public ForesterController _ForestController;
        TerrainPainting _TerrainGrab = new TerrainPainting();

        [HideInInspector]
        public GameObject _Point;

        public ForestPreset _ForestPreset;
        public AreaPreset _AreaPreset;

        [HideInInspector]
        public GameObject _ForestHolder;

        [HideInInspector]
        public GameObject _ForestPrefab;

        private ParticleSystem[] _ParticleSystems;

        [HideInInspector]
        public List<GameObject> _Points = new List<GameObject>();
        
        private Vector3 _AverageMin = new Vector3(0, 0, 0);
        private Vector3 _AverageMax = new Vector3(0, 0, 0);

        public string _ForestPresetName = "";

        [HideInInspector]
        public Vector3 _Center = new Vector3(0, 0, 0);

        public List<FrozenObjects> _FrozenObjects = new List<FrozenObjects>();

        //[HideInInspector]
        public float[] _Distances;

        [HideInInspector]
        public float[] _Distance;

        [HideInInspector]
        public float[] _MaxDist;

        public bool[] _IncludeGroup;

        public List<ForestGroup> _ForestGroup = new List<ForestGroup>();
        public List<CreatedObjects> _CreatedObjects = new List<CreatedObjects>();

        [HideInInspector]
        public Vector2[] _DistanceBetween;

        [Header("Stencil Properties")]
        public List<StencilMask> _StencilMasks = new List<StencilMask>();

        [Header("Border Offset")]
        public float _BorderOffset = 2;

        private float _WorldHeight = 2000;

        [HideInInspector]
        public bool _ClosedEnd = true;

        [HideInInspector]
        public float _Progress = 0F;
        [HideInInspector]
        public bool _ShowProgressBar = true;
        private bool _Working = false;
        public bool legacyUpdate;

        private bool _RetainPrefabs = true;

#if UNITY_EDITOR
        void Awake()
        {
            if (!Application.isEditor || Application.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }

        void Start()
        {
            LegacyRepairs();
        }

        void LegacyRepairs()
        {
            //---------------TO BE REMOVED IN V1.4 THIS IS A LEGACY FIX--------------------

            if (_CreatedObjects.Count > 0 && !legacyUpdate)
            {
                for (int c = 0; c < _CreatedObjects.Count; c++)
                {
                    if (_CreatedObjects[c]._FoliageInfo.Count == 0 && _CreatedObjects[c]._FoliageObjects.Count > 0)
                    {
                        for (int f = 0; f < _CreatedObjects[c]._FoliageObjects.Count; f++)
                        {
                            _CreatedObjects[c]._FoliageInfo.Add(new FoliageInfo());
                            _CreatedObjects[c]._FoliageInfo[f]._FoliageObject = _CreatedObjects[c]._FoliageObjects[f];
                        }
                    }
                }
                legacyUpdate = true;
                SetDirty();
            }

            if (_ForestGroup.Count > 0)
            {
                for (int i = 0; i < _ForestGroup.Count; i++)
                {
                    if (_ForestGroup[i]._OffsetOverDistance.Count > 0)
                    {
                        foreach (OffsetOverDistance ood in _ForestGroup[i]._OffsetOverDistance)
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
                    }
                    _ForestGroup[i]._OffsetOverDistance.Clear();
                }
                SetDirty();
            }

            if (_CreatedObjects.Count > 0)
            {
                if(_CreatedObjects[0]._GroupName == null)
                {
                    for(int i = 0; i < _ForestGroup.Count;i++)
                    {
                        _CreatedObjects[i]._GroupName = _ForestGroup[i]._GroupName;
                        _ForestGroup[i]._CreatedObjectIndex = i;
                    }
                }
            }
            //-----------------------------------------------------------------------------
        }

        void Update()
        {

            if (_Point == null)
            {
                _Point = AssetDatabase.LoadAssetAtPath<GameObject>(_AssetPath + "/InternalResources/Prefabs/Point.prefab");
            }

            if (_ForestPrefab == null)
            {
                _ForestPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_AssetPath + "/InternalResources/Prefabs/Forest.prefab");
            }

            if (_ForestPreset == null || _ForestPreset.name != _ForestPresetName)
            {

                //Deforestation();
                //_CreatedObjects.Clear();
                _ForestGroup.Clear();
                //_FrozenObjects.Clear();
            }

            if (_ForestPreset != null)
            {
                if (_ForestPreset.name != _ForestPresetName)
                {
                    LoadForestPreset();
                }

                if (_ForestPreset._ForestGroup.Count != _ForestGroup.Count)
                {
                    LoadForestPreset(true);
                }

                for (int i = 0; i < _ForestGroup.Count; i++)
                {
                    if (_ForestGroup[i]._GroupName != _ForestPreset._ForestGroup[i]._GroupName)
                    {
                        int coIndex = GetCreatedObjectsIndex(i);
                        _ForestGroup[i]._GroupName = _ForestPreset._ForestGroup[i]._GroupName;
                        _CreatedObjects[coIndex]._GroupName = _ForestGroup[i]._GroupName;
                        _CreatedObjects[coIndex]._IncludedGroup.name = _ForestGroup[i]._GroupName;
                    }
                }
            }

            if (_ClosedEnd)
            {
                //Work out average vector
                CalulateCenter();
                CalculateDistance();
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        public void CalculateDistance()
        {

            //Calculate the distance from center to each node
            _Distances = new float[_Points.Count];
            _DistanceBetween = new Vector2[_Points.Count];
            _MaxDist = new float[_Points.Count];
            _Distance = new float[_Points.Count];

            for (int i = 0; i <= _Points.Count - 1; i++)
            {
                //Distance from center to node
                _Points[i].transform.localScale = Vector3.one;
                _Distances[i] = (Vector3.Distance(_Center, _Points[i].transform.position))/ transform.localScale.x;
 
                //Calculate the distance between each node x = distance from previous, y = distance to next
                if (i == 0)
                {
                    _DistanceBetween[i].x = (Vector3.Distance(_Points[i].transform.position, _Points[_Points.Count - 1].transform.position)) / transform.localScale.x;
                    if (_Points.Count > 1) _DistanceBetween[i].y = (Vector3.Distance(_Points[i].transform.position, _Points[i + 1].transform.position)) / transform.localScale.x;
                }
                else if (i > 0 && i < _Points.Count - 1)
                {
                    _DistanceBetween[i].x = (Vector3.Distance(_Points[i].transform.position, _Points[i - 1].transform.position)) / transform.localScale.x;
                    _DistanceBetween[i].y = (Vector3.Distance(_Points[i].transform.position, _Points[i + 1].transform.position)) / transform.localScale.x;
                }
                else if (i == _Points.Count - 1)
                {
                    _DistanceBetween[i].x = (Vector3.Distance(_Points[i].transform.position, _Points[i - 1].transform.position)) / transform.localScale.x;
                    _DistanceBetween[i].y = (Vector3.Distance(_Points[i].transform.position, _Points[0].transform.position)) / transform.localScale.x;
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public void LoadForestPreset(bool partial = false)
        {
            if (!partial) _ForestGroup.Clear();

            _ForestPresetName = _ForestPreset.name;

            //First do a cleanup for all uniqueID's that exist in forester but not the preset
            for (int a = 0; a < _ForestGroup.Count; a++)
            {
                bool keep = false;
                for (int b = 0; b < _ForestPreset._ForestGroup.Count; b++)
                {
                    if (_ForestGroup[a]._UniqueID == _ForestPreset._ForestGroup[b]._UniqueID)
                    {
                        keep = true;
                    }
                }
                if (!keep) _ForestGroup.RemoveAt(a);
            }

            for (int i = 0; i < _ForestPreset._ForestGroup.Count; i++)
            {
                //Check and see if uniqueID matches existing if so then reorder and set to skip
                bool exists = false;
                if (partial)
                {
                    for (int b = 0; b < _ForestGroup.Count; b++)
                    {
                        if (_ForestGroup[b]._UniqueID == _ForestPreset._ForestGroup[i]._UniqueID)
                        {
                            //Move
                            exists = true;
                            ForestGroup oldGroup = _ForestGroup[b];
                            _ForestGroup.RemoveAt(b);
                            _ForestGroup.Insert(i, oldGroup);
                            break;
                        }
                    }
                    if (!exists)
                    {
                        _ForestGroup.Add(new ForestGroup());
                    }
                }
                else
                {
                    _ForestGroup.Add(new ForestGroup());
                }

                if (!partial || partial && !exists)
                {
                    if (_ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length > 0)
                    {
                        _ForestGroup[i]._OffsetOverDist = new AnimationCurve();
                        for (int n = 0; n < _ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length; n++)
                        {
                            _ForestGroup[i]._OffsetOverDist.AddKey(_ForestPreset._ForestGroup[i]._OffsetOverDist.keys[n].time, _ForestPreset._ForestGroup[i]._OffsetOverDist.keys[n].value);
                        }
                        for (int n = 0; n < _ForestPreset._ForestGroup[i]._OffsetOverDist.keys.Length; n++)
                        {
                            AnimationUtility.SetKeyLeftTangentMode(_ForestGroup[i]._OffsetOverDist, n, AnimationUtility.GetKeyLeftTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, n));
                            AnimationUtility.SetKeyRightTangentMode(_ForestGroup[i]._OffsetOverDist, n, AnimationUtility.GetKeyRightTangentMode(_ForestPreset._ForestGroup[i]._OffsetOverDist, n));
                        }
                    }

                    if (_ForestPreset._ForestGroup[i]._TargetTextures.Count > 0)
                    {
                        for (int n = 0; n < _ForestPreset._ForestGroup[i]._TargetTextures.Count; n++)
                        {
                            _ForestGroup[i]._TargetTextures.Add(_ForestPreset._ForestGroup[i]._TargetTextures[n]);
                        }
                    }

                    for (int n = 0; n < _StencilMasks.Count; n++)
                    {
                        _ForestGroup[i]._StencilMaskState.Add(true);
                        _ForestGroup[i]._StencilExcludeState = false;
                    }

                    _ForestGroup[i]._MaxOffset = _ForestPreset._ForestGroup[i]._MaxOffset;
                    _ForestGroup[i]._MinOffset = _ForestPreset._ForestGroup[i]._MinOffset;
                    _ForestGroup[i]._Offset = _ForestPreset._ForestGroup[i]._Offset;
                    _ForestGroup[i]._Fill = _ForestPreset._ForestGroup[i]._Fill;
                    _ForestGroup[i]._MaxBorderOffset = _ForestPreset._ForestGroup[i]._MaxBorderOffset;
                    _ForestGroup[i]._MinBorderOffset = _ForestPreset._ForestGroup[i]._MinBorderOffset;
                    _ForestGroup[i]._BorderOffset = _ForestPreset._ForestGroup[i]._BorderOffset;
                    _ForestGroup[i]._RandomRotation = _ForestPreset._ForestGroup[i]._RandomRotation;
                    _ForestGroup[i]._RandomScale = _ForestPreset._ForestGroup[i]._RandomScale;
                    _ForestGroup[i]._MinRotation = _ForestPreset._ForestGroup[i]._MinRotation;
                    _ForestGroup[i]._MaxRotation = _ForestPreset._ForestGroup[i]._MaxRotation;
                    _ForestGroup[i]._MinScale = _ForestPreset._ForestGroup[i]._MinScale;
                    _ForestGroup[i]._MaxScale = _ForestPreset._ForestGroup[i]._MaxScale;
                    _ForestGroup[i]._AngleLimit = _ForestPreset._ForestGroup[i]._AngleLimit;
                    _ForestGroup[i]._MinAngleLimit = _ForestPreset._ForestGroup[i]._MinAngleLimit;
                    _ForestGroup[i]._MaxAngleLimit = _ForestPreset._ForestGroup[i]._MaxAngleLimit;
                    _ForestGroup[i]._UniqueID = _ForestPreset._ForestGroup[i]._UniqueID;
                    _ForestGroup[i]._HasParticles = _ForestPreset._ForestGroup[i]._HasParticles;
                    _ForestGroup[i]._FaceNormals = _ForestPreset._ForestGroup[i]._FaceNormals;
                    _ForestGroup[i]._DigDepth = _ForestPreset._ForestGroup[i]._DigDepth;
                    _ForestGroup[i]._MinDigDepth = _ForestPreset._ForestGroup[i]._MinDigDepth;
                    _ForestGroup[i]._MaxDigDepth = _ForestPreset._ForestGroup[i]._MaxDigDepth;
                    _ForestGroup[i]._FaceBorderDirection = _ForestPreset._ForestGroup[i]._FaceBorderDirection;
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
                _ForestGroup[i]._GroupName = _ForestPreset._ForestGroup[i]._GroupName;
                _ForestGroup[i]._Icon = _ForestPreset._ForestGroup[i]._Icon;
            }
        }

        public void SaveToSelectedPreset()
        {
            for (int i = 0; i < _ForestPreset._ForestGroup.Count; i++)
            {
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

                _ForestPreset._ForestGroup[i]._MaxOffset = _ForestGroup[i]._MaxOffset;
                _ForestPreset._ForestGroup[i]._MinOffset = _ForestGroup[i]._MinOffset;
                _ForestPreset._ForestGroup[i]._Offset = _ForestGroup[i]._Offset;
                _ForestPreset._ForestGroup[i]._Fill = _ForestGroup[i]._Fill;
                _ForestPreset._ForestGroup[i]._MaxBorderOffset = _ForestGroup[i]._MaxBorderOffset;
                _ForestPreset._ForestGroup[i]._MinBorderOffset = _ForestGroup[i]._MinBorderOffset;
                _ForestPreset._ForestGroup[i]._BorderOffset = _ForestGroup[i]._BorderOffset;
                _ForestPreset._ForestGroup[i]._RandomRotation = _ForestGroup[i]._RandomRotation;
                _ForestPreset._ForestGroup[i]._RandomScale = _ForestGroup[i]._RandomScale;
                _ForestPreset._ForestGroup[i]._MinRotation = _ForestGroup[i]._MinRotation;
                _ForestPreset._ForestGroup[i]._MaxRotation = _ForestGroup[i]._MaxRotation;
                _ForestPreset._ForestGroup[i]._MinScale = _ForestGroup[i]._MinScale;
                _ForestPreset._ForestGroup[i]._MaxScale = _ForestGroup[i]._MaxScale;
                _ForestPreset._ForestGroup[i]._AngleLimit = _ForestGroup[i]._AngleLimit;
                _ForestPreset._ForestGroup[i]._MinAngleLimit = _ForestGroup[i]._MinAngleLimit;
                _ForestPreset._ForestGroup[i]._MaxAngleLimit = _ForestGroup[i]._MaxAngleLimit;
                _ForestPreset._ForestGroup[i]._HasParticles = _ForestGroup[i]._HasParticles;
                _ForestPreset._ForestGroup[i]._FaceNormals = _ForestGroup[i]._FaceNormals;
                _ForestPreset._ForestGroup[i]._DigDepth = _ForestGroup[i]._DigDepth;
                _ForestPreset._ForestGroup[i]._MinDigDepth = _ForestGroup[i]._MinDigDepth;
                _ForestPreset._ForestGroup[i]._MaxDigDepth = _ForestGroup[i]._MaxDigDepth;
                _ForestPreset._ForestGroup[i]._FaceBorderDirection = _ForestGroup[i]._FaceBorderDirection;
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
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public void IncludeAllGroups(bool state = true)
        {
            _IncludeGroup = new bool[_ForestGroup.Count];
            for (int i = 0; i < _IncludeGroup.Length; i++)
            {
                _IncludeGroup[i] = state;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckPrerequisites(int groupID, int foliageID, bool byGroup, bool paintMode, RaycastHit hitInfo = new RaycastHit())
        {
            if (_ForestPreset == null)
            {
                _ForestPreset = AssetDatabase.LoadAssetAtPath<ForestPreset>(_AssetPath + "/InternalResources/Defaults/BasicForest.asset");
            }

            _Distance = new float[_Points.Count];

            if (_ForestController == null)
            {
                if (GameObject.Find("ForesterController") != null)
                {
                    _ForestController = GameObject.Find("ForesterController").GetComponent<ForesterController>();
                }
                else
                {
                    ForesterController newController = Instantiate(AssetDatabase.LoadAssetAtPath<ForesterController>(_AssetPath + "/InternalResources/Prefabs/ForesterController.prefab"));
                    newController.name = "ForesterController";
                    _ForestController = newController;
                }
            }
            if (_CreatedObjects.Count < _ForestGroup.Count)
            {
                for (int i = _CreatedObjects.Count; i < _ForestGroup.Count; i++)
                {
                    AddNewCreatedObject(i);
                }
            }

            if (paintMode)
            {
                PaintFoliage(groupID, foliageID, hitInfo);
            }
            else
            {
                if (_ClosedEnd) CreateForest(groupID, byGroup);
                if (!_ClosedEnd) CreateForestBorder(groupID, byGroup);
            }
        }

        void AddNewCreatedObject(int groupID)
        {
            _CreatedObjects.Add(new CreatedObjects());
            _CreatedObjects[_CreatedObjects.Count-1]._GroupName = _ForestGroup[groupID]._GroupName;
            _ForestGroup[groupID]._CreatedObjectIndex = _CreatedObjects.Count - 1;
        }

        public void RemoveFoliage(int groupID, GameObject foliageObject, int foliageID = -1)
        {
            if (foliageID < 0)
            {
                for (int i = 0; i < _CreatedObjects[groupID]._FoliageInfo.Count; i++)
                {
                    if (_CreatedObjects[groupID]._FoliageInfo[i]._FoliageObject == foliageObject || _CreatedObjects[groupID]._FoliageInfo[i]._FoliageObject == foliageObject.transform.parent.gameObject)
                    {
                        foliageID = i;
                        break;
                    }
                }
            }

            if (_CreatedObjects[groupID]._FoliageInfo[foliageID]._FoliageObject == foliageObject || _CreatedObjects[groupID]._FoliageInfo[foliageID]._FoliageObject == foliageObject.transform.parent.gameObject)
            {

                //-----------------Terrain Texture Stamp------------------------------------------------------------------------------
                if (_CreatedObjects[groupID]._FoliageInfo[foliageID]._AllowStamping && _CreatedObjects[groupID]._FoliageInfo[foliageID]._StampTex != null)
                {
                    if (_CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfaceObj.GetComponent<Terrain>() != null)
                    {
                        RaycastHit hit = new RaycastHit();
                        hit.point = _CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfacePos;
                        TerrainStamping(_CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfaceObj.GetComponent<Terrain>(), hit, _CreatedObjects[groupID]._FoliageInfo[foliageID]._FoliageObject, _CreatedObjects[groupID]._FoliageInfo.Count - 1, groupID, false, true);
                    }
                }


                //-----------------Terrain Height Stamp------------------------------------------------------------------------------
                if (_CreatedObjects[groupID]._FoliageInfo[foliageID]._AllowHeight && _CreatedObjects[groupID]._FoliageInfo[foliageID]._StampTexHeight != null)
                {
                    if (_CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfaceObj.GetComponent<Terrain>() != null)
                    {
                        RaycastHit hit = new RaycastHit();
                        hit.point = _CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfacePos;
                        TerrainHeightStamping(_CreatedObjects[groupID]._FoliageInfo[foliageID]._SurfaceObj.GetComponent<Terrain>(), hit, _CreatedObjects[groupID]._FoliageInfo[foliageID]._FoliageObject, foliageID, groupID, false, true);
                    }
                }

                DestroyImmediate(_CreatedObjects[groupID]._FoliageInfo[foliageID]._FoliageObject);
                _CreatedObjects[groupID]._FoliageInfo.RemoveAt(foliageID);
            }
            SetDirty();
        }

        public void CalulateCenter()
        {
            Vector3 centroid = Vector3.zero;
            foreach (GameObject point in _Points)
            {
                centroid += point.transform.position;
            }
            _Center = centroid /= (_Points.Count);
        }

        public void PaintFoliage(int groupID, int foliageID, RaycastHit hitInfo)
        {
            if (_ForestHolder == null)
            {
                GameObject newForest = Instantiate(_ForestPrefab, _Center, _ForestPrefab.transform.rotation) as GameObject;
                newForest.name = "Forest";
                _ForestHolder = newForest;
                _ForestHolder.transform.SetParent(_ForestController.transform);
                _ForestController._ForestObjects.Add(_ForestHolder);
            }

            bool createGroup = true;
            if (_CreatedObjects[groupID]._IncludedGroup != null) createGroup = false;

            GameObject newGroup = null;
            if (createGroup)
            {
                newGroup = Instantiate(new GameObject(), _Center, _ForestHolder.transform.rotation) as GameObject;
                newGroup.name = _ForestGroup[groupID]._GroupName;
                DestroyImmediate(GameObject.Find("New Game Object"));
                newGroup.transform.SetParent(_ForestHolder.transform);
                _CreatedObjects[groupID]._IncludedGroup = newGroup;
            }
            else
            {
                newGroup = _CreatedObjects[groupID]._IncludedGroup;
            }

            GameObject newTree = null;
            if (_RetainPrefabs)
            {
                if (_ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._FoliageObject.scene.name == null)
                {
                    newTree = PrefabUtility.InstantiatePrefab(_ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._FoliageObject) as GameObject;
                }
                else
                {
                    newTree = Instantiate(_ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._FoliageObject) as GameObject;
                }
            }
            else
            {
                newTree = Instantiate(_ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._FoliageObject) as GameObject;
            }

            newTree.transform.position = hitInfo.point;
            newTree.transform.rotation = newGroup.transform.rotation;
            newTree.tag = _ForestGroup[groupID]._GroupName;
            if (newTree.transform.childCount >= 0)
            {
                foreach (Transform t in newTree.transform)
                {
                    t.tag = _ForestGroup[groupID]._GroupName;
                }
            }
            newTree.transform.SetParent(newGroup.transform);
            newTree.name = _ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._FoliageObject.name;

            if (_ForestGroup[groupID]._RandomRotation) newTree.transform.rotation = new Quaternion(newTree.transform.rotation.x, Random.Range(_ForestGroup[groupID]._MinRotation, _ForestGroup[groupID]._MaxRotation), newTree.transform.rotation.z, Random.Range(_ForestGroup[groupID]._MinRotation, _ForestGroup[groupID]._MaxRotation));

            if (_ForestGroup[groupID]._RandomScale)
            {
                float randomScale = Random.Range(_ForestGroup[groupID]._MinScale, _ForestGroup[groupID]._MaxScale);
                newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }

            _CreatedObjects[groupID]._FoliageInfo.Add(new FoliageInfo());
            int infoIndex = _CreatedObjects[groupID]._FoliageInfo.Count - 1;
            _CreatedObjects[groupID]._FoliageInfo[infoIndex]._FoliageObject = newTree;
            _CreatedObjects[groupID]._FoliageInfo[infoIndex]._SurfacePos = hitInfo.point;
            _CreatedObjects[groupID]._FoliageInfo[infoIndex]._SurfaceObj = hitInfo.transform.gameObject;

            if (hitInfo.transform.GetComponent<Terrain>() != null)
            {
                Terrain terrain = hitInfo.transform.GetComponent<Terrain>();

                //-----------------Terrain Stamp------------------------------------------------------------------------------
                if (_ForestGroup[groupID]._TerrainStamping._TerrainTex != null && _ForestGroup[groupID]._TerrainStamping._StampTex != null && _ForestGroup[groupID]._TerrainStamping._AllowStamping)
                {
                    TerrainStamping(terrain, hitInfo, newTree, _CreatedObjects[groupID]._FoliageInfo.Count - 1, groupID, false);
                }

                //-----------------Terrain Height Stamp------------------------------------------------------------------------------
                if (_ForestGroup[groupID]._TerrainStamping._StampTexHeight != null && _ForestGroup[groupID]._TerrainStamping._AllowHeight)
                {
                    TerrainHeightStamping(terrain, hitInfo, newTree, _CreatedObjects[groupID]._FoliageInfo.Count - 1, groupID);
                }
            }
            //------------------------------------------Set to normal angle----------------------------------------------------
            if (_ForestGroup[groupID]._FaceNormals)
            {
                Vector3 angle = Quaternion.FromToRotation(newTree.transform.up, hitInfo.normal).eulerAngles;
                newTree.transform.eulerAngles = angle;
                if (_ForestGroup[groupID]._RandomRotation) newTree.transform.Rotate(new Vector3(0, Random.Range(_ForestGroup[groupID]._MinRotation, _ForestGroup[groupID]._MaxRotation), 0), Space.Self);
            }

            newTree.transform.position = new Vector3(newTree.transform.position.x, hitInfo.point.y - _ForestGroup[groupID]._DigDepth, newTree.transform.position.z);

            //Particle system if applicable
            if (_ForestGroup[groupID]._HasParticles && _ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._ParticleSystem != null)
            {
                CreateParticles(newTree, groupID, foliageID);
            }

            //Create Satellite objects if applicable
            if (_ForestPreset._ForestGroup[groupID]._FoliageObjects[foliageID]._SatelliteObjects.Count > 0)
            {
                CreateSatelliteObjects(newTree, groupID, foliageID);
            }
            SetDirty(true);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        //Called to create a poly fill forest filled with trees

        public void CreateForest(int groupID, bool byGroup)
        {
            //Added if distance isn't calculated before trying to create forest and points causes array length error.
            while (_Distance.Length != _Points.Count || _Distances.Length != _Points.Count)
            {
                CalculateDistance();
            }
            //------------------------------------------------------------------------------------
            CalculateDistance();
            _Progress = 0;
            float chunkProgress = 0;

            bool exists = false; //Only really used when box cast is testing for collisions this is functionality that has only partially been added. 
            float distPercent = 0;
            if (_ForestHolder == null)
            {
                GameObject newForest = Instantiate(_ForestPrefab, _Center, _ForestPrefab.transform.rotation) as GameObject;
                newForest.name = "Forest";
                _ForestHolder = newForest;
                _ForestHolder.transform.SetParent(_ForestController.transform);
                _ForestController._ForestObjects.Add(_ForestHolder);
            }

            int includeCount = 0;
            foreach (bool inc in _IncludeGroup)
            {
                if (inc == true) includeCount++;
            }

            int groupStart = 0;
            int groupEnd = _IncludeGroup.Length - 1;
            if (byGroup)
            {
                includeCount = 1;
                groupStart = groupID;
                groupEnd = groupID;
            }

            if (!_Working)
            {
                _Working = true;
                for (int g = groupStart; g <= groupEnd; g++)
                {
                    bool createGroup = true;

                    int coIndex = GetCreatedObjectsIndex(g);

                    if (_CreatedObjects[coIndex]._IncludedGroup != null) createGroup = false;

                    GameObject newGroup = null;
                    if (createGroup)
                    {
                        newGroup = Instantiate(new GameObject(), _Center, _ForestHolder.transform.rotation) as GameObject;
                        newGroup.name = _ForestGroup[g]._GroupName;
                        DestroyImmediate(GameObject.Find("New Game Object"));
                        newGroup.transform.SetParent(_ForestHolder.transform);
                        _CreatedObjects[coIndex]._IncludedGroup = newGroup;
                    }
                    else
                    {
                        newGroup = _CreatedObjects[coIndex]._IncludedGroup;
                    }

                    if (_IncludeGroup[g])
                    {
                        for (int p = 0; p <= _Points.Count - 1; p++)
                        {
                            distPercent = 0;
                            _Distance[p] = 0;

                            while (distPercent < 100)
                            {
                                float adjustedCenter = Mathf.Lerp(1, _Distances[p], _ForestGroup[g]._Fill / 100);
                                distPercent = (_Distance[p] / adjustedCenter) * 100;

                                _Progress = ((distPercent + chunkProgress) / (_Points.Count * includeCount));
                                float normStartTime = Mathf.Clamp(_ForestGroup[g]._OffsetOverDist[0].time, 0, 900);
                                float normEndTime = Mathf.Clamp(_ForestGroup[g]._OffsetOverDist[_ForestGroup[g]._OffsetOverDist.length - 1].time, 1, 900);
                                _Distance[p] += (Mathf.Abs((Mathf.Clamp(_ForestGroup[g]._OffsetOverDist.Evaluate(Normalize(distPercent, normStartTime, normEndTime)), 0.5f, 900))) / (_ForestGroup[g]._Offset * transform.localScale.x));
                                _Distance[p] = Mathf.Clamp(_Distance[p], 0, adjustedCenter);
                                //----------------------------------------------------------------------------------------------------------------------------
                                //Create Foliage Object
                                if (_ForestPreset._ForestGroup[g]._FoliageObjects.Count != 0)
                                {
                                    int randomIndex = Random.Range(0, _ForestPreset._ForestGroup[g]._FoliageObjects.Count);
                                    if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject != null)
                                    {
                                        GameObject newTree = null;
                                        if (_RetainPrefabs)
                                        {
                                            if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject.scene.name == null)
                                            {
                                                newTree = PrefabUtility.InstantiatePrefab(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject) as GameObject;
                                            }
                                            else
                                            {
                                                newTree = Instantiate(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject) as GameObject;
                                            }
                                        }
                                        else
                                        {
                                            newTree = Instantiate(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject) as GameObject;
                                        }

                                        newTree.name = _ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject.name;
                                        CheckTagExistance(_ForestGroup[g]._GroupName);
                                        newTree.tag = _ForestGroup[g]._GroupName; //Should probably check if tag exists here
                                        if (newTree.transform.childCount >= 0)
                                        {
                                            foreach (Transform t in newTree.transform)
                                            {
                                                t.tag = _ForestGroup[g]._GroupName;
                                            }
                                        }
                                        newTree.transform.position = _Points[p].transform.position;
                                        newTree.transform.SetParent(_Points[p].transform);
                                        newTree.transform.LookAt(_Center);
                                        newTree.transform.rotation = new Quaternion(0, newTree.transform.rotation.y, 0, newTree.transform.rotation.w);
                                        newTree.transform.localPosition = new Vector3(newTree.transform.localPosition.x, newTree.transform.localPosition.y, newTree.transform.localPosition.z + _Distance[p]);
                                        float dirDecider = Random.Range(-1.0f, 1.0f);
                                        GameObject nextPoint = p == (_Points.Count - 1) ? _Points[0].gameObject : _Points[p + 1].gameObject;
                                        GameObject prevPoint = p == 0 ? _Points[_Points.Count - 1].gameObject : _Points[p - 1].gameObject;
                                        GameObject pointTarget = dirDecider <= 0 ? prevPoint : nextPoint;

                                        float spreadDist = (Vector3.Distance(_Points[p].transform.position, pointTarget.transform.position));
                                        float spread = Random.Range(0f, (spreadDist / 2));

                                        newTree.transform.LookAt(pointTarget.transform.position);
                                        newTree.transform.Translate(Random.Range(-transform.localScale.x, transform.localScale.x), 0, spread, Space.Self);
                                        newTree.transform.position = new Vector3(newTree.transform.position.x, _WorldHeight, newTree.transform.position.z);

                                        newTree.transform.SetParent(newGroup.transform);
                                        if (_ForestGroup[g]._RandomRotation) newTree.transform.rotation = new Quaternion(newTree.transform.rotation.x, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation), newTree.transform.rotation.z, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation));


                                        if (_ForestGroup[g]._RandomScale)
                                        {
                                            float randomScale = Random.Range(_ForestGroup[g]._MinScale, _ForestGroup[g]._MaxScale);
                                            newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                                        }
                                        exists = true;


                                        //------------------------------------------Filtering Stack----------------------------------------------------
                                        Vector3 castPos = newTree.transform.position;
                                        RaycastHit hit;
                                        int layerNum = LayerMask.NameToLayer("Terrain");
                                        Terrain terrain = null;
                                        if (Physics.Raycast(castPos, Vector3.down, out hit, _WorldHeight * 3, 1 << layerNum))
                                        {
                                            if (hit.transform.gameObject.layer == layerNum)
                                            {
                                                //------------------------------Angle Limit-------------------------------------------------------------------
                                                if (_ForestGroup[g]._AngleLimit > 0)
                                                {
                                                    float angle = Vector3.Angle(Vector3.up, hit.normal);
                                                    if (angle > _ForestGroup[g]._AngleLimit)
                                                    {
                                                        exists = false;
                                                    }
                                                }
                                                if (hit.transform.GetComponent<Terrain>() != null)
                                                {
                                                    terrain = hit.transform.GetComponent<Terrain>();

                                                    //------------------------------------Texture Filtering (Terrain)--------------------------------------------
                                                    exists = TextureFilterCheck(terrain, newTree, hit, g);
                                                }
                                                else
                                                {
                                                    //------------------------------------Texture Filtering (Sub Mesh)---------------------------------------------
                                                    exists = TextureFilterCheck(null, newTree, hit, g);
                                                }
                                                //------------------------------------------Set to normal angle----------------------------------------------------
                                                if (_ForestGroup[g]._FaceNormals)
                                                {
                                                    Vector3 angle = Quaternion.FromToRotation(newTree.transform.up, hit.normal).eulerAngles;
                                                    newTree.transform.eulerAngles = angle;
                                                    if (_ForestGroup[g]._RandomRotation) newTree.transform.Rotate(new Vector3(0, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation), 0), Space.Self);
                                                }
                                                newTree.transform.position = new Vector3(newTree.transform.position.x, hit.point.y - _ForestGroup[g]._DigDepth, newTree.transform.position.z);
                                            }
                                        }

                                        //------------------------------------------Stencil Filtering----------------------------------------------------
                                        exists = StencilFilterCheck(castPos, g, exists);

                                        //Checks surroundings for overlapping objects
                                        if (!_ForestPreset._ForestGroup[g]._Overlap)
                                        {
                                            foreach (FoliageInfo fg in _CreatedObjects[coIndex]._FoliageInfo)
                                            {
                                                GameObject foliageObj = fg._FoliageObject;
                                                float distance = Vector3.Distance(foliageObj.transform.position, newTree.transform.position);
                                                if (distance <= _ForestPreset._ForestGroup[g]._OverlapDistance)
                                                {
                                                    exists = false;
                                                }
                                            }
                                        }


                                        //If object cannot be grounded then remove and do not add to list
                                        if (newTree.transform.position.y >= _WorldHeight - 50)
                                        {
                                            exists = false;
                                        }

                                        if (exists)
                                        {
                                            _CreatedObjects[coIndex]._FoliageInfo.Add(new FoliageInfo());
                                            int infoIndex = _CreatedObjects[coIndex]._FoliageInfo.Count - 1;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._FoliageObject = newTree;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._SurfacePos = hit.point;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._SurfaceObj = hit.transform.gameObject;

                                            //Particle system if applicable
                                            if (_ForestGroup[g]._HasParticles && _ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._ParticleSystem != null)
                                            {
                                                CreateParticles(newTree, g, randomIndex);
                                            }

                                            //Create Satellite objects if applicable
                                            if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._SatelliteObjects.Count > 0)
                                            {
                                                CreateSatelliteObjects(newTree, g, randomIndex);
                                            }

                                            //-----------------Terrain Texture Stamp------------------------------------------------------------------------------
                                            if (hit.transform.GetComponent<Terrain>() != null)
                                            {
                                                FindDefaultTerrain(terrain);

                                                if (_ForestGroup[g]._TerrainStamping._TerrainTex != null && _ForestGroup[g]._TerrainStamping._StampTex != null && _ForestGroup[g]._TerrainStamping._AllowStamping)
                                                {
                                                    TerrainStamping(terrain, hit, newTree, _CreatedObjects[coIndex]._FoliageInfo.Count - 1, g);
                                                }

                                                //-----------------Terrain Height Stamp------------------------------------------------------------------------------
                                                if (_ForestGroup[g]._TerrainStamping._StampTexHeight != null && _ForestGroup[g]._TerrainStamping._AllowHeight)
                                                {
                                                    TerrainHeightStamping(terrain, hit, newTree, _CreatedObjects[coIndex]._FoliageInfo.Count - 1, g);
                                                }
                                            }
                                            //-------------------------------------------------------------------------------------------------------------------
                                        }
                                        else
                                        {
                                            DestroyImmediate(newTree);
                                        }
                                        if (_ShowProgressBar)
                                        {
                                            if (EditorUtility.DisplayCancelableProgressBar("Planting Forest...", Mathf.RoundToInt(_Progress) + "%", _Progress / 100))
                                            {
                                                break;
                                            }
                                        }
                                        //----------------------------------------------------------------------------------------------------------------------------
                                    }
                                }
                            }
                            chunkProgress += 100;
                        }
                        ShowHideGroups(g);
                    }
                }
            }
            _Working = false;
            if (_ShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
                GUIUtility.ExitGUI();
            }
            _Progress = 0;
            ClearUnusedCoGroups();
            SetDirty(true);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        //Called to create a straight lined forest of trees

        public void CreateForestBorder(int groupID, bool byGroup)
        {
            _Progress = 0;
            float chunkProgress = 0;

            bool exists = false; //Only really used when box cast is testing for collisions this is functionality that has only partially been added. 

            if (_ForestHolder == null)
            {
                GameObject newForest = Instantiate(_ForestPrefab, _Center, _ForestPrefab.transform.rotation) as GameObject;
                newForest.name = "Forest";
                _ForestHolder = newForest;
                _ForestHolder.transform.SetParent(_ForestController.transform);
                _ForestController._ForestObjects.Add(_ForestHolder);
            }

            int includeCount = 0;
            foreach (bool inc in _IncludeGroup)
            {
                if (inc == true) includeCount++;
            }

            int groupStart = 0;
            int groupEnd = _IncludeGroup.Length - 1;

            if (byGroup)
            {
                includeCount = 1;
                groupStart = groupID;
                groupEnd = groupID;
            }
            if (!_Working)
            {
                for (int g = groupStart; g <= groupEnd; g++)
                {
                    _Working = true;
                    bool createGroup = true;

                    int coIndex = GetCreatedObjectsIndex(g);

                    if (_CreatedObjects[coIndex]._IncludedGroup != null) createGroup = false;

                    GameObject newGroup = null;
                    if (createGroup)
                    {
                        newGroup = Instantiate(new GameObject(), _Center, _ForestHolder.transform.rotation) as GameObject;
                        newGroup.name = _ForestGroup[g]._GroupName;
                        DestroyImmediate(GameObject.Find("New Game Object"));
                        newGroup.transform.SetParent(_ForestHolder.transform);
                        _CreatedObjects[coIndex]._IncludedGroup = newGroup;
                    }
                    else
                    {
                        newGroup = _CreatedObjects[coIndex]._IncludedGroup;
                    }

                    if (_IncludeGroup[g])
                    {
                        for (int i = 0; i < _Points.Count - 1; i++)
                        {
                            float distance = 0;
                            if (_Points.Count > 0) distance = Vector3.Distance(_Points[i].transform.position, _Points[i + 1].transform.position);
                            float distanceTrack = 0;
                            while (distanceTrack < distance)
                            {

                                distanceTrack += distance / (_ForestGroup[g]._Offset * transform.localScale.x);
                                float rawDistance = (distanceTrack / distance) * 100;
                                _Progress = ((rawDistance + chunkProgress) / (_Points.Count * includeCount));

                                if (distanceTrack > distance)
                                {
                                    break;
                                }

                                if (_ForestPreset._ForestGroup[g]._FoliageObjects.Count != 0)
                                {
                                    int randomIndex = Random.Range(0, _ForestPreset._ForestGroup[g]._FoliageObjects.Count);
                                    if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject != null)
                                    {
                                        GameObject newTree = null;
                                        if (_RetainPrefabs)
                                        {
                                            if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject.scene.name == null)
                                            {
                                                newTree = PrefabUtility.InstantiatePrefab(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject) as GameObject;
                                                newTree.transform.position = _Points[i].transform.position;
                                                newTree.transform.rotation = _Points[i].transform.rotation;
                                            }
                                            else
                                            {
                                                newTree = Instantiate(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject, _Points[i].transform.position, _Points[i].transform.rotation) as GameObject;
                                            }
                                        }
                                        else
                                        {
                                            newTree = Instantiate(_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject, _Points[i].transform.position, _Points[i].transform.rotation) as GameObject;
                                        }

                                        newTree.name = _ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._FoliageObject.name;
                                        CheckTagExistance(_ForestGroup[g]._GroupName);
                                        newTree.tag = _ForestGroup[g]._GroupName;
                                        if (newTree.transform.childCount >= 0)
                                        {
                                            foreach (Transform t in newTree.transform)
                                            {
                                                t.tag = _ForestGroup[g]._GroupName;
                                            }
                                        }

                                        newTree.transform.LookAt(_Points[i + 1].transform.position);
                                        newTree.transform.Translate(new Vector3(Random.Range(-_ForestGroup[g]._BorderOffset, _ForestGroup[g]._BorderOffset), 0, distanceTrack), Space.Self);
                                        newTree.transform.position = new Vector3(newTree.transform.position.x, _WorldHeight, newTree.transform.position.z);
                                        newTree.transform.rotation = _ForestGroup[g]._FaceBorderDirection ? newTree.transform.rotation : new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w); //Border specific rotation to keep foliage object facing towards node

                                        newTree.transform.SetParent(newGroup.transform);

                                        if (_ForestGroup[g]._RandomRotation) newTree.transform.rotation = _ForestGroup[g]._FaceBorderDirection ? newTree.transform.rotation : new Quaternion(newTree.transform.rotation.x, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation), newTree.transform.rotation.z, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation));

                                        if (_ForestGroup[g]._RandomScale)
                                        {
                                            float randomScale = Random.Range(_ForestGroup[g]._MinScale, _ForestGroup[g]._MaxScale);
                                            newTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                                        }
                                        exists = true;

                                        //------------------------------------------Filtering Stack----------------------------------------------------
                                        Vector3 castPos = newTree.transform.position;
                                        RaycastHit hit;
                                        int layerNum = LayerMask.NameToLayer("Terrain");
                                        Terrain terrain = null;
                                        if (Physics.Raycast(castPos, Vector3.down, out hit, _WorldHeight * 3, 1 << layerNum))
                                        {
                                            if (hit.transform.gameObject.layer == layerNum)
                                            {
                                                //------------------------------Angle Limit-------------------------------------------------------------------
                                                if (_ForestGroup[g]._AngleLimit > 0)
                                                {
                                                    float angle = Vector3.Angle(Vector3.up, hit.normal);
                                                    if (angle > _ForestGroup[g]._AngleLimit)
                                                    {
                                                        exists = false;
                                                    }
                                                }
                                                if (hit.transform.GetComponent<Terrain>() != null)
                                                {
                                                    terrain = hit.transform.GetComponent<Terrain>();

                                                    //------------------------------------Texture Filtering (Terrain)--------------------------------------------
                                                    exists = TextureFilterCheck(terrain, newTree, hit, g);
                                                }
                                                else
                                                {
                                                    //------------------------------------Texture Filtering (Sub Mesh)---------------------------------------------
                                                    exists = TextureFilterCheck(null, newTree, hit, g);
                                                }
                                                //------------------------------------------Set to normal angle----------------------------------------------------
                                                if (_ForestGroup[g]._FaceNormals)
                                                {
                                                    Vector3 angle = Quaternion.FromToRotation(newTree.transform.up, hit.normal).eulerAngles;
                                                    newTree.transform.eulerAngles = angle;
                                                    if (_ForestGroup[g]._RandomRotation) newTree.transform.Rotate(new Vector3(0, Random.Range(_ForestGroup[g]._MinRotation, _ForestGroup[g]._MaxRotation), 0), Space.Self);
                                                }
                                                newTree.transform.position = new Vector3(newTree.transform.position.x, hit.point.y - _ForestGroup[g]._DigDepth, newTree.transform.position.z);
                                            }
                                        }

                                        //------------------------------------------Stencil Filtering----------------------------------------------------
                                        exists = StencilFilterCheck(castPos, g, exists);

                                        //If object cannot be grounded then remove and do not add to list
                                        if (newTree.transform.position.y >= _WorldHeight - 50)
                                        {
                                            exists = false;
                                        }

                                        if (exists)
                                        {
                                            _CreatedObjects[coIndex]._FoliageInfo.Add(new FoliageInfo());
                                            int infoIndex = _CreatedObjects[coIndex]._FoliageInfo.Count - 1;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._FoliageObject = newTree;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._SurfacePos = hit.point;
                                            _CreatedObjects[coIndex]._FoliageInfo[infoIndex]._SurfaceObj = hit.transform.gameObject;

                                            //Particle system if applicable
                                            if (_ForestGroup[g]._HasParticles && _ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._ParticleSystem != null)
                                            {
                                                CreateParticles(newTree, g, randomIndex);
                                            }

                                            //Create Satellite objects if applicable
                                            if (_ForestPreset._ForestGroup[g]._FoliageObjects[randomIndex]._SatelliteObjects.Count > 0)
                                            {
                                                CreateSatelliteObjects(newTree, g, randomIndex);
                                            }


                                            //-----------------Terrain Stamp------------------------------------------------------------------------------
                                            if (hit.transform.GetComponent<Terrain>() != null)
                                            {
                                                FindDefaultTerrain(terrain);

                                                if (_ForestGroup[g]._TerrainStamping._TerrainTex != null && _ForestGroup[g]._TerrainStamping._StampTex != null && _ForestGroup[g]._TerrainStamping._AllowStamping)
                                                {
                                                    exists = TerrainStamping(terrain, hit, newTree, _CreatedObjects[coIndex]._FoliageInfo.Count - 1, g);
                                                }

                                                //-----------------Terrain Height Stamp------------------------------------------------------------------------------
                                                if (_ForestGroup[g]._TerrainStamping._StampTexHeight != null && _ForestGroup[g]._TerrainStamping._AllowHeight)
                                                {
                                                    TerrainHeightStamping(terrain, hit, newTree, _CreatedObjects[coIndex]._FoliageInfo.Count - 1, g);
                                                }
                                            }
                                            //-------------------------------------------------------------------------------------------------------------------
                                        }
                                        else
                                        {
                                            DestroyImmediate(newTree);
                                        }
                                        if (_ShowProgressBar)
                                        {
                                            if (EditorUtility.DisplayCancelableProgressBar("Planting Forest...", Mathf.RoundToInt(_Progress) + "%", _Progress / 100))
                                            {
                                                break;
                                            }
                                        }
                                        //----------------------------------------------------------------------------------------------------------------------------
                                    }
                                }
                            }
                            chunkProgress += 100;
                        }
                        ShowHideGroups(g);
                    }
                }
            }
            _Working = false;
            if (_ShowProgressBar)
            {
                EditorUtility.ClearProgressBar();
                GUIUtility.ExitGUI();
            }
            _Progress = 0;
            ClearUnusedCoGroups();
            SetDirty(true);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------

        public void CreateParticles(GameObject parent, int group, int obj)
        {
            Vector3 spawnPos = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
            ParticleSystem newParticleSys = Instantiate(_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._ParticleSystem, spawnPos, _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._ParticleSystem.transform.rotation) as ParticleSystem;
            newParticleSys.transform.SetParent(parent.transform);
            newParticleSys.transform.localPosition = new Vector3(0, _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._ParticleSystemHeight, 0);
            ParticleSystem.VelocityOverLifetimeModule setVelocity = newParticleSys.velocityOverLifetime;
            setVelocity.space = ParticleSystemSimulationSpace.World;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------

        public void CreateSatelliteObjects(GameObject parent, int group, int obj)
        {
            if (_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects.Count > 0)
            {
                for (int s = 0; s < _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects.Count; s++)
                {
                    for (int i = 0; i < Random.Range(0, _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._MaxNum); i++)
                    {
                        Vector3 startPos = new Vector3(parent.transform.position.x, _WorldHeight, parent.transform.position.z);
                        GameObject clone = Instantiate(_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Object, startPos, _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Object.transform.rotation);
                        float distance = _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._SatelliteSpread;
                        clone.transform.SetParent(parent.transform);
                        clone.transform.localPosition = new Vector3(Random.Range(-distance, distance), clone.transform.position.y, Random.Range(-distance, distance));

                        float yPos = 0;

                        RaycastHit hit;
                        int layerNum = LayerMask.NameToLayer("Terrain");
                        if (Physics.Raycast(clone.transform.position, Vector3.down, out hit, _WorldHeight * 3, 1 << layerNum))
                        {
                            if (hit.transform.gameObject.layer == layerNum)
                            {
                                if (_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._FaceNormals)
                                {
                                    Vector3 angle = Quaternion.FromToRotation(clone.transform.up, hit.normal).eulerAngles;
                                    clone.transform.eulerAngles = angle;
                                    if (_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Rotation) clone.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0), Space.Self);
                                }

                                yPos = hit.point.y + 0.01f;
                            }
                        }
                        if (_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Hover)
                        {
                            yPos = yPos + _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._YPos;
                        }

                        clone.transform.position = new Vector3(clone.transform.position.x, yPos, clone.transform.position.z);

                        if (_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Scale)
                        {
                            float scale = Random.Range(_ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._MinScale, _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._MaxScale);
                            clone.transform.localScale = new Vector3(scale, scale, scale);
                        }

                        clone.name = _ForestPreset._ForestGroup[group]._FoliageObjects[obj]._SatelliteObjects[s]._Object.name;
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        public void ResetAssetPath()
        {
            _AssetPath = null;

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
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        void SetDirty(bool saveAssets = false)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            if (saveAssets) AssetDatabase.SaveAssets();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        void ClearAllPoints()
        {
            foreach (GameObject node in _Points)
            {
                DestroyImmediate(node);
            }
            _Points.Clear();
            SetDirty();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        void CreatePoint(GameObject point, Vector3 pos)
        {
            GameObject newPoint = Instantiate(point, pos, transform.rotation) as GameObject;
            _Points.Add(newPoint);

            newPoint.transform.SetParent(transform);
            newPoint.name = ("Node" + (_Points.Count - 1).ToString());
            newPoint.GetComponent<PointNode>().id = _Points.Count - 1;
            newPoint.GetComponent<PointNode>().forester = this;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public void SetScaleAndRot(bool scale = false, bool rotate = false, float minScale = 1, float maxScale = 1, float minRotation = 0, float maxRotation = 360)
        {
            //Set Scale and rotation if active
            if (scale)
            {
                float newScale = Random.Range(minScale, maxScale);
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }

            if (rotate)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(minRotation, maxRotation), transform.eulerAngles.z);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public void Rebuild(bool movePivot = false)
        {
            Vector3 pivotPos = gameObject.transform.position;

            Vector3 scale = transform.localScale;

            transform.localScale = Vector3.one;

            //Rebuild node list using child order to repair issue when upgrading to v.1.3.4 **TO BE REMOVED IN V.1.4***

            if (_Points.Count == 0)
            {
                PointNode[] nodes = GetComponentsInChildren<PointNode>();
                foreach (PointNode pn in nodes)
                {
                    _Points.Add(pn.gameObject);
                }
            }
            //------------------------------------------------------------------------------

            if (_AreaPreset != null)
            {
                _ClosedEnd = _AreaPreset._CloseEnd;
                ClearAllPoints();

                for (int i = 0; i < _AreaPreset._Positions.Count; i++)
                {
                    CreatePoint(_Point, gameObject.transform.TransformPoint(_AreaPreset._Positions[i]));
                }
            }

            //Correct position
            foreach (GameObject go in _Points)
            {
                go.transform.SetParent(null);
            }

            //Recalculate Center
            CalulateCenter();
            gameObject.transform.position = _Center;
            //-------------------------------------------------------------------------------

            foreach (GameObject go in _Points)
            {
                go.transform.SetParent(gameObject.transform);
            }
            if(movePivot)gameObject.transform.position = pivotPos;
            transform.localScale = scale; // Reset scale to default 
            //Set to world contours
            float lastHeight = 0;
            foreach (GameObject go in _Points)
            {
                go.transform.SetSiblingIndex(go.GetComponent<PointNode>().id); //Uses opportunity to clean up child index

                go.transform.position = new Vector3(go.transform.position.x, _WorldHeight, go.transform.position.z);

                RaycastHit hit;

                int layerNum = LayerMask.NameToLayer("Terrain");

                if (Physics.Raycast(go.transform.position, Vector3.down, out hit, _WorldHeight * 3, 1 << layerNum))
                {
                    if (hit.transform.gameObject.layer == layerNum)
                    {
                        go.transform.position = new Vector3(go.transform.position.x, hit.point.y, go.transform.position.z);
                        lastHeight = hit.point.y;
                    }
                }
                else
                {
                    go.transform.position = new Vector3(go.transform.position.x, lastHeight, go.transform.position.z);
                }
            }
        }

        public void CheckTagExistance(string tagname)
        {
            //Check if group name exists as tag for assignment
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool exists = false;
            for (int n = 0; n < tagsProp.arraySize; n++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(n);
                if (t.stringValue == tagname)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                SerializedProperty tag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                tag.stringValue = tagname;
                tagManager.ApplyModifiedProperties();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void Deforestation(bool byGroup = false, int SelectedGroup = -1,bool clearOnly = false)
        {
            if (_ForestController != null)
            {
                _ForestController.gameObject.SetActive(false);
                _ForestController._FoliageObjects.Clear();
            }

            float expectedNodeCount = 0;
            float rawProgress = 0;

            if (_ForestGroup.Count > 0 && byGroup && SelectedGroup >= 0)
            {
                if (_CreatedObjects.Count > 0)
                {
                    if (SelectedGroup < _CreatedObjects.Count)
                    {
                        int coIndex = GetCreatedObjectsIndex(SelectedGroup);

                        if (_CreatedObjects[coIndex]._FoliageInfo.Count > 0)
                        {
                            expectedNodeCount = _CreatedObjects[coIndex]._FoliageInfo.Count;
                            for (int i = 0; i < _CreatedObjects[coIndex]._FoliageInfo.Count; i++)
                            {
                                bool delete = true;
                                for (int n = 0; n < _FrozenObjects.Count; n++)
                                {
                                    if (_FrozenObjects[n]._FoliageObject == _CreatedObjects[coIndex]._FoliageInfo[i]._FoliageObject || _FrozenObjects[n]._Parent == _CreatedObjects[coIndex]._FoliageInfo[i]._FoliageObject)
                                        delete = false;
                                }
                                if (delete)
                                {
                                    RemoveFoliage(coIndex, _CreatedObjects[coIndex]._FoliageInfo[i]._FoliageObject, i);
                                    i = i - 1;
                                }
                                rawProgress += 1;
                                float progress = (rawProgress / expectedNodeCount) * 100;
                                if (_ShowProgressBar)
                                {
                                    if (EditorUtility.DisplayCancelableProgressBar("Clearing Forest...", Mathf.RoundToInt(progress) + "%", progress / 100))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (_ShowProgressBar) EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                if (_CreatedObjects.Count > 0)
                {
                    ClearAllCreated(_CreatedObjects, "Clearing Forest...");
                }

                if (_ShowProgressBar) EditorUtility.ClearProgressBar();
            }
            if (_ForestController != null)
            {
                _ForestController.gameObject.SetActive(true);
            }
            ClearUnusedCoGroups();
            if (clearOnly && _ShowProgressBar) GUIUtility.ExitGUI();
            SetDirty(true);
        }

        void ClearAllCreated(List<CreatedObjects> createdObjects,string message)
        {
            float expectedNodeCount = 0;
            float rawProgress = 0;

            //Progress precalculation
            for (int g = 0; g < _CreatedObjects.Count; g++)
            {
                expectedNodeCount += _CreatedObjects[g]._FoliageInfo.Count;
            }

            for (int g = 0; g < _CreatedObjects.Count; g++)
            {
                for (int i = 0; i < _CreatedObjects[g]._FoliageInfo.Count; i++)
                {
                    bool delete = true;
                    for (int n = 0; n < _FrozenObjects.Count; n++)
                    {
                        if (_FrozenObjects[n]._FoliageObject == _CreatedObjects[g]._FoliageInfo[i]._FoliageObject || _FrozenObjects[n]._Parent == _CreatedObjects[g]._FoliageInfo[i]._FoliageObject)
                        {
                            delete = false;
                        }
                    }
                    if (delete)
                    {
                        RemoveFoliage(g, _CreatedObjects[g]._FoliageInfo[i]._FoliageObject, i);
                        i = i - 1;
                    }
                    rawProgress += 1;
                    float progress = (rawProgress / expectedNodeCount) * 100;
                    if (_ShowProgressBar)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar(message, Mathf.RoundToInt(progress) + "%", progress / 100))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void ShowHideGroups(int groupID)
        {
            if (_CreatedObjects.Count > 0)
            {
                if (groupID < _CreatedObjects.Count)
                {
                    int coIndex = GetCreatedObjectsIndex(groupID);
                    if (_CreatedObjects[coIndex]._FoliageInfo.Count > 0)
                    {
                        GameObject groupGo = _CreatedObjects[coIndex]._FoliageInfo[0]._FoliageObject.transform.parent.transform.gameObject;
                        groupGo.SetActive(_IncludeGroup[groupID]);
                    }
                }
            }
        }

        int GetCreatedObjectsIndex(int groupID)
        {
            //Find created objects index for this group
            int coIndex = 0;
            bool exists = false;
            for (int i = 0; i < _CreatedObjects.Count; i++)
            {
                if (_CreatedObjects[i]._GroupName == _ForestGroup[groupID]._GroupName)
                {
                    coIndex = i;
                    exists = true;
                    i = _CreatedObjects.Count;
                }
            }

            if(!exists)
            {
                AddNewCreatedObject(groupID);
                coIndex = _CreatedObjects.Count - 1;
            }
            return coIndex;
        }

        void ClearUnusedCoGroups()
        {
            for(int i = 0; i < _CreatedObjects.Count;i++)
            {
                bool exists = false;
                if (_CreatedObjects[i]._FoliageInfo.Count > 0) return;
                for(int g = 0; g < _ForestGroup.Count;g++)
                {
                    if (_CreatedObjects[i]._GroupName == _ForestGroup[g]._GroupName)
                    {
                        exists = true;
                        if (exists) exists = false;
                    }
                }
                if (!exists)
                {
                    if (_CreatedObjects[i]._IncludedGroup != null) DestroyImmediate(_CreatedObjects[i]._IncludedGroup);
                    _CreatedObjects.RemoveAt(i);
                    i = i - 1;
                }
            }
        }

        public float Normalize(float value, float min, float max)
        {
            float normalized = (value - min) / (max - min);
            return normalized;
        }

        public float Denormalize(float value, float min, float max)
        {
            float denormalized = (value * (max - min) + min);
            return denormalized;
        }

        bool TextureFilterCheck(Terrain terrain, GameObject foliageObject, RaycastHit hit, int g)
        {
            bool exists = true;
            if (_ForestGroup[g]._TargetTextures.Count > 0)
            {
                if (terrain != null)
                {
                    TerrainLayer[] splatTxArray;
                    splatTxArray = terrain.terrainData.terrainLayers;
                    string textureName = splatTxArray[_TerrainGrab.TerrainTargetTex(terrain, foliageObject.transform.position)].diffuseTexture.name;
                    exists = false;
                    foreach (Texture2D tx in _ForestGroup[g]._TargetTextures)
                    {
                        if (tx != null)
                        {
                            CheckFilterTexValid(tx);
                            if (textureName == tx.name) exists = true;
                        }
                    }
                }
                else
                {
                    MeshRenderer mr = hit.transform.GetComponent<MeshRenderer>();
                    Vector2 texCoord = hit.textureCoord;
                    int submeshIndex = 0;
                    MeshFilter mf = hit.transform.GetComponent<MeshFilter>();
                    int[] hittedTriangle = new int[] { mf.sharedMesh.triangles[Mathf.Abs(hit.triangleIndex * 3)], mf.sharedMesh.triangles[Mathf.Abs(hit.triangleIndex * 3 + 1)], mf.sharedMesh.triangles[Mathf.Abs(hit.triangleIndex * 3 + 2)] };

                    for (int s = 0; s < mf.sharedMesh.subMeshCount; s++)
                    {
                        int[] subMeshTris = mf.sharedMesh.GetTriangles(s);
                        for (int t = 0; t < subMeshTris.Length; t += 3)
                        {
                            if (subMeshTris[t] == hittedTriangle[0] && subMeshTris[t + 1] == hittedTriangle[1] && subMeshTris[t + 2] == hittedTriangle[2])
                            {
                                submeshIndex = s;
                                break;
                            }
                        }
                    }

                    Texture2D mainTex = mr.sharedMaterials[submeshIndex].mainTexture as Texture2D;
                    if (mainTex == null)
                    {
                        exists = true;
                    }
                    else
                    {
                        CheckFilterTexValid(mainTex);
                        Color[] colorGrab = new Color[5];
                        colorGrab[0] = mainTex.GetPixelBilinear(texCoord.x, texCoord.y);
                        colorGrab[1] = mainTex.GetPixelBilinear(texCoord.x, texCoord.y + 0.25f);
                        colorGrab[2] = mainTex.GetPixelBilinear(texCoord.x, texCoord.y - 0.25f);
                        colorGrab[3] = mainTex.GetPixelBilinear(texCoord.x + 0.25f, texCoord.y);
                        colorGrab[4] = mainTex.GetPixelBilinear(texCoord.x - 0.25f, texCoord.y);
                        bool[] txCheck = new bool[5];

                        foreach (Texture2D tx in _ForestGroup[g]._TargetTextures)
                        {
                            if (tx == null)
                            {
                                //Ignore and continue as true
                            }
                            else
                            {
                                CheckFilterTexValid(tx);
                                exists = true;
                                txCheck[0] = tx.GetPixelBilinear(texCoord.x, texCoord.y) == colorGrab[0] ? true : false;
                                txCheck[1] = tx.GetPixelBilinear(texCoord.x, texCoord.y + 0.25f) == colorGrab[1] ? true : false;
                                txCheck[2] = tx.GetPixelBilinear(texCoord.x, texCoord.y - 0.25f) == colorGrab[2] ? true : false;
                                txCheck[3] = tx.GetPixelBilinear(texCoord.x + 0.25f, texCoord.y) == colorGrab[3] ? true : false;
                                txCheck[4] = tx.GetPixelBilinear(texCoord.x - 0.25f, texCoord.y) == colorGrab[4] ? true : false;
                                foreach (bool value in txCheck)
                                {
                                    if (!value)
                                    {
                                        exists = false;
                                    }
                                }
                            }
                            if (exists == true) break;
                        }
                    }
                }
            }
            return exists;
        }

        public bool TerrainStamping(Terrain terrain, RaycastHit hit, GameObject foliageObject, int foliageIndex, int g, bool performFilterCheck = true, bool clearLayer = false)
        {
            bool exists = true;
            int targetLayer = 0;

            List<TerrainLayer> splatTxList = new List<TerrainLayer>();
            for (int n = 0; n < terrain.terrainData.terrainLayers.Length; n++)
            {
                splatTxList.Add(terrain.terrainData.terrainLayers[n]);
            }

            bool txExists = false;

            for (int s = 0; s < splatTxList.Count; s++)
            {
                if (splatTxList[s].diffuseTexture == _ForestGroup[g]._TerrainStamping._TerrainTex)
                {
                    txExists = true;
                    targetLayer = s;
                }
            }
            if (!txExists)
            {
                splatTxList.Add(new TerrainLayer());
                splatTxList[splatTxList.Count - 1].diffuseTexture = _ForestGroup[g]._TerrainStamping._TerrainTex;
                splatTxList[splatTxList.Count - 1].tileSize = _ForestGroup[g]._TerrainStamping._TileSize;
                targetLayer = splatTxList.Count - 1;
            }

            if (performFilterCheck && !clearLayer)
            {
                exists = TextureFilterCheck(terrain, foliageObject, hit, g);
            }

            if (exists)
            {
                terrain.terrainData.terrainLayers = splatTxList.ToArray();

                Color32[] stampPixelList = new Color32[1];

                if (!clearLayer)
                {
                    if (_ForestGroup[g]._TerrainStamping._StampTex == null) return false;
                    stampPixelList = _ForestGroup[g]._TerrainStamping._StampTex.GetPixels32();
                    FindDefaultTerrain(terrain);
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._AllowStamping = true;
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTex = _ForestGroup[g]._TerrainStamping._StampTex;
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampSize = _ForestGroup[g]._TerrainStamping._StampSize;
                    _TerrainGrab.SetTerrainColor(terrain, targetLayer, _ForestGroup[g]._TerrainStamping._StampSize, _ForestGroup[g]._TerrainStamping._StampOpacity, _ForestGroup[g]._TerrainStamping._AllowStampOverlap, hit.point, stampPixelList, clearLayer);
                }
                else
                {
                    //Stamp check
                    if (_CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTex == null) return false;
                    stampPixelList = _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTex.GetPixels32();

                    if (_CreatedObjects.Count > g)
                    {
                        TerrainData terrainData = FindDefaultTerrain(terrain);
                        _TerrainGrab.ResetSplatColors(terrain, _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampSize, hit.point, stampPixelList, terrainData);
                    }
                }
            }
            return exists;
        }

        TerrainData FindDefaultTerrain(Terrain terrain)
        {
            bool tExists = false;
            TerrainSurfaceData ts = new TerrainSurfaceData();
            for(int i = 0; i < _ForestController._TerrainSurfaceData.Count;i++)
            { 
                if (_ForestController._TerrainSurfaceData[i]._TerrainName == terrain.terrainData.name)
                {
                    if (_ForestController._TerrainSurfaceData[i]._RawTerrainData != null)
                    {
                        tExists = true;
                        ts = _ForestController._TerrainSurfaceData[i];
                        return ts._RawTerrainData as TerrainData;
                    }
                    else
                    {
                        _ForestController._TerrainSurfaceData.RemoveAt(i);
                    }
                }
            }

            if (!tExists)
            {
                _ForestController._TerrainSurfaceData.Add(new TerrainSurfaceData());
                _ForestController._TerrainSurfaceData[_ForestController._TerrainSurfaceData.Count - 1]._TerrainName = terrain.terrainData.name;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(terrain.terrainData), _AssetPath + "/Data/Terrains/" + terrain.terrainData.name + ".asset");
                AssetDatabase.Refresh();
                ts._RawTerrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(_AssetPath + "/Data/Terrains/" + terrain.terrainData.name + ".asset");
                _ForestController._TerrainSurfaceData[_ForestController._TerrainSurfaceData.Count - 1]._RawTerrainData = ts._RawTerrainData;
                return ts._RawTerrainData as TerrainData;
            }
            return null;
        }

        public bool TerrainHeightStamping(Terrain terrain, RaycastHit hit, GameObject foliageObject, int foliageIndex, int g, bool performFilterCheck = false, bool clearLayer = false)
        {
            bool exists = true;

            if (performFilterCheck && !clearLayer)
            {
                exists = TextureFilterCheck(terrain, foliageObject, hit, g);
            }

            if (exists)
            {
                TerrainData terrainData = FindDefaultTerrain(terrain);
                Color32[] stampPixelList = new Color32[1];
                if (!clearLayer)
                {
                    if (_ForestGroup[g]._TerrainStamping._StampTexHeight == null) return false;
                    stampPixelList = _ForestGroup[g]._TerrainStamping._StampTexHeight.GetPixels32();
                    float heightStrength = Random.Range(_ForestGroup[g]._TerrainStamping._MinHeightStrength, _ForestGroup[g]._TerrainStamping._MaxHeightStrength);
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._AllowHeight = _ForestGroup[g]._TerrainStamping._AllowHeight;
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTexHeight = _ForestGroup[g]._TerrainStamping._StampTexHeight;
                    _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampSizeHeight = _ForestGroup[g]._TerrainStamping._StampSizeHeight;
                    _TerrainGrab.SetTerrainHeight(terrain, _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampSizeHeight, heightStrength, hit.point, stampPixelList,terrainData);
                }
                else
                {
                    if (_CreatedObjects.Count > g)
                    {
                        if (terrain != null && _CreatedObjects[g]._FoliageInfo[foliageIndex]._AllowHeight)
                        {
                            if (_CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTexHeight == null) return false;
                            stampPixelList = _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampTexHeight.GetPixels32();
                            _TerrainGrab.ResetHeight(terrain, _CreatedObjects[g]._FoliageInfo[foliageIndex]._StampSizeHeight + 1, hit.point, stampPixelList, terrainData);
                        }
                    }
                }
            }
            return exists;
        }

        public void ForestOverlap()
        {
            ForesterTool[] forestTools = Resources.FindObjectsOfTypeAll(typeof(ForesterTool)) as ForesterTool[];
            for (int co = 0; co < this._CreatedObjects.Count; co++)
            {
                for (int fi = 0; fi < _CreatedObjects[co]._FoliageInfo.Count; fi++)
                {
                    for (int t = 0; t < forestTools.Length; t++)
                    {
                        if (forestTools[t] == this) return;
                        for (int g = 0; g < forestTools[t]._CreatedObjects.Count; g++)
                        {
                            for (int f = 0; f < forestTools[t]._CreatedObjects[g]._FoliageInfo.Count; f++)
                            {
                                if (Vector3.Distance(_CreatedObjects[co]._FoliageInfo[fi]._FoliageObject.transform.position, forestTools[t]._CreatedObjects[g]._FoliageInfo[f]._FoliageObject.transform.position) < forestTools[t]._ForestGroup[g]._OverlapDistance)
                                {
                                    RemoveFoliage(co, _CreatedObjects[co]._FoliageInfo[fi]._FoliageObject, fi);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            SetDirty(true);
        }

        public void CheckFilterTexValid(Texture2D tex)
        {
            try
            {
                tex.GetPixel(0, 0);
            }
            catch (UnityException e)
            {
                if (e.Message.StartsWith("Texture '" + tex.name + "' is not readable"))
                {
                    string path = AssetDatabase.GetAssetPath(tex);
                    TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
                    ti.isReadable = true;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        bool StencilFilterCheck(Vector3 castPos,int g,bool exists)
        {
            //------------------------------------------Stencil Filtering----------------------------------------------------
            if (_StencilMasks.Count > 0)
            {
                RaycastHit stencilHit;
                bool existsInStencil = false;
                bool outsideBorder = false;
                bool insideBorder = false;
                int inactiveCount = 0;
                int stencilLayerNum = LayerMask.NameToLayer("ForestStencil");

                for (int s = 0; s < _StencilMasks.Count; s++)
                {
                    if (!_ForestGroup[g]._StencilMaskState[s]) inactiveCount++;
                }

                if (inactiveCount < _StencilMasks.Count)
                {
                    for (int s = 0; s < _StencilMasks.Count; s++)
                    {
                        StencilMask stencilMask = _StencilMasks[s];
                        if (_ForestGroup[g]._StencilMaskState[s])
                        {
                            if (stencilMask._StencilObject != null)
                            {
                                stencilMask._StencilObject.layer = stencilLayerNum;
                                if (Physics.Raycast(castPos, Vector3.down, out stencilHit, _WorldHeight * 3, 1 << stencilLayerNum))
                                {
                                    if (stencilHit.transform.gameObject.layer == stencilLayerNum)
                                    {
                                        if (stencilHit.transform.GetComponent<MeshRenderer>() != null)
                                        {
                                            MeshRenderer renderer = stencilHit.transform.GetComponent<MeshRenderer>();
                                            Texture2D tex = renderer.sharedMaterial.mainTexture as Texture2D;
                                            Color sample = tex.GetPixelBilinear(stencilHit.textureCoord.x, stencilHit.textureCoord.y);
                                            if (exists && !existsInStencil)
                                            {
                                                existsInStencil = sample != Color.black ? true : false;
                                            }
                                            insideBorder = true;
                                        }
                                    }
                                }
                                else
                                {
                                    outsideBorder = true;
                                }
                                stencilMask._StencilObject.layer = 0;
                            }
                            else
                            {
                                existsInStencil = exists;
                            }
                        }
                    }
                }
                else
                {
                    existsInStencil = true;
                }
                if (_ForestGroup[g]._StencilExcludeState && outsideBorder && !existsInStencil && !insideBorder)
                {
                    existsInStencil = true;
                }
                return existsInStencil;
            }
            else
            {
                return exists;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
#endif
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class StencilMask
    {
        public GameObject _StencilObject;
        public Texture _StencilTex;
        public bool _IgnorePastBorder = true;
    }

    [System.Serializable]
    public class OffsetOverDistance
    {
        public string name;
        public float OffsetOverDist;
        //is percentage of overall distance
        public int startDist = 0;
        public int endDist = 100;
    }

    [System.Serializable]
    public class CreatedObjects
    {
        public string _GroupName;
        public GameObject _IncludedGroup;
        public List<FoliageInfo> _FoliageInfo = new List<FoliageInfo>();

        // Obsolete! To be removed in V1.4! Now embedded into FoliageInfo
        public List<GameObject> _FoliageObjects = new List<GameObject>();
    }

    [System.Serializable]
    public class FrozenObjects
    {
        public int _Index = 0;
        public GameObject _Parent;
        public GameObject _FoliageObject;
        public Material[] _Materials;
    }

    [System.Serializable]
    public class FoliageInfo
    {
        public GameObject _FoliageObject;
        public Vector3 _SurfacePos;
        public bool _AllowStamping = false;
        public Texture2D _StampTex;
        public int _StampSize = 0;
        public bool _AllowHeight = false;
        public Texture2D _StampTexHeight;
        public int _StampSizeHeight = 0;
        public GameObject _SurfaceObj;
    }

    public class MultidimentionalFloatX2
    {
        public float[] ConvertToArray(float[,] data)
        {
            if (data == null) return null;
            List<float> newArray = new List<float>();
            for (int x = 0; x < Mathf.Sqrt(data.Length); x++)
            {
                for (int y = 0; y < Mathf.Sqrt(data.Length); y++)
                {
                    newArray.Add(data[x, y]);
                }
            }
            return newArray.ToArray();
        }

        public float[,] ConvertFromArray(float[] data)
        {
            if (data == null) return null;
            float[,] newArray = new float[(int)Mathf.Sqrt(data.Length), (int)Mathf.Sqrt(data.Length)];
            int index = 0;
            for (int x = 0; x < Mathf.Sqrt(data.Length); x++)
            {
                for (int y = 0; y < Mathf.Sqrt(data.Length); y++)
                {
                    newArray[x, y] = data[index];
                    index++;
                }
            }
            return newArray;
        }
    }

    public class MultidimentionalFloatX3
    {
        public float[] ConvertToArray(float[,,] data, TerrainData terrainData)
        {
            if (data == null) return null;
            List<float> newArray = new List<float>();
            for (int n = 0; n < terrainData.terrainLayers.Length; n++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    for (int y = 0; y < data.GetLength(1); y++)
                    {
                        newArray.Add(data[x, y, n]);
                    }
                }
            }
            return newArray.ToArray();
        }

        public float[,,] ConvertFromArray(float[] data, TerrainData terrainData)
        {
            if (data == null) return null;
            float[,,] newArray = new float[(int)Mathf.Sqrt(data.Length / terrainData.terrainLayers.Length), (int)Mathf.Sqrt(data.Length / terrainData.terrainLayers.Length), terrainData.terrainLayers.Length];
            int index = 0;
            for (int n = 0; n < terrainData.terrainLayers.Length; n++)
            {
                for (int x = 0; x < Mathf.Sqrt(data.Length / terrainData.terrainLayers.Length); x++)
                {
                    for (int y = 0; y < Mathf.Sqrt(data.Length / terrainData.terrainLayers.Length); y++)
                    {
                        newArray[x, y, n] = data[index];
                        index++;
                    }
                }
            }
            return newArray;
        }
    }
}

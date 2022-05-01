using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forester
{
    [CreateAssetMenu(fileName = "NewForestPreset", menuName = "Forester/New Forest Preset")]
    public class ForestPreset : ScriptableObject
    {
        [SerializeField]
        public List<ForestGroup> _ForestGroup = new List<ForestGroup>();

    }

    [System.Serializable]
    public class PrefabType
    {
        public GameObject _Prefab;
        public float _ScaleMin = 0.75f;
        public float _ScaleMax = 1;

        [Header("Add a custom particle system")]
        public ParticleSystem _ParticleSystem;
        public float _ParticleSystemHeight = 4;
    }

    [System.Serializable]
    public class ForestGroup
    {
        public Texture2D _Icon;
        public string _GroupName;
        public float _MinOffset = 0.01f;
        public float _MaxOffset = 1;
        public float _Offset = 1; // density
        public float _Fill = 50; // 0 is default
        public float _MinBorderOffset = 0.01f;
        public float _MaxBorderOffset = 1;
        public float _BorderOffset = 1;
        public float _MinDigDepth = 0;
        public float _MaxDigDepth = 1;
        public float _DigDepth = 0;
        public bool _FaceBorderDirection = false;
        public bool _RandomRotation = false;
        public float _MinRotation = 0;
        public float _MaxRotation = 360;
        public bool _RandomScale = false;
        public float _MinScale = 0.5f;
        public float _MaxScale = 1;
        public float _MinAngleLimit = 0;
        public float _MaxAngleLimit = 90;
        public float _AngleLimit = 0;
        public bool _FaceNormals = false;
        public bool _HasParticles = false;
        public int _TagIndex = 0;
        public bool _Overlap = true;
        public float _MinOverlapDistance = 0;
        public float _MaxOverlapDistance = 5.0f;
        public float _OverlapDistance = 5.0f;
        public float _MinPaintFreq = 0f;
        public float _MaxPaintFreq = 1f;
        public float _PaintFreq = 0.5f;
        public int _UniqueID = 0;
        public List<Texture2D> _TargetTextures = new List<Texture2D>();
        public List<bool> _StencilMaskState = new List<bool>();
        public bool _StencilExcludeState = false;
        public int _CreatedObjectIndex = -1;
        public List<FoliageObjects> _FoliageObjects = new List<FoliageObjects>();
        public List<OffsetOverDistance> _OffsetOverDistance = new List<OffsetOverDistance>();
        public AnimationCurve _OffsetOverDist = new AnimationCurve();
        //--------------------------Terrain Stamping-----------------------------------------
        public TerrainStamping _TerrainStamping = new TerrainStamping();
        //-----------------------------------------------------------------------------------
    }

    [System.Serializable]
    public class FoliageObjects
    {
        public GameObject _FoliageObject;
        public ParticleSystem _ParticleSystem;
        public float _ParticleSystemHeight;
        public List<SatelliteObjects> _SatelliteObjects = new List<SatelliteObjects>();
    }

    [System.Serializable]
    public class SatelliteObjects
    {
        public GameObject _Object;
        public int _MaxNum = 0;
        public bool _FaceNormals = true;
        public bool _Rotation = true;
        public bool _Scale = true;
        public float _MinScale = 0.5f;
        public float _MaxScale = 1;
        public bool _Hover = false;
        public float _YPos = 0;
        public float _SatelliteSpread = 2.0f;
    }
    [System.Serializable]
    public class TerrainStamping
    {
        public bool _AllowStamping = false;
        public Texture2D _TerrainTex;
        public Texture2D _StampTex;
        public int _StampSize = 5;
        public int _StampOpacity = 100;
        public Vector2 _TileSize = new Vector2(15, 15);
        public bool _AllowStampOverlap = true;

        public bool _AllowHeight = false;
        public Texture2D _StampTexHeight;
        public int _StampSizeHeight = 5;
        public float _MinHeightStrength = 0;
        public float _MaxHeightStrength = 1;
    }
}


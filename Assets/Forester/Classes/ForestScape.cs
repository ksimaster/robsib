using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forester
{
    [ExecuteInEditMode]
    public class ForestScape : MonoBehaviour {

        public ForestScapePreset _ForestScapePreset;
        public string _PresetName = "<Insert Preset Name>";

        public string _CurrentPreset;

        public List<bool> _AreaInclude = new List<bool>();
        public List<bool> _ForestInclude = new List<bool>();

        public bool _DensityRandom = false;
        public bool _OffsetRandom = false;
        public bool _OverlapFoliage = true;

        public float _MinFill = 0;
        public float _MaxFill = 3;
        public float _Fill = 2.5f;

        public float _MinOffset = 0;
        public float _MaxOffset = 30;
        public float _Offset = 20;

        public float _MinRotation = 0;
        public float _MaxRotation = 360;

        public float _MinScale = 1;
        public float _MaxScale = 1.25f;

        public float _DistanceExpansion = 100;

        public GameObject _TargetedObject;

        public List<AreaPresetType> _AreaPresets = new List<AreaPresetType>();
        public List<ForestPresetType> _ForestPresets = new List<ForestPresetType>();
        public List<Texture2D> _TargetTextures = new List<Texture2D>();

        public List<GameObject> _ForestTools = new List<GameObject>();

        public string _AssetPath;


        void Start()
        {
         _MinFill = 0;
        _MaxFill = 3;
        _Fill = 2.5f;

        _MinOffset = 0;
        _MaxOffset = 30;
        _Offset = 20;

        _MinRotation = 0;
        _MaxRotation = 360;

        _MinScale = 1;
        _MaxScale = 1.25f;

        _DistanceExpansion = 100;
    }

    }
    [System.Serializable]
    public class AreaPresetType
    {
        public AreaPreset _AreaPreset;
        public bool _Include = false;
    }

    [System.Serializable]
    public class ForestPresetType
    {
        public ForestPreset _ForestPreset;
        public bool _Include = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forester
{
    [CreateAssetMenu(fileName = "NewForestScape", menuName = "Forester/New ForestScape Preset")]
    public class ForestScapePreset : ScriptableObject
    {

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
        public float _Rotation = 0;

        public float _MinScale = 1;
        public float _MaxScale = 1.25f;
        public float _Scale = 0;

        public List<AreaPresetType> _AreaPresets = new List<AreaPresetType>();
        public List<ForestPresetType> _ForestPresets = new List<ForestPresetType>();
        public List<Texture2D> _TargetTexture = new List<Texture2D>();
    }
}

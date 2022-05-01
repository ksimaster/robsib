using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forester
{
    [CreateAssetMenu(fileName = "NewAreaPreset", menuName = "Forester/New Area Preset")]
    public class AreaPreset : ScriptableObject
    {
        public List<Rect> _Windows = new List<Rect>();
        public List<Vector3> _Positions = new List<Vector3>();
        public bool _CloseEnd = true;
    }
}
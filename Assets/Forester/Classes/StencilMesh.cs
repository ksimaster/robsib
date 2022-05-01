using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Forester
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class StencilMesh : MonoBehaviour
    {
        [HideInInspector]
        public MeshFilter _MeshFilter;
        [HideInInspector]
        public Mesh _Mesh;
        [HideInInspector]
        public Vector3 _LastPosition;
        [HideInInspector]
        public Vector3 _LastRotation;
        [HideInInspector]
        public Vector3 _LastScale;
        [HideInInspector]
        public float _LastHeight = 0;
        [HideInInspector]
        public float _Subdivisions = 1;
        public float _MaxResolution = 128;      
        public float _DistanceFromSurface = 0.2f;
        [HideInInspector]
        public Texture _Tex;
        // Use this for initialization
        void OnEnable()
        {
            _MeshFilter = GetComponent<MeshFilter>();
            _Mesh = new Mesh();
            _MeshFilter.mesh = _Mesh;
        }
    }
}

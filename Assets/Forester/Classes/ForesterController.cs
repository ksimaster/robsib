using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Forester
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ForesterController : MonoBehaviour
    {
        public enum WindMode
        {
            Global,
            Windzones,
        }

        public WindMode _WindMode = WindMode.Global;

        public enum WindDirections
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest,
        }

        public WindDirections _WindDirection = WindDirections.North;

        public float _GlMain = 0.75f;
        public float _GlTurbulance = 0.75f;
        public float _GlPulseMag = 1.0f;
        public float _GlPulseFreq = 1.0f;
        private Vector3 _GlWindDir;

        public WindZone[] _WindZones;
        [Space(10)]

        [HideInInspector]
        public List<GameObject> _ForestObjects = new List<GameObject>();
        [HideInInspector]
        public List<ControllerObjects> _FoliageObjects = new List<ControllerObjects>();
        [HideInInspector]
        public List<TerrainSurfaceData> _TerrainSurfaceData = new List<TerrainSurfaceData>();

        void Start()
        {
            if (_WindZones.Length != 0) _WindMode = WindMode.Global; //Set default

            SetWindDirection(_WindDirection,_WindZones);

            if(Application.isPlaying)
            {
                foreach(ControllerObjects obj in _FoliageObjects)
                {
                    Material[] mats = obj.foliageObject.GetComponent<Renderer>().sharedMaterials;
                    if (_WindMode != WindMode.Global)
                    {
                        mats = obj.foliageObject.GetComponent<Renderer>().materials;
                    }

                   for(int i = 0; i < mats.Length;i++)
                    {
                        mats[i] = new Material(mats[i]);
                    }
                }
                UpdateForestObjectsList();
            }
        }

        public void UpdateForestObjectsList()
        {
            _FoliageObjects.Clear();
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach(Transform child in children)
            {
                if(child.GetComponent<Renderer>() != null && !child.GetComponent<ParticleSystem>())
                {
                    Material[] mats = child.GetComponent<Renderer>().sharedMaterials;
                    //Check if valid object
                    bool added = false;
                    foreach (Material mat in mats)
                    {
                        if(mat.shader.name == "Forester/FoliageMovement")
                        {
                            if (!added)
                            {
                                _FoliageObjects.Add(new ControllerObjects());
                                _FoliageObjects[_FoliageObjects.Count - 1].foliageObject = child.gameObject;
                                _FoliageObjects[_FoliageObjects.Count - 1].particleSystems = child.GetComponentsInChildren<ParticleSystem>();
                                added = true;
                            }
                             _FoliageObjects[_FoliageObjects.Count - 1].materials.Add(mat);
                        }
                    }
                }
            }
            SetWindDirection(_WindDirection, _WindZones);
        }

        //Temporary function to clear up issues be removed in v2.0;
        public void CleanupForestList()
        {
            for(int i = 0; i < _ForestObjects.Count;i++)
            {
                if(_ForestObjects[i] == null)
                {
                    _ForestObjects.RemoveAt(i);
                    i = 0;
                }
            }
        }

        public void SetWindDirection(WindDirections direction, WindZone[] windZones)
        {
            switch (direction)
            {
                case WindDirections.North:
                    _GlWindDir = new Vector3(0, 0, _GlMain);
                    break;

                case WindDirections.NorthEast:
                    _GlWindDir = new Vector3(_GlMain, 0, _GlMain);
                    break;

                case WindDirections.East:
                    _GlWindDir = new Vector3(_GlMain, 0, 0);
                    break;

                case WindDirections.SouthEast:
                    _GlWindDir = new Vector3(_GlMain, 0, -_GlMain);
                    break;

                case WindDirections.South:
                    _GlWindDir = new Vector3(0, 0, -_GlMain);
                    break;

                case WindDirections.SouthWest:
                    _GlWindDir = new Vector3(-_GlMain, 0, -_GlMain);
                    break;

                case WindDirections.West:
                    _GlWindDir = new Vector3(-_GlMain, 0, 0);
                    break;

                case WindDirections.NorthWest:
                    _GlWindDir = new Vector3(-_GlMain, 0, _GlMain);
                    break;
            }

            if (_WindMode == WindMode.Global)
            {
                for (int i = 0; i < _FoliageObjects.Count; i++)
                {
                    Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
                    Vector3 worldPos = -m.MultiplyPoint3x4(_GlWindDir);

                    _FoliageObjects[i].windDir = worldPos;
                    _FoliageObjects[i].turbulance = _GlTurbulance;

                    UpdateFoliageConditions(i, _FoliageObjects[i].materials.ToArray(), worldPos, _GlTurbulance,_GlPulseFreq, _GlPulseMag, _FoliageObjects[i].particleSystems);
                }
            }

                if (_WindMode == WindMode.Windzones)
            {
                for (int wz = 0; wz < windZones.Length; wz++)
                {
                    if (windZones[wz].mode == WindZoneMode.Directional)
                    {
                        for (int i = 0; i < _FoliageObjects.Count; i++)
                        {
                            if (wz == 0) _FoliageObjects[i].windzones.Clear();

                            _FoliageObjects[i].windzones.Add(windZones[wz]); //Adds new wind influence

                            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
                            Vector3 worldPos = m.MultiplyPoint3x4(windZones[wz].transform.forward);
                            float turbulance = windZones[wz].windTurbulence;
                            float pulseFreq = windZones[wz].windPulseFrequency;
                            float pulseMag = windZones[wz].windPulseMagnitude;
                            float randomTurbulance = Random.Range(turbulance / 2, turbulance);
                            Vector3 _WindDirVector = worldPos * windZones[wz].windMain;

                            if (_FoliageObjects[i].windzones.Count > 1)
                            {
                                //Blend with current influence here
                                _WindDirVector = new Vector3(Mathf.Max(_WindDirVector.x, _FoliageObjects[i].windDir.x), Mathf.Max(_WindDirVector.y, _FoliageObjects[i].windDir.y), Mathf.Max(_WindDirVector.z, _FoliageObjects[i].windDir.z));
                                turbulance = (Mathf.Max(turbulance, _FoliageObjects[i].turbulance));
                            }

                            _FoliageObjects[i].windDir = _WindDirVector;
                            _FoliageObjects[i].turbulance = turbulance;

                            UpdateFoliageConditions(i, _FoliageObjects[i].materials.ToArray(), _WindDirVector, randomTurbulance, pulseFreq, pulseMag, _FoliageObjects[i].particleSystems);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _FoliageObjects.Count; i++)
                        {

                            float distance = Vector3.Distance(windZones[wz].transform.position, _FoliageObjects[i].foliageObject.transform.position);

                            if (distance < windZones[wz].radius)
                            {
                                if (wz == 0) _FoliageObjects[i].windzones.Clear();

                                _FoliageObjects[i].windzones.Add(windZones[wz]); //Adds new wind influence

                                Vector3 newPos = _FoliageObjects[i].foliageObject.transform.position - windZones[wz].transform.position;
                                float turbulance = windZones[wz].windTurbulence;
                                float pulseFreq = windZones[wz].windPulseFrequency;
                                float pulseMag = windZones[wz].windPulseMagnitude;
                                float randomTurbulance = Random.Range(turbulance / 2, turbulance);
                                Vector3 _WindDirVector = newPos / 10 * windZones[wz].windMain;

                                if (_FoliageObjects[i].windzones.Count > 1)
                                {
                                    //Blend with current influence here
                                    _WindDirVector = new Vector3(Mathf.Max(_WindDirVector.x, _FoliageObjects[i].windDir.x), Mathf.Max(_WindDirVector.y, _FoliageObjects[i].windDir.y), Mathf.Max(_WindDirVector.z, _FoliageObjects[i].windDir.z));
                                    turbulance = (Mathf.Max(turbulance, _FoliageObjects[i].turbulance));
                                }

                                _FoliageObjects[i].windDir = _WindDirVector;
                                _FoliageObjects[i].turbulance = turbulance;

                                UpdateFoliageConditions(i, _FoliageObjects[i].materials.ToArray(), _WindDirVector, randomTurbulance, pulseFreq, pulseMag, _FoliageObjects[i].particleSystems);
                            }
                            else if(_FoliageObjects[i].windzones.Count == 0)
                            {
                                Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
                                Vector3 worldPos = m.MultiplyPoint3x4(_GlWindDir);

                                _FoliageObjects[i].windDir = worldPos;
                                _FoliageObjects[i].turbulance = _GlTurbulance;

                                UpdateFoliageConditions(i, _FoliageObjects[i].materials.ToArray(), worldPos, _GlTurbulance, _GlPulseFreq, _GlPulseMag, _FoliageObjects[i].particleSystems);
                            }
                        }
                    }
                }
            }
            CleanupForestList(); //Temporary function to clear up issues be removed in v2.0;
        }

        public void UpdateFoliageConditions(int index, Material[] mats, Vector3 windDirection, float speed,float pulseFreq,float pulseMag,ParticleSystem[] particles)
        {
            foreach (Material mat in mats)
            {
                mat.SetVector("_Offset", new Vector4(windDirection.x, windDirection.y, windDirection.z, 0));
                mat.SetFloat("_Speed", speed);
                mat.SetFloat("_PulseFrequency", pulseFreq);
                mat.SetFloat("_PulseMagnitude", pulseMag);
            }

            foreach (ParticleSystem particleSystem in particles)
            {
                ParticleSystem.VelocityOverLifetimeModule setVelocity = particleSystem.velocityOverLifetime;
                setVelocity.x = windDirection.x;
                setVelocity.y = windDirection.y;
                setVelocity.z = windDirection.z;
                setVelocity.space = ParticleSystemSimulationSpace.World;
            }

        }
    }
    [System.Serializable]
    public class ControllerObjects
    {
        public GameObject foliageObject;
        public ParticleSystem[] particleSystems;
        public List<Material> materials = new List<Material>();
        public List<WindZone> windzones = new List<WindZone>();
        public Vector3 windDir;
        public float turbulance;
    }

    [System.Serializable]
    public class TerrainSurfaceData
    {
        public string _TerrainName;
        public Object _RawTerrainData;
    }
}
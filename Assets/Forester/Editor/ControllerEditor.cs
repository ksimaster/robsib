using UnityEngine;
using UnityEditor;

namespace Forester
{
    [CustomEditor(typeof(ForesterController))]
    [System.Serializable]
    public class ControllerEditor : Editor
    {
        ForesterController t;

        public override void OnInspectorGUI()
        {
            t = target as ForesterController;

            DrawDefaultInspector();

            GUILayout.Space(5);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
            GUILayout.Space(5);

            if(GUILayout.Button("Update Foliage Objects"))
            {
                t.UpdateForestObjectsList();
            }

            if (GUILayout.Button("Set Wind Properties"))
            {
                t.UpdateForestObjectsList();
                t.SetWindDirection(t._WindDirection,t._WindZones);
            }

        }
    }
}
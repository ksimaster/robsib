using UnityEngine;

namespace Forester
{
    [ExecuteInEditMode]
    [System.Serializable] // make sticky
    public class PointNode : MonoBehaviour
    {
        public int id;
        public ForesterTool forester;
        void Update()
        {
            transform.LookAt(forester._Center);
            if (transform.name != "Node" + id)
            {
                transform.name = "Node" + id;
                transform.SetSiblingIndex(id);
            }
        }
    }
}
using UnityEngine;

public class Adv : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test JS");
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLPluginJS.CallFunction();
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;

public class Adv : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Test JS");
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLPluginJS.CallFunction();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

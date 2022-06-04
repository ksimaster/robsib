using UnityEngine;
using UnityEngine.UI;

public class GenerateWarm : MonoBehaviour
{
    public Slider sliderObject;
    public float generateValue;
    public float timeOut;

    private void Awake()
    {
        PlayerPrefs.SetInt(PlayerConstants.TreeHome, 0);
    }

    private void Start()
    {
        //countResource = PlayerPrefs.GetInt(nameResource);
    }
}

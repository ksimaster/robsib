using UnityEngine;
using UnityEngine.UI;

public class GenerateWarm : MonoBehaviour
{
    public Slider sliderObject;
    public float generateValue;
    public float timeOut;
    private int cntr;

    private void Awake()
    {
        PlayerPrefs.SetInt(PlayerConstants.TreeHome, 0);
    }

    private void Start()
    {
        //countResource = PlayerPrefs.GetInt(nameResource);
    }

    private void FixedUpdate()
    {
        cntr = (cntr + 1) % 10;
        if (cntr % 10 != 0)
        {
            return;
        }

        var countResource = PlayerPrefs.GetInt(PlayerConstants.TreeHome);
        if (countResource > 0)
        {
            countResource--;
            PlayerPrefs.SetInt(PlayerConstants.TreeHome, countResource);
            sliderObject.value += generateValue;
        }
    }
}

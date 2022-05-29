using UnityEngine;
using UnityEngine.UI;

public class GenerateFuel : MonoBehaviour
{
    public Slider sliderObject;
    public float generateValue;
    public string nameResource;
    public float timeOut;
    private float fuel;
    // public float behindMax;

    private float timer = 0.0f;
    private float waitTime = 2.0f;

    // public float fuelExpenses;

    private void Awake()
    {
        // PlayerPrefs.SetInt(nameResource, 1);
    }
    private void Start()
    {
        //countResource = PlayerPrefs.GetInt(nameResource);
    }
    private void FixedUpdate()
    {
        var countResource = PlayerPrefs.GetInt(nameResource);
        while (countResource > 0)
        {
            countResource--;
            PlayerPrefs.SetInt(nameResource, countResource);
            sliderObject.value += generateValue;
        }
    }
}

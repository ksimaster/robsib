using UnityEngine;
using UnityEngine.UI;

public class GenerateFuel : MonoBehaviour
{
    public string nameResource;
    public Slider sliderFuelInHouse;
    public Slider sliderFuelInCar;

    private void Awake()
    {
        PlayerPrefs.SetInt(PlayerConstants.OreHome, 0);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
    
    }
}

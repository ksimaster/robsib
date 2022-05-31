using UnityEngine;
using UnityEngine.UI;

public class GenerateFuel : MonoBehaviour
{
    public string nameResource;
    public Slider sliderFuelInHouse;
    public Slider sliderFuelInCar;
    private int cntr;

    private void Awake()
    {
        PlayerPrefs.SetInt(PlayerConstants.OreHome, 0);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        cntr = (cntr + 1) % 10;
        if (cntr % 10 != 0)
        {
            return;
        }

        var countOre = PlayerPrefs.GetInt(PlayerConstants.OreHome);
        if (countOre > 0)
        {
            countOre--;
            PlayerPrefs.SetInt(PlayerConstants.OreHome, countOre);
            sliderFuelInHouse.value += PlayerConstants.FuelGenerateValue;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class UnloadInHome : MonoBehaviour
{
    public string homeTag;
    //public Text[] textCountResource;
    private string[] nameCarResources = { "TreeCar", "OreCar" };
    private string[] nameHomeResources = { PlayerConstants.TreeHome, PlayerConstants.OreHome };
    private string tag;

    public Slider sliderFuelInHouse;
    public Slider sliderFuelInCar;

    private void OnCollisionEnter(Collision col)
    {
        this.tag = col.gameObject.tag;
    }

    private void OnCollisionExit(Collision col)
    {
        this.tag = null;
    }

    void FixedUpdate()
    {
        if (tag == null)
        {
            return;
        }

        if (tag.Equals(homeTag))
        {
            for (int i = 0; i < 2; i++)
            {
                var car = PlayerPrefs.GetInt(nameCarResources[i]); // выводим из префа, что хранится в плэеере в переменную
                var home = PlayerPrefs.GetInt(nameHomeResources[i]); // выводим из префа, что хранится в доме-складе в переменную
                if (car > 0)
                {
                    PlayerPrefs.SetInt(nameCarResources[i], car - 1);
                    PlayerPrefs.SetInt(nameHomeResources[i], home + 1);
                }
            }


            var fuelHome = PlayerPrefs.GetFloat(PlayerConstants.Fuel);
            if (fuelHome > 0)
            {
                var freeTankVolume = PlayerConstants.MaxFuelCar - sliderFuelInCar.value;
                var fuelToPour = Math.Min(Math.Min(freeTankVolume, fuelHome), 0.2f);
                sliderFuelInHouse.value = fuelHome - fuelToPour;
                sliderFuelInCar.value += fuelToPour;
            }
        }
    }

}

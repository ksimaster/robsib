using System;
using UnityEngine;
using UnityEngine.UI;

public class UnloadInHome : MonoBehaviour
{
    private const int DefaultDelay = 10;
    private const int DefaultActionDelay = 3;
    public string homeTag;
    //public Text[] textCountResource;
    private string[] nameCarResources = { "TreeCar", "OreCar" };
    private string[] nameHomeResources = { PlayerConstants.TreeHome, PlayerConstants.OreHome };
    private string tag;
    public Slider sliderFuelInHouse;
    public Slider sliderFuelInCar;
    public Slider sliderWarmHome;
    private int delay = 0;
    private bool isAtHome = false;
    private int actionDelay = 0;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.Equals(homeTag))
        {
            tag = col.gameObject.tag;
            isAtHome = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag.Equals(homeTag))
        {
            delay = DefaultDelay;
            isAtHome = false;
        }
    }

    void FixedUpdate()
    {
        if (ShoudWait())
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

            var fuelHome = sliderFuelInHouse.value;
            
            if (fuelHome > 0)
            {
                var freeTankVolume = PlayerConstants.MaxFuelCar - sliderFuelInCar.value;
                var fuelToPour = Math.Min(Math.Min(freeTankVolume, fuelHome), 0.15f);
                sliderFuelInHouse.value = fuelHome - fuelToPour;
                sliderFuelInCar.value += fuelToPour;
            }

            var countOre = PlayerPrefs.GetInt(PlayerConstants.OreHome);
            if (countOre > 0)
            {
                countOre--;
                PlayerPrefs.SetInt(PlayerConstants.OreHome, countOre);
                sliderFuelInHouse.value += PlayerConstants.FuelGenerateValue;
            }

            var treeResource = PlayerPrefs.GetInt(PlayerConstants.TreeHome);
            if (treeResource > 0)
            {
                treeResource--;
                PlayerPrefs.SetInt(PlayerConstants.TreeHome, treeResource);
                sliderWarmHome.value += PlayerConstants.WarmGenerateValue;
            }
        }
    }

    private bool ShoudWait()
    {
        if (tag == null)
        {
            return true;
        }

        if (!isAtHome)
        {
            if (delay == 0)
            {
                tag = null;
                return true;
            }
            else
            {
                delay--;
            }
        }

        if (actionDelay != 0)
        {
            actionDelay--;
            return true;
        }
        else
        {
            actionDelay = DefaultActionDelay;
        }

        return false;
    }
}

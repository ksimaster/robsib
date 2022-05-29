using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnloadInHome : MonoBehaviour
{
    public string homeTag;
    public int[] countCarResources;
    public int[] countHomeResources;
    //public Text[] textCountResource;
    private string[] nameCarResources = { "TreeCar", "OreCar" };
    private string[] nameHomeResources = { PlayerConstants.TreeHome, PlayerConstants.OreHome };
    private float fuelCount;
    public Slider sliderFuelInHouse;
    public Slider sliderFuelInCar;
     

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(homeTag))
        {
            for (int i = 0; i < countCarResources.Length; i++)
            {
                countCarResources[i] = PlayerPrefs.GetInt(nameCarResources[i]); // выводим из префа, что хранится в плэеере в переменную
                countHomeResources[i] = PlayerPrefs.GetInt(nameHomeResources[i]); // выводим из префа, что хранится в доме-складе в переменную
                PlayerPrefs.SetInt(nameCarResources[i], 0); //обнуляем переменную в префе плэеера
                PlayerPrefs.SetInt(nameHomeResources[i], countCarResources[i] + countHomeResources[i]); // записываем в преф склада переменную равную сумме переменных склада и плэера 
                //textCountResource[i].text = countResources[i].ToString();
            }

            fuelCount = sliderFuelInHouse.value;
            var freeTankVolume = PlayerConstants.MaxFuelCar - sliderFuelInCar.value;
            var fuelToPour = Math.Min(freeTankVolume, fuelCount);
            sliderFuelInHouse.value -= fuelToPour;
            sliderFuelInCar.value += fuelToPour;
        }
    }
}

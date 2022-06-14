using System;
using UnityEngine;
using UnityEngine.UI;

public class FuelFlow : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    public Slider sliderFuelInCar;

    private int cntr = 0;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerConstants.FuelInCar))
        {
            sliderFuelInCar.value = PlayerPrefs.GetFloat(PlayerConstants.FuelInCar);
        }
    }

    private void FixedUpdate()
    {
        SaveProgress();

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            var path = (float)Math.Sqrt(horizontal * horizontal + vertical * vertical);
            sliderFuelInCar.value -= path * PlayerConstants.FuelConsumption;
            if (sliderFuelInCar.value <= 0)
            {
                return;
            }
        }
    }

    private void SaveProgress()
    {
        cntr = (++cntr) % 100;
        if (cntr == 1)
        {
            PlayerPrefs.SetFloat(PlayerConstants.FuelInCar, sliderFuelInCar.value);
        }
    }
}

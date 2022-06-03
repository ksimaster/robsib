using System;
using UnityEngine;
using UnityEngine.UI;

public class FuelFlow : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    public Slider sliderFuelInCar;

    private void FixedUpdate()
    {
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
        

}

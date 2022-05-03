using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFuel : MonoBehaviour
{
    public Slider sliderOut;
    public Text sliderText;
    public float speedFrost;
    public float behindMax;
    public float deadValue;
    public string nameValue;

    private void Awake()
    {
        sliderOut.value = sliderOut.maxValue - behindMax;
        PlayerPrefs.SetFloat("Fuel", sliderOut.value);
    }
    private void Start()
    {
        sliderOut.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("");
        });
        
        
    }
    private void FixedUpdate()
    {
        
        sliderOut.value -= speedFrost;
        PlayerPrefs.SetFloat("Fuel", sliderOut.value);

        if (sliderOut.value <= deadValue) Debug.Log("Михалыч погиб! Вы проиграли!");
        
    }

}

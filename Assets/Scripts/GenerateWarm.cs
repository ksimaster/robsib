using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateWarm : MonoBehaviour
{
    public Slider sliderObject;
    public float generateValue;
    public string nameResource;
    public float timeOut;
    private float fuel;
    // public float behindMax;

    private int countResource;
    private bool isGenerate = false;
    private float timer = 0.0f;
    private float waitTime = 2.0f;

    //public float warmExpenses;

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
        //timer += Time.deltaTime;
        countResource = PlayerPrefs.GetInt(nameResource);
        while (countResource > 0)
        {

            GenerateResource();
        }
    }

   public void GenerateResource()
    {
        countResource--;
        PlayerPrefs.SetInt(nameResource, countResource);
        sliderObject.value = sliderObject.value + generateValue;
        fuel = PlayerPrefs.GetFloat("Fuel");  
        PlayerPrefs.SetFloat("Fuel", fuel-generateValue);
    }
}

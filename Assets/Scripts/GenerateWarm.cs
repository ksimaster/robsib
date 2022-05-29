using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateWarm : MonoBehaviour
{
    public Slider sliderObject;
    public float generateValue;
    public float timeOut;
    private float fuel;
    // public float behindMax;
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
        var countResource = PlayerPrefs.GetInt(PlayerConstants.TreeHome);
        while (countResource > 0)
        {
            countResource--;
            PlayerPrefs.SetInt(PlayerConstants.TreeHome, countResource);
            sliderObject.value += generateValue;
        }
    }
}

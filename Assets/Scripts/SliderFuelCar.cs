using UnityEngine;
using UnityEngine.UI;

public class SliderFuelCar : MonoBehaviour
{
    public Slider slider;
    public Text sliderText;
    public float behindMax;
    public float deadValue;
    private float fuelSpendSpeed = PlayerConstants.FuelSpendIdle;

    private void Awake()
    {
        slider.value = slider.maxValue - behindMax;
        UpdateFuelCount();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.00");
            UpdateFuelCount();
        });
    }

    void FixedUpdate()
    {
        slider.value -= fuelSpendSpeed;
        if (slider.value <= deadValue) Debug.Log("Остановить машину");
    }

    private void UpdateFuelCount()
    {
        PlayerPrefs.SetFloat(PlayerConstants.FuelCar, slider.value);
    }
}

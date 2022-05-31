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
    }

    private void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.00");
        });
    }

    void FixedUpdate()
    {
        slider.value -= fuelSpendSpeed;
        if (slider.value <= deadValue) Debug.Log("Остановить машину");
    }
}

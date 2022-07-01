using UnityEngine;
using UnityEngine.UI;

public class SliderFuelCar : MonoBehaviour
{
    public Slider slider;
    public Text sliderText;
    public float behindMax;
    public float deadValue;
    public GameObject panelDeath;
    private float fuelSpendSpeed = PlayerConstants.FuelSpendIdle;

    private void Awake()
    {
        slider.value = slider.maxValue - behindMax;
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.00");
        });
    }

    private void Start()
    {

    }

    void FixedUpdate()
    {
        slider.value -= fuelSpendSpeed;
        if (slider.value <= deadValue)
        {
            Debug.Log("Кончилось топливо");
            Time.timeScale = 0;
            panelDeath.SetActive(true);
        }
            
    }
}

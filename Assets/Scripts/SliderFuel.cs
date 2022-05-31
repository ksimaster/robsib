using UnityEngine;
using UnityEngine.UI;

public class SliderFuel : MonoBehaviour
{
    public Slider sliderOut;
    public Text sliderText;
    public float behindMax;
    public string nameValue;

    private void Awake()
    {
        sliderOut.value = sliderOut.maxValue - behindMax;
    }

    private void Start()
    {
        sliderOut.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.");
            UpdateFuelCount();
        });           
    }

    private void UpdateFuelCount()
    {
        PlayerPrefs.SetFloat(PlayerConstants.Fuel, sliderOut.value);
    }
}

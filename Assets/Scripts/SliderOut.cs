using UnityEngine;
using UnityEngine.UI;

public class SliderOut : MonoBehaviour
{
    public Slider sliderOut;
    public Slider sliderSibling;

    public Text sliderText;
    public float behindMax;

    public string nameValue;
    public GameObject PanelDead;

    private void Awake()
    {
        sliderOut.value = sliderOut.maxValue - behindMax;
        PlayerPrefs.SetFloat(nameValue, sliderOut.value);
    }

    private void Start()
    {
        sliderOut.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.00");
        });        
    }

    private void FixedUpdate()
    {
        if (sliderSibling.value <= sliderSibling.minValue)
        {
            return;
        }

        if (sliderOut.name == "SliderOut" && (sliderOut.value - sliderOut.minValue) < (sliderSibling.value - sliderSibling.minValue))
        {
            sliderOut.value -= PlayerConstants.SpeedUpFrost;
        }
        else
        {
            sliderOut.value -= PlayerConstants.SpeedFrost;
        }

        if (sliderOut.value <= sliderOut.minValue) {
            PanelDead.SetActive(true);
        }
    }
}

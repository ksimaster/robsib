using UnityEngine;
using UnityEngine.UI;

public class SliderFuel : MonoBehaviour
{
    public Slider sliderOut;
    public Text sliderText;
    public float behindMax;
    public string nameValue;
    
    private int cntr = 0;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerConstants.Fuel))
        {
            sliderOut.value = PlayerPrefs.GetFloat(PlayerConstants.Fuel);
            sliderText.text = sliderOut.value.ToString("0.");
        }
        else
        {
            sliderOut.value = sliderOut.maxValue - behindMax;
        }
    }

    private void Start()
    {
        sliderOut.onValueChanged.AddListener((v) =>
        {
            SaveProgress();
            sliderText.text = v.ToString("0.");
        });           
    }

    private void SaveProgress()
    {
        cntr = (++cntr) % 100;
        if (cntr == 1)
        {
            PlayerPrefs.SetFloat(PlayerConstants.Fuel, sliderOut.value);
        }
    }
}

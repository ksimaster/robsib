using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    private const float DefaultMusicFloat = 0.5f;
    private const float DefaultsoundEffectsFloat = 0.25f;

    public const string MusicPref = "MusicPref";
    public const string SoundEffectsPref = "SoundEffectsPref";
    public Slider musicSlider, soundEffectsSlider;
    public AudioSource musicAudio;
    public AudioSource[] soundEffectsAudio;
    // Start is called before the first frame update
    
    void Awake()
    {
        if(!PlayerPrefs.HasKey(MusicPref))
        {
            musicSlider.value = DefaultMusicFloat;
            soundEffectsSlider.value = DefaultsoundEffectsFloat;
            PlayerPrefs.SetFloat(MusicPref, DefaultMusicFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, DefaultsoundEffectsFloat);
        }
        else
        {
            musicSlider.value = PlayerPrefs.GetFloat(MusicPref);
            soundEffectsSlider.value = PlayerPrefs.GetFloat(SoundEffectsPref);
        }
    }

    public void UpdateMusic()
    {
        musicAudio.volume = musicSlider.value;
        PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
    }

    public void UpdateEffects()
    {
        for (int i = 0; i < soundEffectsAudio.Length; i++)
        {
            soundEffectsAudio[i].volume = soundEffectsSlider.value;
        }
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsSlider.value);
    }
}
    
    
        
    


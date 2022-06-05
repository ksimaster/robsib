using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    private const float DefaultMusicFloat = 0.5f;
    private const float DefaultsoundEffectsFloat = 0.25f;

    private static readonly string MusicPref = "MusicPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";
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
            soundEffectsSlider.value = PlayerPrefs.GetFloat(SoundEffectsPref);
            musicSlider.value = PlayerPrefs.GetFloat(MusicPref);
        }
    }

    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsSlider.value);
        PlayerPrefs.Save();
    }

    public void UpdateSound()
    {
        musicAudio.volume = musicSlider.value;

        for(int i = 0; i < soundEffectsAudio.Length; i++)
        {
            soundEffectsAudio[i].volume = soundEffectsSlider.value;
        }

        SaveSoundSettings();
    }  
}
    
    
        
    


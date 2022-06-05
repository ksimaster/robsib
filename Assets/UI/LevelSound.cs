using UnityEngine;

public class LevelSound : MonoBehaviour
{
    public AudioSource musicAudio;
    public AudioSource[] soundEffectsAudio;

    private void Start()
    {
        LevelSoundSettings();
    }

    private void LevelSoundSettings()
    {
        var musicFloat = PlayerPrefs.GetFloat(VolumeManager.MusicPref);
        var soundEffectsFloat = PlayerPrefs.GetFloat(VolumeManager.SoundEffectsPref);

        musicAudio.volume = musicFloat;

        for(int i = 0; i < soundEffectsAudio.Length; i++)
        {
            soundEffectsAudio[i].volume = soundEffectsFloat;
        }
    }
}

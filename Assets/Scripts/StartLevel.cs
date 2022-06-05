using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    private readonly string[] StoredKeys = new string[] { VolumeManager.SoundEffectsPref, VolumeManager.MusicPref };
    
    // Start is called before the first frame update
    void Awake()
    {
        // Store some prefs between sessions.
        var storedDict = new Dictionary<string, float>();
        foreach (var key in StoredKeys) 
        {
            if (PlayerPrefs.HasKey(key))
            {
                storedDict.Add(key, PlayerPrefs.GetFloat(key));
            }
        }

        PlayerPrefs.DeleteAll();
        foreach (var key in storedDict.Keys)
        {
            PlayerPrefs.SetFloat(key, storedDict[key]);
        }
    }
}

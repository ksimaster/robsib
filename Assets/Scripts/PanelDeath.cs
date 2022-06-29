using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PanelDeath : MonoBehaviour
{
    private readonly string[] StoredFloatKeys = new string[] { VolumeManager.SoundEffectsPref, VolumeManager.MusicPref };

    private readonly string[] StoredIntKeys = new string[] { PlayerConstants.IsEducationComplete, PlayerConstants.MoveMode };

    private readonly string[] StoredStringKeys = new string[] { };

    public void RestartLevel()
    {
        DeletePrefs();
        SceneManager.LoadScene("MaineScene");
    }


    public void DeletePrefs()
    {
        // Store some prefs between sessions.
        var storedDict = new Dictionary<string, object>();
        foreach (var key in StoredFloatKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                storedDict.Add(key, PlayerPrefs.GetFloat(key));
            }
        }

        foreach (var key in StoredIntKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                storedDict.Add(key, PlayerPrefs.GetInt(key));
            }
        }

        foreach (var key in StoredStringKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                storedDict.Add(key, PlayerPrefs.GetString(key));
            }
        }

        PlayerPrefs.DeleteAll();
        foreach (var key in storedDict.Keys)
        {
            var value = storedDict[key];
            if (value is int intValue)
            {
                PlayerPrefs.SetInt(key, intValue);
            }

            if (value is float floatValue)
            {
                PlayerPrefs.SetFloat(key, floatValue);
            }

            if (value is string stringValue)
            {
                PlayerPrefs.SetString(key, stringValue);
            }
        }
    }
}

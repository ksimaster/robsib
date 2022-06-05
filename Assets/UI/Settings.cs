using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown qualityDropdown;

    void Start()
    {
    }

    public void UpdateMoveControl(bool val)
    {
        var player = GameObject.FindWithTag("Player");
        var playerScript = player.GetComponent<MoveGeneral>();
        playerScript.isEasy = !playerScript.isEasy;
        PlayerPrefs.SetInt(PlayerConstants.MoveMode, playerScript.isEasy ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            qualityDropdown.value = 3;
    }
}

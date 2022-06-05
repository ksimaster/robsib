using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    public GameObject startPanel;
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            PlayerPrefs.SetInt(PlayerConstants.IsEducationComplete, 1);
            startPanel.SetActive(false);
        });

        if (PlayerPrefs.GetInt(PlayerConstants.IsEducationComplete) == 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            startPanel.SetActive(false);
        }
    }
}

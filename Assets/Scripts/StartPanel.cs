using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject panelOnStart;
   // public Button button;

    private void Awake()
    {
        /*
        button.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            PlayerPrefs.SetInt(PlayerConstants.IsEducationComplete, 1);
            panelOnStart.SetActive(true); // запуск панели с предысторией
            startPanel.SetActive(false);
        });
        */
        if (PlayerPrefs.GetInt(PlayerConstants.IsEducationComplete) == 0)
        {
            Time.timeScale = 0f;
           
        }
        else
        {
            panelOnStart.SetActive(true); // запуск панели с предысторией
            Time.timeScale = 0f;
            startPanel.SetActive(false);
        }
    }

    public void Reset()
    {
        PlayerPrefs.SetInt(PlayerConstants.IsEducationComplete, 0);
    }
    public void PressStartPanel()
    {
        Time.timeScale = 0f;
        PlayerPrefs.SetInt(PlayerConstants.IsEducationComplete, 1);
        panelOnStart.SetActive(true); // запуск панели с предысторией
        startPanel.SetActive(false);
    }
}

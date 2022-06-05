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
            startPanel.SetActive(false);
        });

        if (PlayerPrefs.GetInt(PlayerConstants.IsFristRun) == 0)
        {
            Time.timeScale = 0f;
            PlayerPrefs.SetInt(PlayerConstants.IsFristRun, 1);
        }
        else
        {
            startPanel.SetActive(false);
        }
    }
}

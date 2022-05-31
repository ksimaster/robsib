using UnityEngine;
using UnityEngine.SceneManagement;


public class PanelDeath : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneManager.LoadScene("MaineScene");
    }
}

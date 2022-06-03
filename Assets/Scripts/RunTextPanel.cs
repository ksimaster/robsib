using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunTextPanel : MonoBehaviour
{
    public Text textOnPanel;
    private string textToRun;
    public float speedText;
    int randMessage;
   // public Text textShutka;
    public bool isNeedRange;
    string[] messages =  { "Плох тот генерал, который не перестал быть солдафоном. ",
    "Военнослужащий возит за собой свою семью по частям. ",
    "Профессор понял, что совершил ошибку, передав секрет своего смертельного оружия военным. Но было уже поздно. И он лег спать.",
    "Когда американский солдат идёт в атаку, он не боится ничего, потому что застрахован. Когда русский солдат идёт в атаку, тогда никто не застрахован…",
    "Армия — дело сугубо добровольное: хочешь — иди, не хочешь — заберут."
     };
    public Slider sliderOut;
    public GameObject PanelMessage;

    private void Awake()
    {
        PanelMessage.SetActive(false);
    }
    private void Start()
    {
        //textToRun = TextGameObject.text;
        textOnPanel.text = "";
        
    }
    private void FixedUpdate()
    {
        if (isNeedRange) return;
        if (sliderOut.value < -13 && sliderOut.value > -14)
        { 
            isNeedRange = true;
            PanelMessage.SetActive(true);
            GetMessageText();

            StartCoroutine(TextRunCoroutine());
        }
    }
    public void GetMessageText()
    {
        randMessage = Random.Range(0, messages.Length);
        
        textToRun = messages[randMessage];
    }
    /* //пауза для панели сообщений
    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }
    */
    IEnumerator TextRunCoroutine()
    {
        foreach (char abc in textToRun)
        {
            textOnPanel.text += abc;
            yield return new WaitForSeconds(speedText);
        }

    }
}

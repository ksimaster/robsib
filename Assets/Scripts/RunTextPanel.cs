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
    private int i = 0;
   // public Text textShutka;
    //public bool isNeedRange;
    public bool [] isNeedRanges = {false, false, false};
    public float [] temperatures = {-14, -20, -30};
    
    public string [][] groupMessages = new string [3][];

    string[] messages =  { "���� ��� �������, ������� �� �������� ���� ����������. ",
    "�������������� ����� �� ����� ���� ����� �� ������. ",
    "��������� �����, ��� �������� ������, ������� ������ ������ ������������ ������ �������. �� ���� ��� ������. � �� ��� �����.",
    "����� ������������ ������ ��� � �����, �� �� ������ ������, ������ ��� �����������. ����� ������� ������ ��� � �����, ����� ����� �� �����������",
    "����� � ���� ������ ������������: ������ � ���, �� ������ � �������."
     };
    public Slider sliderOut;
    public GameObject PanelMessage;

    private void Awake()
    {
        PanelMessage.SetActive(false);
        groupMessages [0] = new string[] { "���� ��� �������, ������� �� �������� ���� ����������. ",
        "�������������� ����� �� ����� ���� ����� �� ������. ",
        "��������� �����, ��� �������� ������, ������� ������ ������ ������������ ������ �������. �� ���� ��� ������. � �� ��� �����.",
        "����� ������������ ������ ��� � �����, �� �� ������ ������, ������ ��� �����������. ����� ������� ������ ��� � �����, ����� ����� �� �����������",
        "����� � ���� ������ ������������: ������ � ���, �� ������ � �������."
     };
        groupMessages [1] = new string[] { "���� ��� �������, ������� �� �������� ���� ����������. ",
        "�������������� ����� �� ����� ���� ����� �� ������. ",
        "��������� �����, ��� �������� ������, ������� ������ ������ ������������ ������ �������. �� ���� ��� ������. � �� ��� �����.",
        "����� ������������ ������ ��� � �����, �� �� ������ ������, ������ ��� �����������. ����� ������� ������ ��� � �����, ����� ����� �� �����������",
        "����� � ���� ������ ������������: ������ � ���, �� ������ � �������."
     };
        groupMessages [2] = new string[] { "���� ��� �������, ������� �� �������� ���� ����������. ",
        "�������������� ����� �� ����� ���� ����� �� ������. ",
        "��������� �����, ��� �������� ������, ������� ������ ������ ������������ ������ �������. �� ���� ��� ������. � �� ��� �����.",
        "����� ������������ ������ ��� � �����, �� �� ������ ������, ������ ��� �����������. ����� ������� ������ ��� � �����, ����� ����� �� �����������",
        "����� � ���� ������ ������������: ������ � ���, �� ������ � �������."
     };
    }
    private void Start()
    {
        //textToRun = TextGameObject.text;
        textOnPanel.text = "";
       
    }
    private void FixedUpdate()
    {
        if(i >= isNeedRanges.Length || i >= groupMessages.Length || i >= temperatures.Length) return;
        if (isNeedRanges[i]) return;
        
        if (sliderOut.value < temperatures[i] + 0.5f && sliderOut.value > temperatures[i] - 0.5f)
        { 
            textOnPanel.text = "";
            isNeedRanges[i] = true;
            PanelMessage.SetActive(true);
            GetMessageText(groupMessages[i]);
            i++;

            StartCoroutine(TextRunCoroutine());
        }
        
        
    }
    public void GetMessageText(string [] messages)
    {
        randMessage = Random.Range(0, messages.Length);
        
        textToRun = messages[randMessage];
    }
    /* //����� ��� ������ ���������
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

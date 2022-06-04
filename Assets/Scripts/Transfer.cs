using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Transfer : MonoBehaviour
{
    public string [] collectTags;
    public Button[] collectButtons;
    public string[] nameCollectResources;
    public string[] nameGiveResources;
    public int[] countResources;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(collectTags[0]))
        {
            collectButtons[0].gameObject.SetActive(true);
        }
        if (col.gameObject.CompareTag(collectTags[1]))
        {
            collectButtons[2].gameObject.SetActive(true);
        }
    }

    public void CollectResource()
    {
        if (collectButtons[0].gameObject.activeSelf)
        {
            countResources[0] = PlayerPrefs.GetInt(nameCollectResources[0]);
            Debug.Log("��������� ������� ���� ������");
            countResources[0]++;
            Debug.Log("������� ������!");
            PlayerPrefs.SetInt(nameCollectResources[0], countResources[0]++);
            Debug.Log("�������� ������ � ����������");
        }

        if (collectButtons[1].gameObject.activeSelf)
        {
            countResources[1] = PlayerPrefs.GetInt(nameCollectResources[1]);
            Debug.Log("��������� ������� ���� ����");
            countResources[1]++;
            Debug.Log("������� ����!");
            Debug.Log("�������� �����: " + countResources[1]);
            PlayerPrefs.SetInt(nameCollectResources[1], countResources[1]++);
            Debug.Log("�������� ���� � ����������");
        }
        /*
        if (collectFirstButton.gameObject.activeSelf) collectFirstButton.gameObject.SetActive(false);
        if (collectSecondButton.gameObject.activeSelf) collectSecondButton.gameObject.SetActive(false);
        */
    }

    public void GiveResource()
    {
        for (int i = 0; i < countResources.Length; i++)
        {
            if (collectButtons[i].gameObject.activeSelf)
            {
                countResources[i] = PlayerPrefs.GetInt(nameGiveResources[i]);
                Debug.Log("������� ��� ������: " + countResources[i]);
                countResources[i]--;
                Debug.Log("������ �������");
                Debug.Log("�������� �����: " + countResources[i]);
                PlayerPrefs.SetInt(nameGiveResources[i], countResources[i]);
                Debug.Log("�������� �������");
            }
        }

        /*
        if (collectFirstButton.gameObject.activeSelf) collectFirstButton.gameObject.SetActive(false);
        if (collectSecondButton.gameObject.activeSelf) collectSecondButton.gameObject.SetActive(false);
        */
    }
}

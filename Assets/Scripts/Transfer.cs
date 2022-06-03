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
            Debug.Log("Загрузили сколько было дерева");
            countResources[0]++;
            Debug.Log("Собрали дерево!");
            PlayerPrefs.SetInt(nameCollectResources[0], countResources[0]++);
            Debug.Log("Записали дерево в переменную");
        }

        if (collectButtons[1].gameObject.activeSelf)
        {
            countResources[1] = PlayerPrefs.GetInt(nameCollectResources[1]);
            Debug.Log("Загрузили сколько было руда");
            countResources[1]++;
            Debug.Log("Собрали руду!");
            Debug.Log("Ресурсов стало: " + countResources[1]);
            PlayerPrefs.SetInt(nameCollectResources[1], countResources[1]++);
            Debug.Log("Записали руду в переменную");
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
                Debug.Log("Ресурса для отдачи: " + countResources[i]);
                countResources[i]--;
                Debug.Log("Отдали ресурсы");
                Debug.Log("Ресурсов стало: " + countResources[i]);
                PlayerPrefs.SetInt(nameGiveResources[i], countResources[i]);
                Debug.Log("Записали ресурсы");
            }
        }

        /*
        if (collectFirstButton.gameObject.activeSelf) collectFirstButton.gameObject.SetActive(false);
        if (collectSecondButton.gameObject.activeSelf) collectSecondButton.gameObject.SetActive(false);
        */
    }
}

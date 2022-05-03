using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Collector : MonoBehaviour
{
    public string collectFirstTag, collectSecondTag;
    public Button collectFirstButton, collectSecondButton;
    public string nameResourceFirst, nameResourceSecond;
    private int countResourceFirst, countResourceSecond;
    private GameObject activatorResource;
    private bool isOre = false;
    private bool isWood = false;

    private void resetActivator()
    {
        isOre = false;
        isWood = false;
        activatorResource = null;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(collectFirstTag) || col.gameObject.CompareTag(collectSecondTag))
            resetActivator();

        if (col.gameObject.CompareTag(collectFirstTag))
        {
            collectFirstButton.gameObject.SetActive(true);
            Debug.Log(col.gameObject.ToString());
            activatorResource = col.gameObject;
            isOre = false;
            isWood = true;
        }

        if (col.gameObject.CompareTag(collectSecondTag))
        {
            collectSecondButton.gameObject.SetActive(true);
            activatorResource = col.gameObject;
            isOre = true;
            isWood = false;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag(collectFirstTag) || col.gameObject.CompareTag(collectSecondTag))
        {
            collectFirstButton.gameObject.SetActive(false);
            collectSecondButton.gameObject.SetActive(false);
            resetActivator();
        }
    }

    public void CollectResource()
    {
        if (collectFirstButton.gameObject.activeSelf)
        {
            countResourceFirst = PlayerPrefs.GetInt(nameResourceFirst);
            Debug.Log("Загрузили сколько было дерева");
            countResourceFirst++;
            if (activatorResource.GetComponent<Giver>().GiveResourceAndCheckVisible())
            {
                collectFirstButton.gameObject.SetActive(false);
            }

            Debug.Log("Собрали дерево!");
            PlayerPrefs.SetInt(nameResourceFirst, countResourceFirst++);
            Debug.Log("Записали дерево в переменную");
        }

        if (collectSecondButton.gameObject.activeSelf)
        {
            countResourceSecond = PlayerPrefs.GetInt(nameResourceSecond);
            Debug.Log("Загрузили сколько было руда");
            countResourceSecond++;

            if (activatorResource.GetComponent<Giver>().GiveResourceAndCheckVisible())
            {
                collectSecondButton.gameObject.SetActive(false);
            }

            Debug.Log("Собрали руду!");
            Debug.Log("Ресурсов стало: " + countResourceSecond);
            PlayerPrefs.SetInt(nameResourceSecond, countResourceSecond++);
            Debug.Log("Записали руду в переменную");
        }


        /*
        if (collectFirstButton.gameObject.activeSelf) collectFirstButton.gameObject.SetActive(false);
        if (collectSecondButton.gameObject.activeSelf) collectSecondButton.gameObject.SetActive(false);
        */
    }

    
}

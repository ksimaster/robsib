using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    private const int DelayStepsBeforHide = 30;

    public string collectFirstTag, collectSecondTag;
    public Button collectFirstButton, collectSecondButton;
    public string nameResourceFirst, nameResourceSecond;
    private int countResourceFirst, countResourceSecond;
    private GameObject activatorResource;
    private bool isOre = false;
    private bool isWood = false;
    private int restStepsBeforHide = 0;
    private bool waitForHide = false;

    private void resetActivator()
    {
        isOre = false;
        isWood = false;
        activatorResource = null;
        waitForHide = false;
        collectFirstButton.gameObject.SetActive(false);
        collectSecondButton.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag(collectFirstTag) && !col.gameObject.CompareTag(collectSecondTag))
            return;

        waitForHide = false;
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

    private async void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag(collectFirstTag) || col.gameObject.CompareTag(collectSecondTag))
        {
            restStepsBeforHide = DelayStepsBeforHide;
            waitForHide = true;
        }
    }

    private void FixedUpdate()
    {
        if (waitForHide) {
            if (restStepsBeforHide == 0) {
                resetActivator();
            }
            else
            {
                restStepsBeforHide--;
            }
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

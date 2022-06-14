using System;
using UnityEngine;
using UnityEngine.UI;

public class Collector : MonoBehaviour
{
    private const int DelayStepsBeforHide = 20;

    public string collectFirstTag, collectSecondTag;
    public Button collectFirstButton, collectSecondButton;
    public string nameResourceFirst, nameResourceSecond;
    private int countResourceFirst, countResourceSecond;
    private GameObject activatorResource;
    private int restStepsBeforHide = 0;
    private bool waitForHide = false;


    private void Awake()
    {
        if (collectFirstButton.gameObject.active)
        {
            collectFirstButton.gameObject.SetActive(false);
        }

        if (collectSecondButton.gameObject.active)
        {
            collectSecondButton.gameObject.SetActive(false);
        }
    }

    private void resetActivator()
    {
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
            activatorResource = col.gameObject;
        }

        if (col.gameObject.CompareTag(collectSecondTag))
        {
            collectSecondButton.gameObject.SetActive(true);
            activatorResource = col.gameObject;
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
            if (restStepsBeforHide <= 0 && !IsNearObject()) {
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
            
            PlayerPrefs.SetInt(nameResourceSecond, countResourceSecond++);
        }
    }

    private bool IsNearObject()
    {
        if (activatorResource == null)
            return false;

        return Math.Pow(activatorResource.transform.position.x - this.gameObject.transform.position.x, 2) +
        Math.Pow(activatorResource.transform.position.z - this.gameObject.transform.position.z, 2) < 20;
    }
}

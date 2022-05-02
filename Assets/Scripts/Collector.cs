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



    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(collectFirstTag))
        {
            collectFirstButton.gameObject.SetActive(true);
            
        }
        if (col.gameObject.CompareTag(collectSecondTag))
        {
            collectSecondButton.gameObject.SetActive(true);
        }


    }

    public void CollectResource()
    {
        if (collectFirstButton.gameObject.activeSelf)
        {
            countResourceFirst = PlayerPrefs.GetInt(nameResourceFirst);
            Debug.Log("Загрузили сколько было дерева");
            countResourceFirst++;
            Debug.Log("Собрали дерево!");
            PlayerPrefs.SetInt(nameResourceFirst, countResourceFirst++);
            Debug.Log("Записали дерево в переменную");
        }

        if (collectSecondButton.gameObject.activeSelf)
        {
            countResourceSecond = PlayerPrefs.GetInt(nameResourceSecond);
            Debug.Log("Загрузили сколько было руда");
            countResourceSecond++;
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

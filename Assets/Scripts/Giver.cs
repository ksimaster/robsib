using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Giver : MonoBehaviour
{
    //public Text textCollectResource;
   // private int countCollectResource;
   // public float minCount, maxCount; 
   // public string collectFirstTag, collectSecondTag;
    public Button [] collectButtons;
    public string [] nameResources;
    public int [] countResources;


/*
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(collectFirstTag))
        {
            collectFirstButton.gameObject.SetActive(true);
        }
        if (col.gameObject.CompareTag(collectFirstTag))
        {
            collectFirstButton.gameObject.SetActive(true);
        }


    }
    */
    public void GiveResource()
    {
        for(int i=0; i < countResources.Length; i++)
        {
            if (collectButtons[i].gameObject.activeSelf)
            {
                countResources[i] = PlayerPrefs.GetInt(nameResources[i]);
                Debug.Log("Ресурса для отдачи: " + countResources[i]);
                countResources[i]--;
                Debug.Log("Отдали ресурсы");
                Debug.Log("Ресурсов стало: " + countResources[i]);
                PlayerPrefs.SetInt(nameResources[i], countResources[i]);
                Debug.Log("Записали ресурсы");
            }
        }

        /*
        if (collectFirstButton.gameObject.activeSelf) collectFirstButton.gameObject.SetActive(false);
        if (collectSecondButton.gameObject.activeSelf) collectSecondButton.gameObject.SetActive(false);
        */
    }

    
}

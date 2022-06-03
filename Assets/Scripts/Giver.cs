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
   // public string collectorTag;
    public Button [] collectButtons;
    public string [] nameResources;
    public int [] countResources;


    /*  private void OnCollisionEnter(Collision col)
      {
          if (col.gameObject.CompareTag(collectorTag))
          {

          }
      }
      */
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

    /*
    Should we hide a button.
    */

    private void Start()
    {
    }

    public void GiveResource()
    {
    }

    public bool GiveResourceAndCheckVisible()
    {
        for(int i=0; i < countResources.Length; i++)
        {
            if (nameResources[i].Length == 1) nameResources[0] = gameObject.name;
            if (collectButtons[i].gameObject.activeSelf)
            {
                countResources[i] = PlayerPrefs.GetInt(nameResources[i]);
                Debug.Log("Ресурса для отдачи: " + countResources[i]);
                countResources[i]--;
                Debug.Log("Отдали ресурсы");
                Debug.Log("Ресурсов стало: " + countResources[i]);
                PlayerPrefs.SetInt(nameResources[i], countResources[i]);
                Debug.Log("Записали ресурсы");

                if (countResources[i] <= 0)
                {
                    Destroy(gameObject);
                    return true;
                }
            }
        }
        
        return false;
    }
}

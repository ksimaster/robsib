using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResourceInObject : MonoBehaviour
{
    
    private int countResourceInObject;
    public float minCount, maxCount;
    public string nameResource;
    public Button collectButton;

    private void Start()
    {
        countResourceInObject = Mathf.RoundToInt(Random.Range(minCount, maxCount));
        Debug.Log(countResourceInObject);
        PlayerPrefs.SetInt(nameResource, countResourceInObject);
        
}

    private void FixedUpdate()
    {
        countResourceInObject = PlayerPrefs.GetInt(nameResource);
        if (countResourceInObject <= 0)
        {
            if (collectButton.gameObject.activeSelf) collectButton.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    /*
    public void CollectResource()
    {
        if (collectFirstButton.gameObject.activeSelf)
        {
            countResourceFirst = PlayerPrefs.GetInt(nameResourceFirst);
            PlayerPrefs.SetInt(nameResourceFirst, countResourceFirst++);
        }

        if (collectSecondButton.gameObject.activeSelf)
        {
            countResourceSecond = PlayerPrefs.GetInt(nameResourceSecond);
            PlayerPrefs.SetInt(nameResourceSecond, countResourceSecond++);
        }

    


    }
    */
}

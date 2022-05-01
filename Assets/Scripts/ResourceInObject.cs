using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResourceInObject : MonoBehaviour
{
  //  public Text textResourceInCollector;
    private int countResourceInObject;
    public float minCount, maxCount; 
    public string collectorTag;

    private void Start()
    {
        countResourceInObject = Mathf.RoundToInt(Random.Range(minCount, maxCount));
        Debug.Log(countResourceInObject);
    }

    private void OnCollisionEnter(Collision col)
    {
        
        Debug.Log("���� �� �������� ����: " + countResourceInObject);
        if (col.gameObject.CompareTag(collectorTag)&&countResourceInObject > 0)
        {
            Debug.Log("���� �������� �� ��������: " + countResourceInObject);
            countResourceInObject--;
            Debug.Log("���� �������� ����� ��������: " + countResourceInObject);
            //textResourceInCollector.text = countResourceInObject.ToString();
            if (countResourceInObject <= 0)
            {
                Destroy(gameObject);
            }


        }


    }
    
}

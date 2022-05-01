using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHome : MonoBehaviour
{
   public GameObject [] homeParts = new GameObject[50];

    void Start()
    {

            StartCoroutine("HideHome");

    }

    IEnumerator HideHome()
    {
        for (int i = 0; i < 50; i++)
        {
            homeParts[i].SetActive(false);
            Debug.Log(i + " элемент уничтожен");
            yield return new WaitForSeconds(3.0f);
        }
        
        
    }


}

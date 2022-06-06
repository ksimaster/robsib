using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PressButtonC : MonoBehaviour
{
    public float waitChange;

    private void Start()
    {
        StartCoroutine(VisibilityText());
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            StopCoroutine(VisibilityText());
            gameObject.SetActive(false);
        } 
        
    }

    IEnumerator VisibilityText()
    {
        while (gameObject.activeSelf)
        {
            gameObject.transform.localScale = Vector3.zero;
           
            yield return new WaitForSeconds(waitChange);
            
            gameObject.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(waitChange);
        }
    }

}

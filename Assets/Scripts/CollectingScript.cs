using UnityEngine;
using UnityEngine.UI;


public class CollectingScript : MonoBehaviour
{
    public Text textResource;
    public int countResource;
    public string collectTag;

    private void OnCollisionEnter(Collision col)
    {
        //Debug.Log(col.gameObject.name);
        Debug.Log("Ресурсов в машине до проверки тэга: " + countResource);
        if (col.gameObject.CompareTag(collectTag))
        {
            Debug.Log("Ресурсов в машине после проверки тэга: " + countResource);
            countResource++;
            Debug.Log("Ресурсов в машине после сбора: " + countResource);
            textResource.text = countResource.ToString();
            

        }


    }

}

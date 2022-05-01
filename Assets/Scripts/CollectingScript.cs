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
        Debug.Log("�������� � ������ �� �������� ����: " + countResource);
        if (col.gameObject.CompareTag(collectTag))
        {
            Debug.Log("�������� � ������ ����� �������� ����: " + countResource);
            countResource++;
            Debug.Log("�������� � ������ ����� �����: " + countResource);
            textResource.text = countResource.ToString();
            

        }


    }

}

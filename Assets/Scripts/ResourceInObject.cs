using UnityEngine;
using UnityEngine.UI;

public class ResourceInObject : MonoBehaviour
{
    private int countResourceInObject;
    public float minCount, maxCount;
    private string nameResource;
    public Button collectButton;

    private void Awake()
    {    
        nameResource = gameObject.name;
        if (!PlayerPrefs.HasKey(nameResource))
        {
            countResourceInObject = Mathf.RoundToInt(Random.Range(minCount, maxCount));
            Debug.Log(countResourceInObject);
            PlayerPrefs.SetInt(nameResource, countResourceInObject);
        } 
        else if (PlayerPrefs.GetInt(nameResource) <= 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        countResourceInObject = PlayerPrefs.GetInt(nameResource);
        if (countResourceInObject <= 0)
        {
            if (collectButton != null && collectButton.gameObject.activeSelf) collectButton.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

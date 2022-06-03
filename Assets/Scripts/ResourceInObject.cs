using UnityEngine;
using UnityEngine.UI;

public class ResourceInObject : MonoBehaviour
{
    private int countResourceInObject;
    public float minCount, maxCount;
    private string nameResource;
    public Button collectButton;

    private void Start()
    {
        nameResource = gameObject.name;
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
}

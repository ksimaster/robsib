using UnityEngine;
using UnityEngine.UI;


public class Warehouse : MonoBehaviour
{
    public int[] countResources;
    public string[] nameResources;
    public Text[] textCountResources;

    private void FixedUpdate()
    {
        /*
         //Берем переменную ресурсов из сохраненных переменных
        countResources[0] = PlayerPrefs.GetInt(nameResources[0]);
        countResources[1] = PlayerPrefs.GetInt(nameResources[1]);
        // Выводим на экран
        textCountResources[0].text = countResources[0].ToString();
        textCountResources[1].text = countResources[1].ToString();
    */
        //тоже самое, через цикл и с возможность работать без вывода на экран
        for (int i = 0; i < countResources.Length; i++)
        {
            if (countResources.Length == 1)
            {
                countResources[i] = PlayerPrefs.GetInt(gameObject.name);
            }
            else
            {
                countResources[i] = PlayerPrefs.GetInt(nameResources[i]);
            }

            // Debug.Log("Загрузили переменную: " + countResources[i]);
            if (textCountResources.Length > 0)
            {
                textCountResources[i].text = countResources[i].ToString();
                //     Debug.Log("Записал в панель: " + countResources[i]);
            }
        }
    }
}

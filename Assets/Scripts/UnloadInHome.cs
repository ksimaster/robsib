using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnloadInHome : MonoBehaviour
{
    public string homeTag;
    public int [] countCarResources;
    public int[] countHomeResources;
    //public Text[] textCountResource;
    private string[] nameCarResources = { "TreeCar", "OreCar" };
    private string[] nameHomeResources = { "TreeHome", "OreHome" };

    //private string fuel = "Fuel";
    private float fuelCount;
    private float fuelExpenses = 4;

    public Slider sliderOut;



    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(homeTag))
        {
            for (int i = 0; i < countCarResources.Length; i++)
            {
                countCarResources[i] = PlayerPrefs.GetInt(nameCarResources[i]); // ������� �� �����, ��� �������� � ������� � ����������
                countHomeResources[i] = PlayerPrefs.GetInt(nameHomeResources[i]); // ������� �� �����, ��� �������� � ����-������ � ����������
                PlayerPrefs.SetInt(nameCarResources[i], 0); //�������� ���������� � ����� �������
                PlayerPrefs.SetInt(nameHomeResources[i], countCarResources[i] + countHomeResources[i]); // ���������� � ���� ������ ���������� ������ ����� ���������� ������ � ������ 
                //textCountResource[i].text = countResources[i].ToString();
            }
            fuelCount = PlayerPrefs.GetFloat("Fuel");
            sliderOut.value = fuelCount - fuelExpenses;
            PlayerPrefs.SetFloat("Fuel", sliderOut.value);
            Debug.Log("��� ������ ��������� ���������� FUEL � Pref");

            
}
    }

}

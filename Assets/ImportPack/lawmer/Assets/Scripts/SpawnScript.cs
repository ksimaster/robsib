using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnScript : MonoBehaviour
{
    public GameObject [] monsters = new GameObject[2];
    public GameObject CameraPoint; // камера на спауне
    public GameObject SpawnPoint; // точка спауна
    public GameObject panelGameOver;
    public GameObject buttonRing;

    // private float speedSpawn; //скорость спауна

    public float minX, minY, minZ; // минимальный край координат с учетом координат точки спауна
    public float maxX, maxY, maxZ; // максимальный край координат с учетом координат точки спауна

    public int countMonster;
    public Text textCountMonster;
    private int i;

    private void Start()
    {
        textCountMonster.text = "Осталось монстров: " + countMonster.ToString();
        
    }

    private void Update()
    {
        if (ColllisionDead.buttonCount)
        {
            buttonRing.SetActive(true);
            ColllisionDead.buttonCount = false;
        }
    }

    public void SpawnMonster()
    {
        if (countMonster > 0)
        {
            i = Random.Range(0, 2);
            float x = Random.Range(minX, maxX) + SpawnPoint.transform.position.x; // позиция X
            float y = Random.Range(minY, maxY) + SpawnPoint.transform.position.y; // позиция Y
            float z = Random.Range(minZ, maxZ) + SpawnPoint.transform.position.z; // позиция Z
            Instantiate(monsters[i], new Vector3(x, y, z), transform.rotation);
            countMonster--;
            textCountMonster.text = "Осталось монстров: " + countMonster.ToString();
            buttonRing.SetActive(false);
        }
        else
        {
            buttonRing.SetActive(false);
            panelGameOver.SetActive(true);
        }
    }


    
}

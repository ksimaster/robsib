using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamScript : MonoBehaviour
{
    public Camera cam1;
    public GameObject[] points = new GameObject[4];
    private int i = 1;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PointCamera(points[i]);
            Debug.Log("Включена камера " + i);
            i++;
            if (i > 3) i = 0; 
        }
    }

    public void PointCamera(GameObject point)
    {

        cam1.transform.position = point.transform.position; //new Vector3(0, 23.1f, 118.1f);
        cam1.transform.rotation = point.transform.rotation; //new Quaternion(0f, Mathf.PI, 0f, 0f);
    }

    /*
    public void SwitchCamera_2()
    {
        cam1.transform.position = new Vector3(11.9f, 23.9f, 5.4f);
        cam1.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public void SwitchCamera_1()
    {
        cam1.transform.position = new Vector3(0f, 23.1f, 118.1f);
        cam1.transform.rotation = new Quaternion(0f, Mathf.PI, 0f, 0f);
    }
    */
}

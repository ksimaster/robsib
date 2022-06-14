using UnityEngine;

public class SwitchCamScript : MonoBehaviour
{
    public Camera cam1;
    public GameObject[] points = new GameObject[8];
    private int cameraId = 0;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerConstants.Camera)) {
            SwitchCamera(PlayerPrefs.GetInt(PlayerConstants.Camera));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCamera(cameraId + 1);
        }
    }

    public void PointCamera(GameObject point)
    {

        cam1.transform.position = point.transform.position; //new Vector3(0, 23.1f, 118.1f);
        cam1.transform.rotation = point.transform.rotation; //new Quaternion(0f, Mathf.PI, 0f, 0f);
    }

    private void SwitchCamera(int newId)
    {
        cameraId = newId;
        if (cameraId > 7)
        {
            cameraId = 0;
        }
        
        PointCamera(points[cameraId]);
        PlayerPrefs.SetInt(PlayerConstants.Camera, cameraId);
        Debug.Log("Включена камера " + cameraId);
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

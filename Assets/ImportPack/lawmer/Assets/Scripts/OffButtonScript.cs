using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffButtonScript : MonoBehaviour
{
    public GameObject target;
    public float distanceToTarget = 3.0f;
    public GameObject off_Button;


    private void Off_Distance()
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);


        // Debug.Log(dist);

        if (Mathf.Abs(dist) > distanceToTarget)
        {
            off_Button.SetActive(false);
        }
    }

    private void Update()
    {
        Off_Distance();
    }
}

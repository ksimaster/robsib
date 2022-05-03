using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGiver : MonoBehaviour
{
    public string collectorTag;
    public GameObject carrierGiver;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(collectorTag))
        {
            carrierGiver.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveOtherObjectOnEnable : MonoBehaviour
{
    public GameObject otherObject;
    private void OnEnable()
    {
        otherObject.SetActive(false);
    }
}

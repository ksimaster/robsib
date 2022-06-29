using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOtherObjectOnDisable : MonoBehaviour
{
    public GameObject otherObject;
    private void OnDisable()
    {
        otherObject.SetActive(true);
    }

        
}

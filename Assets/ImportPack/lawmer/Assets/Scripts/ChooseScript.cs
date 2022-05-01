using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseScript : MonoBehaviour
{
    
    [SerializeField]
    private GameObject chooseObject;



    private void OnMouseEnter()
    {
        chooseObject.SetActive(true);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnChooseScript : MonoBehaviour
{
    [SerializeField]
    private GameObject generalObject;
    [SerializeField]
    private GameObject chooseObject;


    private void OnMouseExit()
    {
        generalObject.SetActive(true);
        chooseObject.SetActive(false);
    }
}

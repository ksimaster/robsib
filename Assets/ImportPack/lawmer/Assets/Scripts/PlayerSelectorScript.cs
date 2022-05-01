using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectorScript : MonoBehaviour
{

    [SerializeField]
    private GameObject Box;

    private void Start()
    {
        
    }

    public void PlayerWithoutBox()
    {
        
        Box.SetActive(false);
    }

    public void PlayerWithBox()
    {

        Box.SetActive(true);
       
        
    }




}

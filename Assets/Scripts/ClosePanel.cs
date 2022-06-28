using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panelToClose;
    private void OnEnable()
    {
        panelToClose.SetActive(false);
    }
}

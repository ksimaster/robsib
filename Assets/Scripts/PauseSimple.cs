using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSimple : MonoBehaviour
{
    public void PauseOn()
    {
        Time.timeScale = 0;
    }
    public void PauseOff()
    {
        Time.timeScale = 1;
    }

}

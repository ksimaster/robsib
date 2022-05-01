using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject button_PutInFire, button_Trow, button_Take;
    public int log;
    public int max_Log = 5;
    public string collisionTagLog;
    public string collisionTagFire;


    void Start()
    {
        
    }

    void Update()
    {
        CheckLog();
    }

    void OnOffButton()
    {
        
    }

   public void LogInHands()
    {
        if (log < max_Log)
        {
            log++;
            Debug.Log(log);
        }
    }
    public void LogOutHands()
    {
        if (log > 0)
        {
            log--;
            Debug.Log(log);
        }
    }

    void CheckLog() { 
        if (log != 0) { 
            button_Trow.SetActive(true);
        } else if (log==0)
        {
            button_Trow.SetActive(false);
            button_PutInFire.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(collisionTagLog))
        {
           
            button_Take.SetActive(true);

        }
       /* else
        {
            button_Take.SetActive(false);
        }
*/
        if (col.gameObject.CompareTag(collisionTagFire) && log!=0)
        {
            button_PutInFire.SetActive(true);

        }
        else
        {
            button_PutInFire.SetActive(false);
        }

    }

 
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    public GameObject button_PutInFinish, button_Trow, button_Take;
    public int countBox;
    public int maxBox = 1;
    public string collisionTagBox;
    public string collisionTagFinish;




    void Update()
    {
        CheckBox();
    }

    void OnOffButton()
    {

    }

    public void LogInHands()
    {
        if (countBox < maxBox)
        {
            countBox++;
            Debug.Log(countBox);
        }
    }
    public void LogOutHands()
    {
        if (countBox > 0)
        {
            countBox--;
            Debug.Log(countBox);
        }
    }

    void CheckBox()
    {
        if (countBox != 0)
        {
            button_Trow.SetActive(true);
        }
        else if (countBox == 0)
        {
            button_Trow.SetActive(false);
            button_PutInFinish.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(collisionTagBox))
        {

            button_Take.SetActive(true);

        }
        /* else
         {
             button_Take.SetActive(false);
         }
 */
        if (col.gameObject.CompareTag(collisionTagFinish) && countBox != 0)
        {
            button_PutInFinish.SetActive(true);

        }
        else
        {
            button_PutInFinish.SetActive(false);
        }

    }


}

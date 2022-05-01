using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDestroyScript : MonoBehaviour
{
    public string collisionTagFinish;
    
    


    private void OnCollisionEnter(Collision col)
    {
        // Debug.Log(col.gameObject.name);
        if (col.gameObject.CompareTag(collisionTagFinish))
        {
            Destroy(gameObject);

        }

    }


}

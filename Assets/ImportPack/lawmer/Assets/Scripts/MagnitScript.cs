using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnitScript : MonoBehaviour
{
    public string collisionMagnitTag;

    private Animator playerAnimator;

    public float xImpuls;
    public float zImpuls;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision col)
    {
        // Debug.Log(col.gameObject.name);
        if (col.gameObject.CompareTag(collisionMagnitTag))
        {
            playerAnimator.SetBool("Hit", true);

            Invoke("ImpulsObject", 1.5f);

            playerAnimator.SetBool("Hit", false);

        }

    }

    public void ImpulsObject()
    {
        gameObject.transform.position = new Vector3(xImpuls, 0, zImpuls);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutScript : MonoBehaviour
{
    AudioSource shout;
    AudioClip shoutSound;
    public bool shoutBool;
    
    
    public void FixedUpdate()
    {
        if(shoutBool) shout.PlayOneShot(shoutSound);
    }
}

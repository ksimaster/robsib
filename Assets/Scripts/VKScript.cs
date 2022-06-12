using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VKScript : MonoBehaviour
{
    public void ShareFriend(){
    	WebGLPluginJS.ShareFunction();
    }

    public void ShowAdInterstitial(){
    	WebGLPluginJS.InterstitialFunction();
    }
    public void ShowAdReward(){
    	WebGLPluginJS.RewardFunction();
    }
}

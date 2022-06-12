using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VKScript : MonoBehaviour
{
	public Slider sliderHome;
	public float rewardBonusToSlider;
    public void ShareFriend(){
    	WebGLPluginJS.ShareFunction();
    }

    public void ShowAdInterstitial(){
    	WebGLPluginJS.InterstitialFunction();
    }
    public void ShowAdReward(){
    	WebGLPluginJS.RewardFunction();
    	sliderHome.value += rewardBonusToSlider;
    }
}

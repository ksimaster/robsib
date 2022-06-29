using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADScript : MonoBehaviour
{
	public Slider sliderHome;
	public Slider sliderFuelCar;
	public float rewardBonusSliderHome;
	public float rewardBonusSliderFuel;
	public float lowBalanceFuel;

    public void ShareFriend(){
    	WebGLPluginJS.ShareFunction();
    }
#if UNITY_WEBGL && !UNITY_EDITOR
    public void ShowAdInterstitial(){
    	WebGLPluginJS.InterstitialFunction();
    }
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
    public void ShowAdReward(){
    	WebGLPluginJS.RewardFunction();
    	sliderHome.value += rewardBonusSliderHome;
    	if(sliderFuelCar.value<=lowBalanceFuel) sliderFuelCar.value += rewardBonusSliderFuel;
    }
#endif
}

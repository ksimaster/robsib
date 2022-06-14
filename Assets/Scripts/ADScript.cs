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

    public void ShowAdInterstitial(){
    	WebGLPluginJS.InterstitialFunction();
    }
    public void ShowAdReward(){
    	WebGLPluginJS.RewardFunction();
    	sliderHome.value += rewardBonusSliderHome;
    	if(sliderFuelCar.value<=lowBalanceFuel) sliderFuelCar.value += rewardBonusSliderFuel;
    }
}

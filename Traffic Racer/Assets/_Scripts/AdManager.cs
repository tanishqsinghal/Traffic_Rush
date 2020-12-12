using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
//using UnityEngine.XR.WSA.Input;
using System.Collections;
using TMPro;

public class AdManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	public static AdManager current;

	//All Ids Are Set To Test Ids
	[Header ("Admob Ids")]
	[SerializeField] string _appId = "ca-app-pub-5406409821252930~3890799233";           			 	//Admob App Id

	[SerializeField] string _bannerAdIdGarage = "ca-app-pub-3940256099942544/6300978111";      			 //Banner Ad Id For Garage scene
	[SerializeField] string _bannerAdIdPause = "ca-app-pub-3940256099942544/6300978111";      			 //Banner Ad Id For Pause Menu
	[SerializeField] string _bannerAdIdDeath = "ca-app-pub-3940256099942544/6300978111";      			 //Banner Ad Id For Game Over

	[SerializeField] string _fullScreenAdId = "ca-app-pub-3940256099942544/1033173712";  			 	//Full Screen Ad Id

	[SerializeField] string _rewardedAdID = "ca-app-pub-3940256099942544/5224354917";    			 	//Rewarded Video Ad Id

	private BannerView _bannerAdGarage;                                       				 			//Banner Ad View for garage
	private BannerView _bannerAdPause;                                       				 			//Banner Ad View for pause menu
	private BannerView _bannerAdDeath;                                       				 			//Banner Ad View for pause menu

	private InterstitialAd _fullscreenAd;                               				 			 	//Full Screen Ad View

	private RewardedAd _rewardedVideoAd;                     				 							//Rewarded Video Ad

	void Awake()
	{
		//If a Ad Manager exists and this isn't it...
		if (current != null && current != this)
		{
			//...destroy this and exit. There can only be one Game Manager
			Destroy(gameObject);
			return;
		}

		//Set this as the current game manager
		current = this;

		//Persis this object between scene reloads
		DontDestroyOnLoad(gameObject);

		//Initialize Mobile Ads Using Admob App Id
		MobileAds.Initialize(_appId);
	}

	void Start()
	{
		//Send Request For Banner Ad
		RequestBannerAdGarage ();
		RequestBannerAdDeath ();
		RequestBannerAdPause ();

		//Send Request For Full Screen Ad
		current.RequestFullScreenAd ();

		//Send Request For Reward Based Video Ad
		RequestRewardedAd ();
	}

	//Request Banner Ad With Banner Id
	void RequestBannerAdGarage()
	{
		current._bannerAdGarage = new BannerView(current._bannerAdIdGarage, AdSize.Banner, AdPosition.Top);

		AdRequest adRequest = new AdRequest.Builder().Build();

		//Load Banner View Using The Build Request
		current._bannerAdGarage.LoadAd(adRequest);

		//Hide Banner Ad View
		current._bannerAdGarage.Hide();
	}

	public static void RequestNewBannerAdGarage()
	{
		//Request New Banner View
		current.RequestBannerAdGarage ();
	}

	//Display Banner Ad
	public static void ShowBannerAdGarage()
	{
		if(current._bannerAdGarage != null)
		{
			//Display Banner Ad
			current._bannerAdGarage.Show ();
		}
	}

	//Function To Hide Banner Ad
	public static void HideBannerGarage()
	{
		if(current._bannerAdGarage != null)
		{
			//Hide Banner Ad View
			current._bannerAdGarage.Hide();
		}
	}

	//Request Banner Ad With Banner Id
	void RequestBannerAdPause()
	{
		current._bannerAdPause = new BannerView(current._bannerAdIdPause, AdSize.Banner, AdPosition.Bottom);

		AdRequest adRequest = new AdRequest.Builder().Build();

		//Load Banner View Using The Build Request
		current._bannerAdPause.LoadAd(adRequest);

		//Hide Banner Ad View
		current._bannerAdPause.Hide();
	}

	public static void RequestNewBannerAdPause()
	{
		//Request New Banner View
		current.RequestBannerAdPause ();
	}

	//Display Banner Ad
	public static void ShowBannerAdPause()
	{
		if(current._bannerAdPause != null)
		{
			//Display Banner Ad
			current._bannerAdPause.Show ();
		}
	}

	//Function To Hide Banner Ad
	public static void HideBannerPause()
	{
		if(current._bannerAdPause != null)
		{
			//Hide Banner Ad View
			current._bannerAdPause.Hide();
		}
	}


	//Request Banner Ad With Banner Id
	void RequestBannerAdDeath()
	{
		current._bannerAdDeath = new BannerView(current._bannerAdIdDeath, AdSize.Banner, AdPosition.Bottom);

		AdRequest adRequest = new AdRequest.Builder().Build();

		//Load Banner View Using The Build Request
		current._bannerAdDeath.LoadAd(adRequest);

		//Hide Banner Ad View
		current._bannerAdDeath.Hide();
	}

	public static void RequestNewBannerAdDeath()
	{
		//Request New Banner View
		current.RequestBannerAdDeath ();
	}

	//Display Banner Ad
	public static void ShowBannerAdDeath()
	{
		if(current._bannerAdDeath != null)
		{
			//Display Banner Ad
			current._bannerAdDeath.Show ();
		}
	}

	//Function To Hide Banner Ad
	public static void HideBannerDeath()
	{
		if(current._bannerAdDeath != null)
		{
			//Hide Banner Ad View
			current._bannerAdDeath.Hide();
		}
	}

	public static void HideAllBannerAds()
	{
		//Hide all banner ads
		HideBannerDeath ();
		HideBannerPause ();
		HideBannerGarage ();
	}

	public static void RequestAllBannerAds()
	{
		//Send request for all banner ads
		RequestNewBannerAdDeath ();
		RequestNewBannerAdPause ();
	}

	//FRequest  Full Screen Ad
	void RequestFullScreenAd()
	{
		current._fullscreenAd = new InterstitialAd(current._fullScreenAdId);

		AdRequest adRequest = new AdRequest.Builder().Build();

		current._fullscreenAd.LoadAd(adRequest);
	}

	//Display Full Screen Ad
	public static void ShowFullScreenAd()
	{
		//Display Full Screen Ad Only If It Is Loaded
		if(current._fullscreenAd.IsLoaded ())
		{
			current._fullscreenAd.Show ();

			//Send REquest For Full Screen Ad
			current.RequestFullScreenAd ();
		}
	}

	//Reward Based Video Ad Request
	void RequestRewardedAd()
	{
		_rewardedVideoAd = new RewardedAd (_rewardedAdID);

		//Set event delegates
		_rewardedVideoAd.OnUserEarnedReward += HandlerRewardedAdRewardEarned;

		AdRequest adRequest = new AdRequest.Builder().Build();

		_rewardedVideoAd.LoadAd(adRequest);
	}

	void HandlerRewardedAdRewardEarned(object sender, Reward args)
	{
		int availableMoney = PlayerPrefs.GetInt ("AvailableMoney", 0);
		int newMoneyAmount = availableMoney + 1000;
		PlayerPrefs.SetInt ("AvailableMoney", newMoneyAmount);

		StartCoroutine (CountAnimation (availableMoney, newMoneyAmount, GameManager.totalMoneyText));

		//Play reward audio
		AudioManager.PlayRewardAudio ();
	}

	public static void ShowRewardedAd()
	{
		//Show the rewarded ad if it is loaded
		if(current._rewardedVideoAd.IsLoaded () == true)
		{
			current._rewardedVideoAd.Show ();

			//Send Request for the ad again
			current.RequestRewardedAd ();
		}
		else
		{
			//Play error sound
			AudioManager.PlayErrorSound ();
		}
	}

	IEnumerator CountAnimation(float initialValue, float targetValue, TextMeshProUGUI targetText)
	{
		float displayNumber = initialValue;
		for(float timer = 0; timer <= 1.5f; timer += Time.deltaTime)
		{
			float progress = timer / 1.5f;

			displayNumber = (int)Mathf.Lerp (initialValue, targetValue, progress);

			//Play count audio
			AudioManager.PlayCountSound ();

			targetText.text = displayNumber.ToString ();

			//yield return null;
			yield return null;
		}

		targetText.text = targetValue.ToString ();
	}
}

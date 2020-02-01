using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class AdManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	public static AdManager current;

	//All Ids Are Set To Test Ids
	[Header ("Admob Ids")]
	[SerializeField] string _appId = "ca-app-pub-5406409821252930~3890799233";           			 //Admob App Id
	[SerializeField] string _bannerAdId = "ca-app-pub-3940256099942544/6300978111";      			 //Banner Ad Id For This App
	[SerializeField] string _fullScreenAdId = "ca-app-pub-3940256099942544/1033173712";  			 //Full Screen Ad Id
	//[SerializeField] string _rewardedAdID = "ca-app-pub-3940256099942544/5224354917";    			 //Rewarded Video Ad Id

	private BannerView _bannerAd;                                       				 			 //Banner Ad View

	private InterstitialAd _fullscreenAd;                               				 			 //Full Screen Ad View

	//private RewardBasedVideoAd _rewardBasedVideoAd;                     				 //Reward Based Video Ad

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
		RequestBannerAd ();

		//Send Request For Full Screen Ad
		current.RequestFullScreenAd ();

		//Send Request For Reward Based Video Ad
		//RequestRewardedAd ();
	}

	//Request Banner Ad With Banner Id
	void RequestBannerAd()
	{
		current._bannerAd = new BannerView(current._bannerAdId, AdSize.Banner, AdPosition.Bottom);

		AdRequest adRequest = new AdRequest.Builder().Build();

		//Load Banner View Using The Build Request
		current._bannerAd.LoadAd(adRequest);

		//Hide Banner Ad View
		current._bannerAd.Hide();
	}

	//FRequest  Full Screen Ad
	void RequestFullScreenAd()
	{
		current._fullscreenAd = new InterstitialAd(current._fullScreenAdId);

		AdRequest adRequest = new AdRequest.Builder().Build();

		current._fullscreenAd.LoadAd(adRequest);
	}

	/*//Reward Based Video Ad Request
	void RequestRewardedAd()
	{
		_rewardBasedVideoAd = RewardBasedVideoAd.Instance;

		AdRequest adRequest = new AdRequest.Builder().Build();

		_rewardBasedVideoAd.LoadAd(adRequest, _rewardedAdID);
	}*/

	public static void RequestNewBannerAd()
	{
		//Request New Banner View
		current.RequestBannerAd ();
	}

	//Display Banner Ad
	public static void ShowBannerAd()
	{
		if(current._bannerAd != null)
		{
			//Display Banner Ad
			current._bannerAd.Show ();
		}

		//Request Banner View Again
		//current.RequestBannerAd ();
	}

	//Function To Hide Banner Ad
	public static void HideBanner()
	{
		if(current._bannerAd != null)
		{
			//Hide Banner Ad View
			current._bannerAd.Hide();
		}
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
}

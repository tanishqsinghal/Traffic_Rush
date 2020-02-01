using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GameManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	static GameManager current;

	public static int gameMode = 0;						//Game mode representor (One way/ two way/ free ride)

	public static int selectedBikeNumber = 0;			//Number of the currently selected bike

	public static int totalCrashes = 0;					//Total rider crashes in the current game session

	void Awake()
	{
		//Set target framerate for the device
		Application.targetFrameRate = 60;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		//If a Game Manager exists and this isn't it...
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

		//Sign in the google user
		SignIn ();

		//If game has been played for the first time
		//Then show the tutorial level directly
		if(PlayerPrefs.GetInt ("PlayingFirstTime", 1) == 1)
		{
			PlayerPrefs.SetInt ("AvailableMoney", 100);
			PlayerPrefs.SetInt ("PlayingFirstTime", 0);
			LoadSceneOfIndex (2);
		}
	}

	void SignIn()
	{
		// Create client configuration
		PlayGamesClientConfiguration config = new 
			PlayGamesClientConfiguration.Builder()
			.Build();

		// Enable debugging output (recommended)
		PlayGamesPlatform.DebugLogEnabled = true;

		// Initialize and activate the platform
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();

		Social.localUser.Authenticate(success => { });
	}

	public void ShowAllLeaderboards() {
		if (PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI();
		}
		else {
			SignIn ();
		}
	}

	public void ShowCurrentLeaderboard()
	{
		if(gameMode == 0)
		{
			ShowOneWayLeaderboard ();
		}
		else if(gameMode == 1)
		{
			ShowTwoWayLeaderboard ();
		}
		else if(gameMode == 2)
		{
			ShowRageModeLeaderboard ();
		}
	}

	void ShowOneWayLeaderboard() {
		if (PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIlLCwipgGEAIQAQ");
		}
		else {
			SignIn ();
		}
	}

	void ShowTwoWayLeaderboard() {
		if (PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIlLCwipgGEAIQAg");
		}
		else {
			SignIn ();
		}
	}

	void ShowRageModeLeaderboard() {
		if (PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIlLCwipgGEAIQAw");
		}
		else {
			SignIn ();
		}
	}

	public void LoadSceneOfIndex(int sceneIndex)
	{
		Time.timeScale = 1f;

		SceneManager.LoadScene (sceneIndex);

		//Hide banner ad
		AdManager.HideBanner ();

		//Request new banner ad
		AdManager.RequestNewBannerAd ();
	}
}

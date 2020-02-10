using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelController : MonoBehaviour
{
	static LevelController current;							//This static class holds reference to itself and mainly used to get values or references from anywhere

	private int _currentLevelNumber = 1;					//Speed level on which the racer is playing currently
	private float _racerStartPositionZ = 0f;				//Start position of the racer on Z axis
	private int _score = 0;									//Current score of the player
	private int _scoreAdder = 0;							//Add this to the current score on each fixed update
	private bool _isGameOver = false;						//Game over status
	private float _timeScaleBeforePause = 0f;				//Global timescale before pausing the game
	private int _totalCloseCalls = 0;						//Total close overtakes in the current game session
	private int _totalDestructions = 0;						//Total obstacles destroyed in the current game session
	private bool _oppositeLaneStatusFlag = false;			//Status flag to check the updation of the opposite lane timer
	private bool _isHighScore = false;						//Is the current score of the player is his high score???

	[SerializeField] bool isThisTutorial = false;			//Is the current scene a tutorial ???

	[SerializeField] float accelerationSpeed = 1f;			//Time taken in transition from current speed to accelerated speed
	[SerializeField] float deAccelerationSpeed = 1f;		//Time taken in transition from current speed to deaccelerated speed
	[SerializeField] float mileage = 100f;					//Distance that the racer can travel using nitro/ fuel

	[SerializeField] float realWorldMultiplier = 10f;		//Multiply the game speed to make it look like real world
	[SerializeField] float distanceToDestroy = 1f;			//Distance of any object from the player to destory itself

	[SerializeField] int maximumLevelNumber = 9;			//Maximum speed level that the racer can achieve

	[SerializeField] int pointsToAddDestory = 150;			//How many points to add in current score when racer destroy an obstacle
	[SerializeField] int pointsToAddCloseCall = 100;		//How many points to add in current score when racer passes a vehicle closely

	[SerializeField] float gameOverDelay = 1f;				//Delay to show game over ui after racer accident
	[SerializeField] float countAnimationTime = 1f;			//Total time of the text count animation

	public static int layerEnemyToInt;						//Integer representation of the layer "Enemy"
	public static int layerPlayerToInt;						//Integer representation of the layer "Player"
	public static int layerRacerToInt;						//Integer representation of the layer "Rider"
	public static int layerNitroToInt;						//Integer representation of the layer "Nitro"
	public static int layerGuardRailToInt;					//Integer representation of the layer "GuardRail"

	public Image nitroAmountFillImage;						//Image fill which shows the currently available nitro amount

	public Image scoreMultiplierImage;						//Fill image of the score multiplier
	public TextMeshProUGUI scoreMultiplierText;				//Text which shows the current score multiplier

	public TextMeshProUGUI currentScoreText;				//UI text which displayes current score

	public TextMeshProUGUI oppositeLaneTimerText;			//UI text which displayes the time in opposite lane
	public GameObject oppositeLaneTimerObject;				//Gameobject which contains the opposite lane timer elements

	public GameObject levelIncreamentPrompt;				//Prompt which is shown when the level is increamented

	public GameObject guardRailEffect;						//Effect shown when the racer is near the guardrails
	public GameObject closeCallEffect;						//When the racer closely overtakes a vehicle
	public GameObject destructionMessageEffect;				//The message effect shown when the racer destroys a vehicle

	public Slider sensitivitySlider;						//Slider which controls the sensitivity of accelerometer

	//UI references
	public GameObject controllerCanvas;						//UI canvas which contains control system
	public GameObject pauseMenuCanvas;						//Pause menu UI

	[Header ("Racer References")]
	public Transform racerTransform;						//Transform of the racer object
	public GameObject[] bikeModels;							//All bike models in the scene
	public static GameObject activeBikeObject;				//Active bike model in the scene

	[Header ("Game Over References")]
	public GameObject gameOverCanvas;						//Game Over UI canvas
	public GameObject highScoreEffect;						//Shown when the player get high score
	public TextMeshProUGUI finalScoreText;					//UI text which displays final score after game over
	public TextMeshProUGUI totalDistanceText;				//UI text which displayes total distance travelled by the racer after game over
	public TextMeshProUGUI totalClosePassesText;			//UI text which displays total close overtakes after game over
	public TextMeshProUGUI totalDestructionsText;			//UI text which displays total number of destructions after game over
	public TextMeshProUGUI oppositeLaneTimeText;			//UI text which displays Time ridden in opposite lane after game over
	public TextMeshProUGUI totalDistanceRewardText;			//UI text which displayes rewards for total distance travelled by the racer after game over
	public TextMeshProUGUI totalClosePassesRewardText;		//UI text which displays rewards for total close overtakes after game over
	public TextMeshProUGUI totalDestructionsRewardText;		//UI text which displays rewards for total number of destructions after game over
	public TextMeshProUGUI oppositeLaneTimeRewardText;		//UI text which displays rewards for Time ridden in opposite lane after game over
	public TextMeshProUGUI totalRewardText;					//UI text which displays the total reward for this game session
	public TextMeshProUGUI totalMoneyText;					//UI text which displays total money that the player has

	void Awake()
	{
		current = this;

		//Get reference to Enemy layer
		layerEnemyToInt = LayerMask.NameToLayer ("Enemy");

		//Get reference to Player layer
		layerPlayerToInt = LayerMask.NameToLayer ("Player");

		//Get reference to Player layer
		layerRacerToInt = LayerMask.NameToLayer ("Rider");

		//Get reference to GuardRail layer
		layerGuardRailToInt = LayerMask.NameToLayer ("GuardRail");

		//Get reference to Nitro layer
		layerNitroToInt = LayerMask.NameToLayer ("Nitro");
	}

	void Start()
	{
		_racerStartPositionZ = racerTransform.position.z;

		//Enable the active bike model
		bikeModels [GameManager.selectedBikeNumber].SetActive (true);

		//Set the active bike object
		activeBikeObject = bikeModels [GameManager.selectedBikeNumber];

		//Set the sensitivity slider position
		sensitivitySlider.value = PlayerPrefs.GetFloat ("Sensitivity", (sensitivitySlider.maxValue + sensitivitySlider.minValue) / 2f);

		//Play level music
		AudioManager.StartLevelAudio ();
	}

	void Update()
	{
		//If Player Presses Back Button While Playing The Game
		//Then Pause The Game
		if(Input.GetKeyDown (KeyCode.Escape))
		{
			if(controllerCanvas.activeInHierarchy)
			{
				PauseGame ();
			}
			else if(pauseMenuCanvas.activeInHierarchy)
			{
				ResumeGame ();
			}
		}
	}

	void FixedUpdate()
	{
		//Increament score only if the player is moving
		if(Forward_Movement.GetRacerSpeed () > 1f && !current._isGameOver)
		{
			_scoreAdder += Forward_Movement.ScoreMultiplier ();
			_score = (int)((racerTransform.position.z - _racerStartPositionZ) * 2.5f) + _scoreAdder;

		}
		currentScoreText.text = _score.ToString ();
	}

	public void PauseGame()
	{
		//Not the curret time scale
		_timeScaleBeforePause = Time.timeScale;

		//Pause the time
		Time.timeScale = 0f;

		//Disable controls
		controllerCanvas.SetActive (false);

		//Enable pause menu UI
		pauseMenuCanvas.SetActive (true);

		//Pause audio
		AudioManager.PauseAudio ();

		//Display banner ad
		AdManager.ShowBannerAd ();
	}

	public void ResumeGame()
	{
		//Hide banner ad
		AdManager.HideBanner ();

		//Request new banner ad
		AdManager.RequestNewBannerAd ();

		//Disable pause menu UI
		pauseMenuCanvas.SetActive (false);

		//Enable controls
		controllerCanvas.SetActive (true);

		//Reset time scale
		Time.timeScale = _timeScaleBeforePause;

		//Resume audio
		AudioManager.ResumeAudio ();
	}

	public static void UpdateNitroFill(float fillamount)
	{
		//Fill the image according to the available nitro amount
		current.nitroAmountFillImage.fillAmount = fillamount;
	}

	public static void UpdateScoreMultiplierUI(int scoreMultiplier, float fillAmount)
	{
		current.scoreMultiplierText.text = "x" + scoreMultiplier.ToString ();
		current.scoreMultiplierImage.fillAmount = fillAmount;
	}

	public static void GameOver()
	{
		//Update game over status
		current._isGameOver = true;

		//Shake the camera
		CameraScript.ShakeCamera (0.1f, 0.1f, 0.1f, 0.75f, 0.0075f);

		//Vibrate the phone
		AudioManager.VibratePhone ();

		if(!current.isThisTutorial)
		{
			//Disbale player controls
			current.controllerCanvas.SetActive (false);

			//Set final score text
			current.finalScoreText.text = "SCORE: " + current._score.ToString ();

			//If the current score of the player is higher than the previous high score in the current game mode
			//then set new high score
			if(current._score > PlayerPrefs.GetInt ("HighScoreInMode" + GameManager.gameMode.ToString (), 0))
			{
				//Enbale high score effect
				current.highScoreEffect.SetActive (true);

				PlayerPrefs.SetInt ("HighScoreInMode" + GameManager.gameMode.ToString (), current._score);

				//Update google play games leaderboards
				if (Social.localUser.authenticated) {

					if(GameManager.gameMode == 0)
					{
						Social.ReportScore (current._score, "CgkI8prkgvkHEAIQAA", (bool success) =>{});
					}
					else if(GameManager.gameMode == 1)
					{
						Social.ReportScore (current._score, "CgkI8prkgvkHEAIQAQ", (bool success) =>{});
					}
					else if(GameManager.gameMode == 2)
					{
						Social.ReportScore (current._score, "CgkI8prkgvkHEAIQAg", (bool success) =>{});
					}
				}

				current._isHighScore = true;
			}

			//Call the game over function of the racer controller
			Racer_Controller.GameOver ();

			//Call game over function of the forward movement
			Forward_Movement.GameOver ();

			current.Invoke ("StartGameOverCoroutine", current.gameOverDelay);
		}
		else
		{
			//Restartscene
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	void StartGameOverCoroutine()
	{
		//Increament number of crashes in the current game session
		GameManager.totalCrashes++;
		//Show ad every second time racer crashes
		if(GameManager.totalCrashes % 2 == 0)
		{
			AdManager.ShowFullScreenAd ();
		}

		StartCoroutine ("EnableGameOverUI");

		//Display banner ad
		AdManager.ShowBannerAd ();
	}

	IEnumerator EnableGameOverUI()
	{
		//Enable game over UI
		gameOverCanvas.SetActive (true);

		if(_isHighScore)
		{
			//Play high score audio
			AudioManager.PlayHighScoreAudio ();

			_isHighScore = false;
		}

		//Get the required values
		float totalDistance = ((racerTransform.position.z - _racerStartPositionZ) / 1000f);
		int totalClosePasses = _totalCloseCalls;
		int totalDestructions = _totalDestructions;
		float timeInOppositeLane = Racer_Controller.TimeInOppositeLane ();

		//Set reuired fields in the game over panel
		totalDistanceText.text     = "TOTAL DISTANCE: "     + totalDistance.ToString ("F1") + "KMs";
		totalClosePassesText.text  = "CLOSE OVERTAKES: "    + totalClosePasses.ToString ();
		totalDestructionsText.text = "TAKE DOWNS: " 		+ totalDestructions.ToString ();
		oppositeLaneTimeText.text  = "OPPOSITE DIRECTION: " + timeInOppositeLane.ToString ("F1") + "s";

		//Calculate the rewards
		int totalDistanceReward 	   = Mathf.RoundToInt (totalDistance * 150f);
		int totalClosePassesReward     = totalClosePasses * 12;
		int totalDestructionsReward    = 0;
		//If the game mode is rage mode
		//Then reduce the destruction reward
		if(GameManager.gameMode == 2)
		{
			totalDestructionsReward = totalDestructions * 5;
		}
		else
		{
			totalDestructionsReward = totalDestructions * 15;
		}
		int timeInOppositeLaneReward   = Mathf.RoundToInt (timeInOppositeLane * 5f);

		int totalReward = totalDistanceReward + totalClosePassesReward + totalDestructionsReward + timeInOppositeLaneReward;

		//Set money count
		totalMoneyText.text = PlayerPrefs.GetInt ("AvailableMoney", 0).ToString ();
		//Increament the total money that the player has
		int updatedMoney = totalReward + PlayerPrefs.GetInt ("AvailableMoney", 0);

		PlayerPrefs.SetInt ("AvailableMoney", updatedMoney);

		//The number which will be displayed as each reward
		float displayNumber = 0;

		//Set reward fields in the game over panel
		if(totalDistanceReward > 0)
		{
			StartCoroutine (CountAnimation (0f, totalDistanceReward, totalDistanceRewardText));
			yield return new WaitForSecondsRealtime (countAnimationTime);
		}
		if(totalClosePassesReward > 0)
		{
			StartCoroutine (CountAnimation (0f, totalClosePassesReward, totalClosePassesRewardText));
			yield return new WaitForSecondsRealtime (countAnimationTime);
		}
		if(totalDestructionsReward > 0)
		{
			StartCoroutine (CountAnimation (0f, totalDestructionsReward, totalDestructionsRewardText));
			yield return new WaitForSecondsRealtime (countAnimationTime);
		}
		if(timeInOppositeLaneReward > 0 && GameManager.gameMode == 1)
		{
			StartCoroutine (CountAnimation (0f, timeInOppositeLaneReward, oppositeLaneTimeRewardText));
			yield return new WaitForSecondsRealtime (countAnimationTime);
		}

		if(totalReward > 0f)
		{
			StartCoroutine (CountAnimation (0f, totalReward, totalRewardText));
			yield return new WaitForSecondsRealtime (countAnimationTime);
		}

		StartCoroutine (CountAnimation ((updatedMoney - totalReward), updatedMoney, totalMoneyText));
	}

	IEnumerator CountAnimation(float initialValue, float targetValue, TextMeshProUGUI targetText)
	{
		float displayNumber = initialValue;
		for(float timer = 0; timer <= countAnimationTime; timer += Time.deltaTime)
		{
			float progress = timer / countAnimationTime;

			displayNumber = (int)Mathf.Lerp (initialValue, targetValue, progress);

			//Play count audio
			AudioManager.PlayCountSound ();

			targetText.text = displayNumber.ToString ();

			//yield return null;
			yield return null;
		}

		targetText.text = targetValue.ToString ();
	}

	public static float AccelerationSpeed()
	{
		return current.accelerationSpeed;
	}

	public static float DeAccelerationSpeed()
	{
		return current.deAccelerationSpeed;
	}

	public static float RealWorldMultiplier()
	{
		return current.realWorldMultiplier;
	}

	public static float DistanceToDestroy()
	{
		return current.distanceToDestroy;
	}

	public static Transform RacerTransform()
	{
		return current.racerTransform;
	}

	public static int GetCurrentLevelNumber()
	{
		return current._currentLevelNumber;
	}

	public static void IncreamentLevel()
	{
		current._currentLevelNumber++;

		current.levelIncreamentPrompt.SetActive (false);
		current.levelIncreamentPrompt.SetActive (true);

		AudioManager.PlayLevelIncreamentAudio ();
	}

	public static float MaximumLevelNumber()
	{
		return current.maximumLevelNumber;
	}

	public static bool IsIncreamentAllowed()
	{
		//If the racer is not already on the max speed level
		//then racer can increament the current speed level
		if(current._currentLevelNumber < current.maximumLevelNumber)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static float Mileage()
	{
		return current.mileage;
	}

	public static GameObject GuardRailEffect()
	{
		return current.guardRailEffect;
	}

	public static int PointsToAddDestroy()
	{
		return current.pointsToAddDestory;
	}

	public static int PointsToAddCloseCall()
	{
		return current.pointsToAddCloseCall;
	}

	public static void CloseCall()
	{
		//Add points to current score only if the game is not over
		if(!current._isGameOver)
		{
			//Enable close call message
			current.closeCallEffect.SetActive (false);
			current.closeCallEffect.SetActive (true);

			//Add points to current score
			current._score += current.pointsToAddCloseCall;

			//Increament total close calls
			current._totalCloseCalls++;

			//Play close overtake audio
			AudioManager.PlayClosePassAudio ();
		}
	}

	public static void ObstacleDestroyed()
	{
		//Add points to current score only if the game is not over
		if(!current._isGameOver)
		{
			//Enbale destruction message
			current.destructionMessageEffect.SetActive (false);
			current.destructionMessageEffect.SetActive (true);

			//Add points to current score
			current._score += current.pointsToAddDestory;

			//Increament number of total destroyed obstacles
			current._totalDestructions++;
		}
	}

	public static void UpdateOppositeLaneTimer(float timeInTheLane)
	{
		if(timeInTheLane > 0.01f)
		{
			current.oppositeLaneTimerText.text = timeInTheLane.ToString ("F1");

			if(current._oppositeLaneStatusFlag == false)
			{
				current._oppositeLaneStatusFlag = true;

				current.oppositeLaneTimerObject.SetActive (true);
			}
		}
		else
		{
			if(current._oppositeLaneStatusFlag == true)
			{
				current._oppositeLaneStatusFlag = false;

				current.oppositeLaneTimerObject.SetActive (false);
			}
		}
	}

	public static bool IsGameOver()
	{
		return current._isGameOver;
	}
}
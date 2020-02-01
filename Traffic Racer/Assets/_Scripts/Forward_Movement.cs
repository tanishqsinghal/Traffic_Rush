using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forward_Movement : MonoBehaviour
{
	static Forward_Movement current;						//The class holds static reference to itself

	private bool _isBoosting = false;						//Check whether the boost is up or not
	private bool _canBoost = true;							//Is the racer allowed to use nitrous or not
	private bool _isBraking = false;						//Check whether the brakes are applied or not
	private float _baseSpeed = 0f;							//Forward moving speed of the racer at start
	private float _currentSpeed = 0f;						//Current forward moving speed of the racer
	private float _startPositionZ = 0f;						//Start position of the racer when the current level started on Z axis
	private float _availableNitro = 1f;						//Amount of available fuel/ nitro
	private float _nitroEmptyPosition = 0f;					//Racer Position on Z axis when the nitro will become empty
	private int _scoreMultiplier = 1;						//Score multiplier which depends on boost time
	private float _scoreTimerStartPoint = 0f;				//Moment when the player started boost

	[SerializeField] float forwardSpeed = 5f;				//Speed by which the racer move forward
	[SerializeField] float distanceToIncreamentLevel = 1f;  //Distance that racer has to travel to increament level
	[SerializeField] float speedIncreamentPerLevel = 1f;	//How much speed will be increamented after each speed level
	//[SerializeField] float boostMultiplier = 1.5f;		//Racer speed will be increamented by this multiplier when boosting

	public GameObject nitrousEffect;						//Nitrous particle effect
	public GameObject nitroWarningEffect;					//Warning effect is shown when the available nitro is low
	public GameObject scoreMultiplierWarningEffect;			//Warning effect is shown when the racer reaches its maximum score multiplier

	void Awake()
	{
		current = this;

		//Set the base speed to start speed
		_baseSpeed = forwardSpeed;
	}

	void Start()
	{
		//Set current speed of the racer to the base speed
		_currentSpeed = forwardSpeed;

		//Get the start position of the racer
		_startPositionZ = transform.position.z;

		//Set the nitrous effect according to the active bike object
		GameObject activeBikeObject = LevelController.activeBikeObject;
		nitrousEffect  = activeBikeObject.transform.GetChild (1).transform.GetChild (5).gameObject;

		//Calaculate nitro empty position
		_nitroEmptyPosition = transform.position.z + LevelController.Mileage ();

		//Start engine sound
		AudioManager.StartNormalEngineAudio ();
	}

    // Update is called once per frame
    void Update()
    {
		/*//If the game is over then stop the racer
		if(LevelController.IsGameOver ())
		{
			forwardSpeed = 0f;
		}*/

		HandleNitroSystem ();

		HandleMovement ();

		HandleScoreMultiplier ();

		HandleLevelIncreament ();
    }

	void HandleNitroSystem()
	{
		if(_availableNitro > 0.01f)
		{
			//Calculate the available nitro amount with the travelled distance by the racer
			_availableNitro = (_nitroEmptyPosition - transform.position.z) / LevelController.Mileage ();

			//Update Nitro fill
			LevelController.UpdateNitroFill (_availableNitro);
		}

		//Display low nitro warning if it is low
		if(_availableNitro < 0.3f)
		{
			nitroWarningEffect.SetActive (true);
		}
		else
		{
			nitroWarningEffect.SetActive (false);
		}
	}

	void HandleMovement()
	{
		//Move the racer only if there is some nitro available
		if(_availableNitro > 0.01f)
		{
			//Racer is allowed to use nitrous
			_canBoost = true;

			//If the racer is boosting
			//Then increament the traffic racer speed
			//else revert the speed back to normal
			if(_isBoosting)
			{
				float boostedSpeed = forwardSpeed * 2f - 2f;
				_currentSpeed = Mathf.Lerp (_currentSpeed, boostedSpeed, LevelController.AccelerationSpeed () * Time.deltaTime);
			}
			else
			{
				//If brakes are applied
				//then stop the racer
				if(_isBraking)
				{
					_currentSpeed = Mathf.Lerp (_currentSpeed, 0, LevelController.DeAccelerationSpeed () * Time.deltaTime);
				}
				else
				{
					_currentSpeed = Mathf.Lerp (_currentSpeed, forwardSpeed, LevelController.DeAccelerationSpeed () * Time.deltaTime);
				}
			}
		}
		else
		{
			//Racer is not allowed to use nitrous
			_canBoost = false;

			//Disable nitrous
			DisableBoost ();

			//If there is no nitro left
			//Then deaacelerate the racer to 0
			_currentSpeed = Mathf.Lerp (_currentSpeed, 0, LevelController.DeAccelerationSpeed () * Time.deltaTime);

			//If the vehicle has stopped completely
			//Then call Game Over function
			if(_currentSpeed < 0.01f && !LevelController.IsGameOver ())
			{
				LevelController.GameOver ();
			}
		}

		//Move the transform in forward direction with forward speed
		transform.Translate (Vector3.forward * _currentSpeed * Time.deltaTime);
	}

	void HandleScoreMultiplier()
	{
		//Increament score multiplier only if the racer is boosting
		if(_isBoosting)
		{
			float timeSinceMultiplierStarted = Time.time - _scoreTimerStartPoint;

			if(timeSinceMultiplierStarted > (float)(_scoreMultiplier))
			{
				//Increament score multiplier
				//If it is less than 5
				if(_scoreMultiplier < 5)
				{
					//Reset the multiplier start time
					_scoreTimerStartPoint = Time.time;

					_scoreMultiplier++;
				}
				else
				{
					//Display score multiplier warning
					scoreMultiplierWarningEffect.SetActive (true);
				}
			}

			float timeToNextMultiplier = _scoreTimerStartPoint + _scoreMultiplier;

			//Calculate the fill amount of the score multiplier
			float fillAmount = 1f - ((timeToNextMultiplier - Time.time) / (float)(_scoreMultiplier));

			//update score multiplier ui
			LevelController.UpdateScoreMultiplierUI (_scoreMultiplier, fillAmount);
		}
		else
		{
			//Reset score multiplier
			_scoreMultiplier = 1;

			//Disable score multiplier warning
			scoreMultiplierWarningEffect.SetActive (false);

			//update score multiplier ui
			LevelController.UpdateScoreMultiplierUI (_scoreMultiplier, 0);
		}

		//If the game is over then also reset the speed multiplier
		if(LevelController.IsGameOver ())
		{
			//Reset score multiplier
			_scoreMultiplier = 1;

			//update score multiplier ui
			LevelController.UpdateScoreMultiplierUI (_scoreMultiplier, 0);
		}
	}

	void HandleLevelIncreament()
	{
		//If the racer has passed the distance which is required to increament level and allowed to increament
		//Then increament the current speed level
		if((Mathf.Abs (transform.position.z - _startPositionZ) > distanceToIncreamentLevel) && LevelController.IsIncreamentAllowed ())
		{
			//Store the current position of the racer as the start position of this speed level
			_startPositionZ = transform.position.z;

			//Increament forward speed of the racer
			forwardSpeed += speedIncreamentPerLevel;

			//Increament the current speed level
			LevelController.IncreamentLevel ();
		}
	}

	public void EnableBoost()
	{
		///Enable nitrous only if the racer is allowed to use them
		if(_canBoost)
		{
			//Enable Boost and disable brakes
			_isBoosting = true;
			DisableBrakes ();

			//Start the scoreTimer
			_scoreTimerStartPoint = Time.time;

			//Play boost audio
			AudioManager.PlayNitrousAudio ();

			//Shake the camera
			CameraScript.ShakeCamera (.1f, .1f, .1f, 0.5f, 0.01f);

			//Enable nitrous effect
			nitrousEffect.SetActive (true);

			//Start boosted engine sound
			AudioManager.StartBoostedEngineAudio ();
		}
	}

	public void DisableBoost()
	{
		_isBoosting = false;

		//Disable nitrous effect
		nitrousEffect.SetActive (false);

		if(_canBoost && !_isBraking)
		{
			//Start normal engine audio
			AudioManager.StartNormalEngineAudio ();
		}
	}

	public void EnableBrakes()
	{
		//Enable Brakes and disable boost
		_isBraking = true;

		DisableBoost ();

		//Play brakes sound
		AudioManager.PlayBrakesAudio ();

		//Mute engine sound
		AudioManager.StopEngineAudio ();
	}

	public void DisableBrakes()
	{
		_isBraking = false;

		//Start normal engine audio
		AudioManager.StartNormalEngineAudio ();
	}

	public void PickupNitro()
	{
		//Set the nitro capacity to full and
		//Calaculate nitro empty position
		_nitroEmptyPosition = transform.position.z + LevelController.Mileage ();
		_availableNitro = 1f;
	}

	//Get the current speed from anywhere
	public static float GetRacerSpeed()
	{
		return current._currentSpeed;
	}

	public static bool IsBoosting()
	{
		return current._isBoosting;
	}

	public static int ScoreMultiplier()
	{
		return current._scoreMultiplier;
	}

	public static float BaseSpeed()
	{
		return current._baseSpeed;
	}

	public static float SpeedIncreamentPerLevel()
	{
		return current.speedIncreamentPerLevel;
	}

	public static void GameOver()
	{
		//Reset the forward speed
		current.forwardSpeed = 0f;

		//Disable boost
		current.DisableBoost ();

		//stop engine
		AudioManager.StopEngineAudio ();
	}
}
